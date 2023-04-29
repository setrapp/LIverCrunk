using UnityEngine;

public class Player : MonoBehaviour {
	public GlobalData globals = null;

	void Awake() {
		if (globals != null) {
			globals.Init();
		}
	}
}
