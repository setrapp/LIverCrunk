using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class PlayerMove : MonoBehaviour {
	private Rigidbody body = null;
	private GroundCheck groundCheck = null;
	public Transform render = null;
	public MoveData runData = null;
	public MoveData sprintData = null;
	public MoveData brakeData = null;
	public MoveData airData = null;

	private MoveData moveData = null;
	public MoveData ActiveMoveData => moveData;

	private float moveStartTime = 0;
	
	void Start() {
		body = GetComponent<Rigidbody>();
		groundCheck = GetComponent<GroundCheck>();
	}

	void Update() {
		var horizontal = Input.GetAxis("Horizontal");
		var tryMove = Mathf.Abs(horizontal) >= 0.001f;
		var direction = horizontal < 0 ? -1 : 1;
		var velocity = body.velocity;
		var velX = velocity.x;
		var curSpeed = Mathf.Abs(velX);
		var minBrakeSpeed = groundCheck.OnGround ? runData.MaxSpeed : airData.MaxSpeed;
		bool facingWrongDirection = (velX > minBrakeSpeed && horizontal < 0) || (velX < -minBrakeSpeed && horizontal > 0);

		moveData = runData;
		if (facingWrongDirection) {
			moveData = brakeData;
		} else if (!groundCheck.OnGround) {
			moveData = airData;
		} else if ((Input.GetAxis("Sprint") >= 0.001f && Mathf.Abs(horizontal) > 0.001f) || curSpeed > runData.MaxSpeed) {
			moveData = sprintData;
		}
		
		if (!tryMove || facingWrongDirection) {
			moveStartTime = Time.time;
		}

		var moveDuration = Time.time - moveStartTime;

		if (tryMove) {
			if (moveData == brakeData) {
				velX = moveData.ApplyDampening(velX);
				transform.right = Vector3.right * (velX < 0 ? -1 : 1);
			} else if (moveData == airData) {
				velX = Mathf.Max(curSpeed, moveData.GetSpeed(moveDuration)) * direction;
				transform.right = Vector3.right * direction;
			} else if (moveData == sprintData) {
				if (Input.GetAxis("Sprint") < 0.001) {
					velX = moveData.ApplyDampening(velX);
				} else {
					velX = moveData.GetSpeed(moveDuration) * direction;
					transform.right = Vector3.right * direction;
				}
			} else {
				velX = moveData.GetSpeed(moveDuration) * direction;
				transform.right = Vector3.right * direction;
			}
		} else {
			velX = moveData.ApplyDampening(velX);
		}

		var speedCutoff = runData.MinSpeed;
		if (!groundCheck.OnGround) {
			speedCutoff = airData.MinSpeed;
		}

		if (Mathf.Abs(velX) < speedCutoff) {
			velX = 0;
		}

		var rotation = render.transform.rotation;
		if (moveData == runData) {// || moveData == airData) {
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
		}
		if (moveData == sprintData) {
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
		}
		if (moveData == brakeData && groundCheck.OnGround) {
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 25);
		}
		render.transform.rotation = rotation;

		velocity.x = velX;
		body.velocity = velocity;
	}

	public string GetAnimParam() {
		if (Mathf.Abs(body.velocity.x) >= 0.001f) {
			if (ActiveMoveData == runData) { return "Run"; }
			else if (ActiveMoveData == sprintData) { return "RunFast"; }
			else if (ActiveMoveData == brakeData) { return "Idle"; }
		}

		return "Idle";
	}

}
