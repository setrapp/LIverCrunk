using UnityEngine;

public class PlayerAnim : MonoBehaviour {
	public Animator anim;
	public PlayerMove playerMove;
	public PlayerJump playerJump;
	public GroundCheck groundCheck;

	private string currentPose = "Idle";

	void Update() {
		var nextPose = "Idle";

		if (groundCheck.OnGround) {
			nextPose = playerMove.GetAnimParam();
		} else {
			nextPose = playerJump.GetAnimParam();
		}

		if (nextPose != currentPose) {
			anim.SetTrigger(nextPose);
			currentPose = nextPose;
		}
	}
}
