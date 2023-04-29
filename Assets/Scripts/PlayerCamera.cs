using UnityEngine;

public class PlayerCamera : MonoBehaviour {
	public Transform player = null;

	void Update() {
		var targetPos = player.position;
		targetPos.z = transform.position.z;

		transform.position = targetPos;
	}
}
