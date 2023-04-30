using UnityEngine;

public class HarvestPoint : MonoBehaviour{
	public float harvestRadius = 10f;
	public float minHarvestHeight = 3f;
	public NpcDie die = null;

	void Update() {
		if (PlayerFeed.Instance != null && PlayerFeed.Instance.CanFeed) {
			var toFeed = PlayerFeed.Instance.transform.position - transform.position;
			if (toFeed.sqrMagnitude <= harvestRadius * harvestRadius && toFeed.y >= minHarvestHeight) {
				PlayerFeed.Instance.Feed(this);
			}
		}
	}

	public bool Harvest() {
		die.Die();
		return true;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, harvestRadius);
	}
}
