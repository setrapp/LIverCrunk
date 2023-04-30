using UnityEngine;

public class Liver : MonoBehaviour {
	public LiverData data;
	public Animator anim;
	private PlayerFeed eater;

	private float health = 0;

	public float HealthPortion => health / data.maxHealthSeconds;

	void Start() {
		if (anim == null) {
			anim = GetComponent<Animator>();
		}

		health = data.maxHealthSeconds;
	}

	public void AwardPlayer(PlayerFeed eater) {
		this.eater = eater;
		// TODO remove this when we have a proper animation
		transform.SetParent(eater.transform);
		//transform.localPosition = new Vector3(0, 4, 0);
	}

	public void Stash() {
		if (eater != null) {
			eater.Player.StashLiver(this);
		}
	}

	public float SoakDamage(float damage) {
		if (damage <= health) {
			health -= damage;
			return 0;
		} else {
			damage -= health;
			health = 0;
			return damage;
		}
	}
}
