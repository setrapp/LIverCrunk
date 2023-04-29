using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour {
	private Rigidbody body = null;
	public Transform render = null;
	public MoveData runData = null;
	public MoveData sprintData = null;
	public MoveData brakeData = null;
	// public MoveData airData = null;
	
	void Start() {
		body = GetComponent<Rigidbody>();
	}

	void Update() {
		var horizontal = Input.GetAxis("Horizontal");
		var direction = horizontal < 0 ? -1 : 1;
		var velocity = body.velocity;
		var velX = velocity.x;
		var curSpeed = Mathf.Abs(velX);

		var moveData = runData;
		if ((velX > runData.MaxSpeed && horizontal < 0) || (velX < -runData.MaxSpeed && horizontal > 0)) {
			moveData = brakeData;
		} else if ((Input.GetAxis("Sprint") > 0 && Mathf.Abs(horizontal) > 0.001f) || curSpeed > runData.MaxSpeed) {
			moveData = sprintData;
		}

		if (Mathf.Abs(horizontal) >= 0.001f) {
			if (moveData == brakeData) {
				velX = moveData.ApplyDampening(velX);
				transform.right = Vector3.right * (velX < 0 ? -1 : 1);
			} else {
				velX = moveData.GetSpeed() * direction;
				transform.right = Vector3.right * direction;
			}
		} else {
			velX = moveData.ApplyDampening(velX);
		}

		var rotation = render.transform.rotation;
		if (moveData == runData) {
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
		}
		if (moveData == sprintData) {
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, -75);
		}
		if (moveData == brakeData) {
			rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 25);
		}
		render.transform.rotation = rotation;

		velocity.x = velX;
		body.velocity = velocity;
	}

}
