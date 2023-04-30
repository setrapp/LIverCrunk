using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck), typeof(PlayerMove))]
public class PlayerJump : MonoBehaviour {
	private Rigidbody body = null;
	private GroundCheck groundCheck = null;
	private PlayerMove mover = null;
	private PlayerFeed feed = null;

	private bool jumping = false;
	private float jumpStartTime = 0;
	private float jumpStartY = 0;
	private bool jumpHolding = false;
	private float jumpHoldDuration = 0;
	private bool jumpReady = false;
	private float timeTryingJump = 0;
	private JumpData jumpData = null;
	public JumpData ActiveJumpData => jumpData;

	public float preJumpAllowanceTime = 0.1f;
	public JumpData standJumpData = null;
	public JumpData runJumpData = null;
	public JumpData sprintJumpData = null;
	public JumpData brakeJumpData = null;

	public bool contextJumpShowing = true;
	public bool runJumpShowing = false;
	public bool sprintJumpShowing = false;
	public bool brakeJumpShowing = false;

	void Start() {
		body = GetComponent<Rigidbody>();
		groundCheck = GetComponent<GroundCheck>();
		mover = GetComponent<PlayerMove>();
		feed = GetComponent<PlayerFeed>();
	}

	void FixedUpdate() {
#if UNITY_EDITOR
		if (Input.GetKeyDown("j")) { toggleJumpsShowing(!contextJumpShowing, false, false, false); }
		if (Input.GetKeyDown("k")) { toggleJumpsShowing(false, !runJumpShowing, false, false); }
		if (Input.GetKeyDown("l")) { toggleJumpsShowing(false, false, !sprintJumpShowing, false); }
		if (Input.GetKeyDown("h")) { toggleJumpsShowing(false, false, false, !brakeJumpShowing); }
#endif

		if (feed.IsFeeding) {
			EndJump();
		} else {
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

				jumpData = pickJump();

				var pos = body.position;
				var jumpHeight = 0f;
				(jumpHeight, jumpDone) = jumpData.GetHeight(Time.time - jumpStartTime, jumpHoldDuration);
				pos.y = jumpStartY + jumpHeight;

				body.position = pos;

				if (jumpDone) {
					EndJump();
				}
			}
		}

		body.useGravity = !groundCheck.OnGround && !jumping && !feed.IsFeeding;
	}

	JumpData pickJump() {
#if UNITY_EDITOR
		if (!Application.isPlaying) { return standJumpData; }
#endif

		if (groundCheck.OnGround) {
			if (mover.ActiveMoveData == mover.runData) {
				if (Mathf.Abs(body.velocity.x) > 0.001f) { return runJumpData; }
				else { return standJumpData; }
			}
			if (mover.ActiveMoveData == mover.sprintData) { return sprintJumpData; }
			if (mover.ActiveMoveData == mover.brakeData) { return brakeJumpData; }
		}

		return jumpData;
	}

	public string GetAnimParam() {
		//if (jumping) { return "Jump"; }
		return "Jump";//"Idle";
	}

	public void EndJump() {
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
		if (body == null) { body = GetComponent<Rigidbody>(); }
		if (groundCheck == null) { groundCheck = GetComponent<GroundCheck>(); }
		if (mover == null) { mover = GetComponent<PlayerMove>(); }

		bool show123 = Input.GetKey(";");
		bool show3 = !show123 && Input.GetKey(".");
		bool show2 = !show123 && !show3 && Input.GetKey("p");
		bool show1 = !show123 && !show3 && !show2;
		//Debug.Log("" + show123 + " " + show3 + " " + show2 + " " + show1);
		
		var resolution = 20;
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
		if (contextJumpShowing) {
			showArc = true;
			data = pickJump();
			moveSpeed = body.velocity.x;//mover.ActiveMoveData.MaxSpeed * direction;
			if (data == standJumpData) {
				Gizmos.color = standColor;
				//moveSpeed = 0f;
			}
			else if (data == runJumpData) { Gizmos.color = runColor; }
			else if (data == sprintJumpData) { Gizmos.color = sprintColor; }
			else if (data == brakeJumpData) {
				Gizmos.color = brakeColor;
				//moveSpeed = body.velocity.x;
			}

		}
		// Running Jump
		else if (runJumpShowing) {
			showArc = true;
			data = runJumpData;
			moveSpeed = mover.runData.MaxSpeed * direction;
			Gizmos.color = runColor;
		}
		// Sprinting Jump
		else if (sprintJumpShowing) {
			showArc = true;
			data = sprintJumpData;
			moveSpeed = mover.sprintData.MaxSpeed * direction;
			Gizmos.color = sprintColor;
		}
		// Braking Jump
		else if (brakeJumpShowing) {
			showArc = true;
			data = brakeJumpData;
			moveSpeed = body.velocity.x;
			Gizmos.color = brakeColor;
		}

		if (showArc) {
			minJump = sampleJumpArc(data, resolution, moveSpeed, false, show1, show2, show3);

			if (groundCheck.OnGround || jumping) {
				maxJump = sampleJumpArc(data, resolution, moveSpeed, true, show1, show2, show3);
			}
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

	void toggleJumpsShowing(bool showContext, bool showRun, bool showSprint, bool showBrake) {
		contextJumpShowing = showContext;
		runJumpShowing = showRun;
		sprintJumpShowing = showSprint;
		brakeJumpShowing = showBrake;
	}

	List<Vector3> sampleJumpArc(JumpData data, int resolution, float moveSpeed, bool fullHold, bool show1, bool show2, bool show3)
	{
		var points = new List<Vector3>();
		if (resolution > 0) {
			var height = 0f;
			var jumpDone = false;
			if (data != null && data.maxJumpDuration >= 0.001f) {
				if (show1) {
					var timeStep = data.maxJumpDuration / resolution;
					var startStep = 0;

					if (!groundCheck.OnGround) {
						if (jumping) {
							startStep = (int)(resolution * Mathf.Clamp01((Time.time - jumpStartTime) / data.maxJumpDuration));
						} else {
							startStep = resolution;
						}
					}

					(var baselineHeight, var baselineJumpDone) = data.GetHeight(timeStep * startStep, fullHold ? data.maxJumpDuration : jumpHoldDuration);

					for (int i = startStep; i <= resolution; i++) {
						(height, jumpDone) = data.GetHeight(timeStep * i, fullHold ? data.maxJumpDuration : jumpHoldDuration);
						height -= baselineHeight;
						points.Add(transform.position + new Vector3(moveSpeed * (timeStep * (i - startStep)), height, 0));
					}

					addGravityToArc(points, timeStep, moveSpeed);
				}

				if (show2) { }

				if (show3) { }
			}
		}

		return points;
	}

	void addGravityToArc(List<Vector3> points, float timeStep, float moveSpeed) {
		if (points == null || points.Count < 1) { return; }

		var x = points[points.Count - 1].x;
		var height = points[points.Count - 1].y;
		var minHeight = height - 100;
		var fallVel = Mathf.Min(body.velocity.y, 0);
		var arcDone = false;

		while (!arcDone) {
			fallVel += Physics.gravity.y * timeStep; // TODO Not accounting for drag
			height += fallVel * timeStep;
			x += moveSpeed * timeStep;
			var testPoint = new Vector3(x, height, 0);
			var step = (testPoint - points[points.Count - 1]);
			var stepDist = step.magnitude;

			if (height < minHeight || Physics.Raycast(points[points.Count - 1], step / stepDist, stepDist)) {
				arcDone = true;
			}

			points.Add(testPoint);
		}
	}
#endif
}
