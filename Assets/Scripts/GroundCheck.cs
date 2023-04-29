using UnityEngine;

public class GroundCheck : MonoBehaviour {
	public float midRayLength = 1.01f;
	public float sideRayLength = 1.01f;
	public float sideRayOffset = 1f;

	private Ray midRay = new Ray(Vector3.zero, Vector3.down);
	private Ray backRay = new Ray(Vector3.zero, Vector3.down);
	private Ray frontRay = new Ray(Vector3.zero, Vector3.down);

	private bool midHit = false;
	private bool backHit = false;
	private bool frontHit = false;

	public bool OnGround => midHit || backHit || frontHit;

	void Update() {
		midRay.origin = transform.position;
		backRay.origin = transform.position + (transform.right * -sideRayOffset);
		frontRay.origin = transform.position + (transform.right * sideRayOffset);

		var req_layer = LayerMask.GetMask("Platform");

		midHit = Physics.Raycast(midRay.origin, midRay.direction, midRayLength, req_layer);
		backHit = Physics.Raycast(backRay.origin, backRay.direction, sideRayLength, req_layer);
		frontHit = Physics.Raycast(frontRay.origin, frontRay.direction, sideRayLength, req_layer);
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = midHit ? Color.blue : Color.red;
		Gizmos.DrawLine(midRay.origin, midRay.origin + (midRay.direction * midRayLength));

		Gizmos.color = backHit ? Color.blue : Color.red;
		Gizmos.DrawLine(backRay.origin, backRay.origin + (backRay.direction * sideRayLength));

		Gizmos.color = frontHit ? Color.blue : Color.red;
		Gizmos.DrawLine(frontRay.origin, frontRay.origin + (frontRay.direction * sideRayLength));
	}
#endif

}
