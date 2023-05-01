using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Mother : MonoBehaviour {
	public Rigidbody player = null;
	//public float barfSpeed = 100f;
	public Collider maw = null;
	public List<MotherText> texts;

	public Image liverMeter = null;
	public MotherState state = MotherState.Delivery;


	public void DeliverLivers() {
		GetComponent<Animator>().SetTrigger("Delivery");
	}

	public void AcknowledgeDelivery() {
		if (LiveGlobals.Instance != null) {
			for (int i = 0; i < LiveGlobals.Instance.heldLivers.Count; i++) {
				LiveGlobals.Instance.givenLivers.Add(LiveGlobals.Instance.heldLivers[i]);
				LiveGlobals.Instance.harvestedLiverIds.Add(LiveGlobals.Instance.heldLivers[i].id);
			}
			LiveGlobals.Instance.priorLiversGiven += LiveGlobals.Instance.heldLivers.Count;

			var liverWorth = 0f;
			for (int i = 0; i < LiveGlobals.Instance.givenLivers.Count; i++) {
				liverWorth += LiveGlobals.Instance.givenLivers[i]?.Worth ?? 0;
			}

			liverMeter.fillAmount = liverWorth / LiveGlobals.Instance.GoalLiverWorth;
			Debug.Log(LiveGlobals.Instance.GoalLiverWorth);
		}

		LiveGlobals.Instance.AttemptReplenishHumans();

		SpeakLines();

		LiveGlobals.Instance.heldLivers.Clear();
	}

	void Update() {
		maw.enabled = player.velocity.y <= 0;
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
				text.AddLines(lines);
				//text.OnStartTalking.Invoke();
				//text.GetComponentInParent<Animator>().SetTrigger("TextBox");
			}
		}
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

