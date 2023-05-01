using UnityEngine;

public class PlayerDisabler : MonoBehaviour {
	private PlayerMove mover = null;
	private PlayerJump jumper = null;

	void Start() {
		mover = GetComponent<PlayerMove>();
		jumper = GetComponent<PlayerJump>();
	}

	public void ToggleDisable(bool disable) {
		if (mover != null) {
			mover.sprintDisabled = disable;
		}

		if (jumper != null) {
			jumper.jumpDisabled = disable;
		}
	}
}
