using UnityEngine;

[RequireComponent(typeof(GroundCheck), typeof(PlayerJump))]
public class PlayerFeed : MonoBehaviour {
	private GroundCheck groundCheck;
	private PlayerJump playerJump;

	private bool feeding = false;
	public bool IsFeeding => feeding;

	void Start() {
		groundCheck = GetComponent<GroundCheck>();
		playerJump = GetComponent<PlayerJump>();
	}

	void Update()
	{
		feeding = false;
		if (Input.GetKey("f")) {
			feeding = true;
			playerJump.EndJump();
		}
	}

	public string GetAnimParam() {
		if (feeding) {
			return "Harvest";
		}

		return "Idle";
	}
}
