using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJumpStart : MonoBehaviour {
	public Vector3 jumpStart = new Vector3(0, 100, 0);

	void Start()
	{
		GetComponent<Rigidbody>().velocity = jumpStart;
	}
}
