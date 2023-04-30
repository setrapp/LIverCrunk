using UnityEngine;

public class Liver : MonoBehaviour {
	public LiverData data;
	public Animator anim;

	void Start() {
		if (anim == null) {
			anim = GetComponent<Animator>();
		}
	}

	public void AwardPlayer(PlayerFeed player) {
		if (anim != null) {
			anim.SetTrigger("AwardPlayer");
		}

		// TODO remove this when we have a proper animation
		transform.SetParent(player.transform);
		transform.localPosition = new Vector3(0, 4, 0);
	}

}
