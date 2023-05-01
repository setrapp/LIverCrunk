using UnityEngine;
using System.Collections.Generic;

public class DisableOnAwake : MonoBehaviour {
	public List<GameObject> disablees = null;

	void Awake() {
		foreach (var disablee in disablees) {
			if (disablee != null) {
				disablee.SetActive(false);
			}
		}
	}
}
