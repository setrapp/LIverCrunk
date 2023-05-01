using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloorOnly : MonoBehaviour {
	
	void Update() {
		var player = LiveGlobals.Instance?.player?.body;
		if (player != null) {
			GetComponent<Collider>().enabled = player.velocity.y <= 0;
		}
	}
}
