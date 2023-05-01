using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Mother : MonoBehaviour {
	public Rigidbody player = null;
	//public float barfSpeed = 100f;
	public Collider maw = null;
	public List<MotherText> texts;

	public Image liverMeter = null;

	void Start() {
		if (LiveGlobals.Instance != null) {
			for (int i = 0; i < LiveGlobals.Instance.heldLivers.Count; i++) {
				LiveGlobals.Instance.givenLivers.Add(LiveGlobals.Instance.heldLivers[i]);
			}

			var liverWorth = 0f;
			for (int i = 0; i < LiveGlobals.Instance.givenLivers.Count; i++) {
				liverWorth += LiveGlobals.Instance.givenLivers[i]?.HealthPortion ?? 0;
			}

			liverMeter.fillAmount = liverWorth / LiveGlobals.Instance.data.goalLiverWorth;
		}
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
		foreach (var text in texts) {
			if (text != null && text.gameObject.activeSelf) {
				text.AddLines(null);
				text.OnStartTalking.Invoke();
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
