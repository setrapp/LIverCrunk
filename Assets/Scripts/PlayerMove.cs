using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class PlayerMove : MonoBehaviour {
	private Rigidbody body = null;
	private GroundCheck groundCheck = null;
	private PlayerFeed feed = null;

	public Transform render = null;
	public MoveData runData = null;
	public MoveData sprintData = null;
	public MoveData brakeData = null;
	public MoveData airData = null;

	private MoveData moveData = null;
	public MoveData ActiveMoveData => moveData;

	private float moveStartTime = 0;
	private bool collidingInAir = false;

	[HideInInspector] public bool sprintDisabled = false;
	
	void Start() {
		body = GetComponent<Rigidbody>();
		groundCheck = GetComponent<GroundCheck>();
		feed = GetComponent<PlayerFeed>();
	}

	void FixedUpdate() {
		var horizontal = Input.GetAxis("Horizontal");
		var tryMove = Mathf.Abs(horizontal) >= 0.001f;
		var direction = horizontal < 0 ? -1 : 1;
		var velocity = body.velocity;
		var velX = velocity.x;
		var curSpeed = Mathf.Abs(velX);
		var minBrakeSpeed = groundCheck.OnGround ? runData.MaxSpeed : airData.MaxSpeed;
		bool facingWrongDirection = (velX > minBrakeSpeed && horizontal < 0) || (velX < -minBrakeSpeed && horizontal > 0);
		var sprintAxis = Input.GetAxis("Sprint");
		if (sprintDisabled) {
			sprintAxis = 0;
		}

		moveData = runData;
		if (facingWrongDirection) {
			moveData = brakeData;
		} else if (!groundCheck.OnGround) {
			moveData = airData;
		} else if ((sprintAxis >= 0.001f && Mathf.Abs(horizontal) > 0.001f) || curSpeed > runData.MaxSpeed) {
			moveData = sprintData;
		}
		
		if (!tryMove || facingWrongDirection) {
			moveStartTime = Time.time;
		}

		var moveDuration = Time.time - moveStartTime;

		if (tryMove) {
			if (moveData == brakeData) {
				velX = moveData.ApplyDampening(velX);
				transform.right = Vector3.right * (groundCheck.OnGround ? (velX < 0 ? -1 : 1) : direction);
			} else if (moveData == airData) {
				velX = Mathf.Max(curSpeed, moveData.GetSpeed(moveDuration)) * direction;
				transform.right = Vector3.right * direction;
			} else if (moveData == sprintData) {
				if (sprintAxis < 0.001) {
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

		if (!body.isKinematic && !collidingInAir && (feed == null || !feed.IsFeeding)) {
			body.velocity = velocity;
		}

		collidingInAir = false;
	}

	public string GetAnimParam() {
		if (Mathf.Abs(body.velocity.x) >= 0.001f) {
			if (ActiveMoveData == runData) { return "Run"; }
			else if (ActiveMoveData == sprintData) { return "RunFast"; }
			else if (ActiveMoveData == brakeData) { return "Idle"; }
		}

		return "Idle";
	}

	/*void OnCollisionEnter(Collision collision) {
		activeCollisions.Add(collision);
	}

	void OnCollisionExit(Collision collision) {
		activeCollisions.Add(collision);
	}*/

	void OnCollisionStay(Collision collision) {
		var contact = collision.GetContact(0);
		if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y) && Vector3.Dot(contact.normal, transform.right) < 0) {
			//Debug.Log("Wall");
			if (groundCheck.OnGround) {
				//collidingInAir = true;
			} else {
				// If we're colliding with something in the air, we should not be able alter our trajectory until we're free (or likely we get away from a wall).
				collidingInAir = true;
			}
		}
	}
}
