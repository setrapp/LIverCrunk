using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

public class MotherText : MonoBehaviour {
	public GameObject textBox = null;
	public TextMeshProUGUI text = null;
	private List<string> lines = new List<string>();

	private int lineIndex = 0;

	public UnityEvent OnStartTalking;
	public UnityEvent OnStopTalking;
	public UnityEvent WhileWaitingToLeave;

	bool linesChecked = false;

	void Update() {
		if (lines.Count > 0) {
			textBox.SetActive(true);
			text.text = lines[0];

			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) {
				lines.RemoveAt(0);
			}

			if (lines.Count < 1) {
				OnStopTalking.Invoke();
			}
		} else {
			textBox.SetActive(false);
			if (linesChecked) {
				WhileWaitingToLeave.Invoke();
			}
		}
	}

	public void AddLines(List<string> newLines) {
		linesChecked = true;
		bool wasTalking = lines.Count > 0;
		if (newLines != null && newLines.Count > 0) {
			lines.AddRange(newLines);
		}

		if (!wasTalking) {
			if (lines.Count > 0) { OnStartTalking.Invoke(); }
			else { OnStopTalking.Invoke(); }
		}
	}
	
}
