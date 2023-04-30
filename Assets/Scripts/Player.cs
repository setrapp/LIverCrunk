using UnityEngine;

public class Player : MonoBehaviour {
	public GlobalData globals = null;
	private Vector3 startPos = Vector3.zero;

	void Awake() {
		if (globals != null) {
			globals.Init();
		}

		startPos = transform.position;
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown("r")) {
			transform.position = startPos;
		}
#endif
	}
}
