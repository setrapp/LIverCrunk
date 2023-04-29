using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class PlayerJump : MonoBehaviour {
	private Rigidbody body = null;
	private GroundCheck groundCheck = null;

	private bool jumping = false;
	private float jumpStartTime = 0;
	private float jumpStartY = 0;
	private bool jumpHolding = false;
	private float jumpHoldDuration = 0;
	private bool jumpReady = false;
	private float timeTryingJump = 0;

	public float preJumpAllowanceTime = 0.1f;
	public JumpData runJumpData = null;

	void Start() {
		body = GetComponent<Rigidbody>();
		groundCheck = GetComponent<GroundCheck>();
	}

	void Update() {
		bool tryJump = Input.GetAxis("Jump") > 0;
		if (tryJump) {
			timeTryingJump += Time.deltaTime;
		} else {
			timeTryingJump = 0;
		}

		jumpReady |= groundCheck.OnGround && timeTryingJump <= preJumpAllowanceTime;

		if (tryJump) {
			if (!jumping && jumpReady && groundCheck.OnGround) {
				jumping = true;
				jumpStartTime = Time.time;
				jumpStartY = body.position.y;
				jumpHolding = true;
				jumpHoldDuration = 0;
				jumpReady = false;
			} else if (jumpHolding) {
				jumping = true;
				jumpHoldDuration += Time.deltaTime;
			}
		}

		bool jumpDone = true;
		if (jumping) {
			jumpReady = false;
			groundCheck.RespectGravity = false;
			body.useGravity = false;

			var jumpData = runJumpData;

			var pos = body.position;
			var jumpHeight = 0f;
			(jumpHeight, jumpDone) = jumpData.GetHeight(Time.time - jumpStartTime, jumpHoldDuration);
			pos.y = jumpStartY + jumpHeight;

			body.position = pos;
		}

		if (jumpDone) {
			EndJump();
		}
	}

	void OnCollisionEnter() {
		EndJump();
	}

	void EndJump() {
		jumping = false;
		jumpStartTime = 0;
		jumpHolding = false;
		jumpHoldDuration = 0;
		groundCheck.RespectGravity = true;
	}
}
