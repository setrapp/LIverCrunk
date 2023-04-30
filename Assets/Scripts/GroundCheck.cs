using UnityEngine;

public class GroundCheck : MonoBehaviour {
	public float midRayLength = 1.01f;
	public float sideRayLength = 1.01f;
	public float sideRayOffset = 1f;

	private Ray midRay = new Ray(Vector3.zero, Vector3.down);
	private Ray backRay = new Ray(Vector3.zero, Vector3.down);
	private Ray frontRay = new Ray(Vector3.zero, Vector3.down);

	private bool midGrounded = false;
	private bool backGrounded = false;
	private bool frontGrounded = false;

	public bool OnGround => midGrounded || backGrounded || frontGrounded;

	void FixedUpdate() {
		midRay.origin = transform.position;
		backRay.origin = transform.position + (transform.right * -sideRayOffset);
		frontRay.origin = transform.position + (transform.right * sideRayOffset);

		var req_layer = LayerMask.GetMask("Platform");

		midGrounded = backGrounded = frontGrounded = false;
		RaycastHit midHit, backHit, frontHit;

		if (Physics.Raycast(midRay, out midHit, midRayLength, req_layer) && (midHit.point - midRay.origin).sqrMagnitude > midRayLength / 2) {
			midGrounded = true;
		}
		if (Physics.Raycast(backRay, out backHit, sideRayLength, req_layer) && (backHit.point - backRay.origin).sqrMagnitude > sideRayLength / 2) {
			backGrounded = true;
		}
		if (Physics.Raycast(frontRay, out frontHit, sideRayLength, req_layer) && (frontHit.point - frontRay.origin).sqrMagnitude > sideRayLength / 2) {
			frontGrounded = true;
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = midGrounded ? Color.blue : Color.red;
		Gizmos.DrawLine(midRay.origin, midRay.origin + (midRay.direction * midRayLength));

		Gizmos.color = backGrounded ? Color.blue : Color.red;
		Gizmos.DrawLine(backRay.origin, backRay.origin + (backRay.direction * sideRayLength));

		Gizmos.color = frontGrounded ? Color.blue : Color.red;
		Gizmos.DrawLine(frontRay.origin, frontRay.origin + (frontRay.direction * sideRayLength));
	}
#endif

}
