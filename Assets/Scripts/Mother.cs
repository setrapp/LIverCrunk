using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Mother : MonoBehaviour {
	public Rigidbody player = null;
	//public float barfSpeed = 100f;
	public Collider maw = null;
	public Collider noEscape = null;
	public List<MotherText> texts;

	public Image liverMeter = null;
	public MotherState state = MotherState.Delivery;

	bool sufficientLivers => LiveGlobals.Instance?.sufficientLivers ?? false;
	public Animator cameraAnimator;
	public GameObject victory;

	public Animator liverDeliveryAnimator = null;

	public float fillMeterDuration = 1;

	void Update() {
		maw.enabled = player.velocity.y <= 0;
	}

	public void DeliverLivers() {
		GetComponent<Animator>().SetTrigger("Delivery");
	}

	public void AcknowledgeDelivery() {
		StartCoroutine(acknowledgeDelivery());
	}

	private IEnumerator acknowledgeDelivery() {
		if (liverDeliveryAnimator != null) {
			var liverDelivers = Mathf.Min(LiveGlobals.Instance.heldLivers.Count, 3);
			liverDeliveryAnimator.SetTrigger($"Liver_{liverDelivers}");
		}

		if (LiveGlobals.Instance != null) {
			var roomForPlayer = 0.9484f;
			var liverWorth = 0f;
			for (int i = 0; i < LiveGlobals.Instance.givenLivers.Count; i++) {
				liverWorth += LiveGlobals.Instance.givenLivers[i]?.Worth ?? 0;
			}
			var startFill = Mathf.Clamp(liverWorth / LiveGlobals.Instance.GoalLiverWorth, 0, roomForPlayer);

			for (int i = 0; i < LiveGlobals.Instance.heldLivers.Count; i++) {
				LiveGlobals.Instance.givenLivers.Add(LiveGlobals.Instance.heldLivers[i]);
				LiveGlobals.Instance.harvestedLiverIds.Add(LiveGlobals.Instance.heldLivers[i].id);
				liverWorth += LiveGlobals.Instance.heldLivers[i].Worth;
			}
			LiveGlobals.Instance.priorLiversGiven += LiveGlobals.Instance.heldLivers.Count;

			var endFill = Mathf.Clamp(liverWorth / LiveGlobals.Instance.GoalLiverWorth, 0, roomForPlayer);


			if (fillMeterDuration > 0 && endFill - startFill > 0.0001f) {
				var liveringDuration = 0f;
				while (liveringDuration < fillMeterDuration) {
					var portion = liveringDuration / fillMeterDuration;
					liverMeter.fillAmount = (startFill * (1 - portion) + (endFill * portion));
					liveringDuration += Time.deltaTime;
					yield return null;
				}
			} else { liverMeter.fillAmount = endFill; }

			if (liverWorth >= LiveGlobals.Instance.GoalLiverWorth) {
				LiveGlobals.Instance.sufficientLivers = true;
			}
			//Debug.Log(liverWorth + " " + LiveGlobals.Instance.GoalLiverWorth);
		}

		LiveGlobals.Instance.AttemptReplenishHumans();

		SpeakLines();
	}

	public void AttemptVictory() {
		StartCoroutine(attemptVictory());
	}

	private IEnumerator attemptVictory() {
		if (sufficientLivers) {
			maw.gameObject.SetActive(false);
			noEscape.gameObject.SetActive(true);
			yield return new WaitForSeconds(1);

			var startFill = liverMeter.fillAmount;
			var endFill = 1f;
			if (fillMeterDuration > 0 && endFill - startFill > 0.0001f) {
				var liveringDuration = 0f;
				while (liveringDuration < fillMeterDuration) {
					var portion = liveringDuration / fillMeterDuration;
					liverMeter.fillAmount = (startFill * (1 - portion) + (endFill * portion));
					liveringDuration += Time.deltaTime;
					yield return null;
				}
			} else { liverMeter.fillAmount = endFill; }

			yield return new WaitForSeconds(0.5f);

			if (cameraAnimator != null) { cameraAnimator.SetTrigger("CameraShake"); }
			if (victory != null) { victory.SetActive(true); }
		}
	}

	public void BarfPlayer() {
		player.gameObject.SetActive(true);
		//player.velocity = new Vector3(0, barfSpeed, 0);
		GetComponent<Animator>().SetTrigger("Rebirth");
	}

	public void SpeakLines() {
		// In case the rigidbody failed to launch properly, manually place the player.
		if (player != null && player.transform.localPosition.y < -3) {
			player.transform.localPosition = new Vector3(player.transform.localPosition.x, -3, player.transform.localPosition.z);
		}

		MotherDialog bestDialog = LiveGlobals.Instance.GetMotherDialog(state);
		List<string> lines = bestDialog?.lines ?? null;
		foreach (var text in texts) {
			if (text != null && text.gameObject.activeSelf) {
				text.AddLines(lines, bestDialog.OnComplete);
				//text.OnStartTalking.Invoke();
				//text.GetComponentInParent<Animator>().SetTrigger("TextBox");
			}
		}

		LiveGlobals.Instance.heldLivers.Clear();
		LiveGlobals.Instance.vesselIds.Clear();
	}

	public void StopLines() {
		foreach (var text in texts) {
			if (text != null && text.gameObject.activeSelf) {
				//text.GetComponentInParent<Animator>().SetTrigger("NoTextBox");
			}
		}

	}
}

[System.Serializable]
public enum MotherState {
	StartGame = 0,
	Respawn,
	Delivery,
}

