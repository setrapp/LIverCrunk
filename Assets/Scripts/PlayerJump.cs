using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck), typeof(PlayerMove))]
public class PlayerJump : MonoBehaviour {
	private Rigidbody body = null;
	private GroundCheck groundCheck = null;
	private PlayerMove mover = null;

	private bool jumping = false;
	private float jumpStartTime = 0;
	private float jumpStartY = 0;
	private bool jumpHolding = false;
	private float jumpHoldDuration = 0;
	private bool jumpReady = false;
	private float timeTryingJump = 0;

	public float preJumpAllowanceTime = 0.1f;
	public JumpData standJumpData = null;
	public JumpData runJumpData = null;
	public JumpData sprintJumpData = null;
	public JumpData brakeJumpData = null;

	void Start() {
		body = GetComponent<Rigidbody>();
		groundCheck = GetComponent<GroundCheck>();
		mover = GetComponent<PlayerMove>();
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

			var jumpData = pickJump();

			var pos = body.position;
			var jumpHeight = 0f;
			(jumpHeight, jumpDone) = jumpData.GetHeight(Time.time - jumpStartTime, jumpHoldDuration);
			pos.y = jumpStartY + jumpHeight;

			body.position = pos;

			if (jumpDone) {
				EndJump();
			}
		}

		body.useGravity = !groundCheck.OnGround && !jumping;
	}

	JumpData pickJump() {
		if (groundCheck.OnGround) {
			if (mover.ActiveMoveData == mover.runData) { return runJumpData; }
			if (mover.ActiveMoveData == mover.sprintData) { return sprintJumpData; }
			if (mover.ActiveMoveData == mover.brakeData) { return brakeJumpData; }
		}

		return standJumpData;
	}

	public string GetAnimParam() {
		return "Idle";
	}

	void EndJump() {
		jumping = false;
		jumpStartTime = 0;
		jumpHolding = false;
		jumpHoldDuration = 0;
	}

	void OnCollisionEnter(Collision collision) {
		EndJump();
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (body == null) {
			body = GetComponent<Rigidbody>();
		}

		bool show123 = Input.GetKey(";");
		bool show3 = !show123 && Input.GetKey(".");
		bool show2 = !show123 && !show3 && Input.GetKey("p");
		bool show1 = !show123 && !show3 && !show2;
		//Debug.Log("" + show123 + " " + show3 + " " + show2 + " " + show1);
		
		var resolution = 10;
		var standColor = Color.white;
		var runColor = Color.blue;
		var sprintColor = Color.green;
		var brakeColor = Color.red;

		bool showArc = false;
		var data = standJumpData;
		var moveSpeed = 0f;
		var direction = transform.right.x < 0 ? -1 : 1;
		List<Vector3> minJump = null;
		List<Vector3> maxJump = null;

		// Context Jump
		if (Input.GetKey("j")) {
			showArc = true;
			data = pickJump();
			moveSpeed = mover.ActiveMoveData.MaxSpeed * direction;
			if (data == standJumpData) {
				Gizmos.color = standColor;
				moveSpeed = 0f;
			}
			if (data == runJumpData) { Gizmos.color = runColor; }
			if (data == sprintJumpData) { Gizmos.color = sprintColor; }
			if (data == brakeJumpData) { Gizmos.color = brakeColor; }

		}
		// Running Jump
		else if (Input.GetKey("k")) {
			showArc = true;
			data = runJumpData;
			moveSpeed = mover.runData.MaxSpeed * direction;
			Gizmos.color = runColor;
		}
		// Sprinting Jump
		else if (Input.GetKey("l")) {
			showArc = true;
			data = sprintJumpData;
			moveSpeed = mover.sprintData.MaxSpeed * direction;
			Gizmos.color = sprintColor;
		}
		// Braking Jump
		else if (Input.GetKey("h")) {
			showArc = true;
			data = brakeJumpData;
			moveSpeed = body.velocity.x;
			Gizmos.color = brakeColor;
		}

		if (showArc) {
			minJump = sampleJumpArc(data, resolution, moveSpeed, false, show1, show2, show3);
			maxJump = sampleJumpArc(data, resolution, moveSpeed, true, show1, show2, show3);
		}

		if (minJump != null) {
			//Gizmos.color *= 0.5f;
			for (int i = 0; i < minJump.Count - 1; i++) {
				Gizmos.DrawLine(minJump[i], minJump[i + 1]);
			}
		}

		if (maxJump != null) {
			var color = Gizmos.color;
			color.a = 0.5f;
			Gizmos.color = color;
			for (int i = 0; i < maxJump.Count - 1; i++) {
				Gizmos.DrawLine(maxJump[i], maxJump[i + 1]);
			}
		}
	}

	List<Vector3> sampleJumpArc(JumpData data, int resolution, float moveSpeed, bool fullHold, bool show1, bool show2, bool show3)
	{
		var points = new List<Vector3>();
		if (data.maxJumpDuration >= 0.001f) {
			if (show1) {
				var timeStep = data.maxJumpDuration / resolution;
				for (int i = 0; i < resolution; i++) {
					var sampleTime = timeStep * i;
					(var height, bool jumpDone) = data.GetHeight(sampleTime, fullHold ? data.maxJumpDuration : 0);
					points.Add(transform.position + new Vector3(moveSpeed * sampleTime, height, 0));
				}
			}
		}

		return points;
	}
#endif
}
