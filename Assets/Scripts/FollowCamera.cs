using UnityEngine;

public class FollowCamera : MonoBehaviour {
	public Transform followTarget = null;

	void Update() {
		var targetPos = followTarget.position;
		targetPos.z = transform.position.z;

		transform.position = targetPos;
	}
}
