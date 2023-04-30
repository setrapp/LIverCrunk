using UnityEngine;

public class Liver : MonoBehaviour {
	public LiverData data;
	public Animator anim;
	private PlayerFeed eater;

	void Start() {
		if (anim == null) {
			anim = GetComponent<Animator>();
		}
	}

	public void AwardPlayer(PlayerFeed eater) {
		this.eater = eater;
		// TODO remove this when we have a proper animation
		//transform.SetParent(player.transform);
		//transform.localPosition = new Vector3(0, 4, 0);
	}

	public void Stash() {
		if (eater != null) {
			eater.Player.StashLiver(this);
		}
	}

}
