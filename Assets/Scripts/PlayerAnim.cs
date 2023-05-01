using UnityEngine;

public class PlayerAnim : MonoBehaviour {
	public Animator anim;
	public Player player;
	public PlayerMove playerMove;
	public PlayerJump playerJump;
	public PlayerFeed playerFeed;
	public GroundCheck groundCheck;

	private string currentPose = "Idle";

	void Update() {
		var nextPose = "Idle";

		if (groundCheck.OnGround) {
			nextPose = playerMove.GetAnimParam();
		} else {
			if (playerFeed != null && playerFeed.IsFeeding) {
				nextPose = playerFeed.GetAnimParam();
			} else {
				nextPose = playerJump.GetAnimParam();
			}
		}

		if (nextPose != currentPose) {
			anim.SetTrigger(nextPose);
			currentPose = nextPose;
		}
	}
}
