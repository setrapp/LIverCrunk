using UnityEngine;

public class HarvestPoint : MonoBehaviour{
	public float harvestRadius = 10f;
	public float minHarvestHeight = 3f;
	public NpcDie die = null;
	public Liver liverPrefab = null;

	void Update() {
		if (PlayerFeed.Instance != null && PlayerFeed.Instance.CanFeed) {
			var toFeed = PlayerFeed.Instance.transform.position - transform.position;
			if (toFeed.sqrMagnitude <= harvestRadius * harvestRadius && toFeed.y >= minHarvestHeight) {
				PlayerFeed.Instance.Feed(this);
			}
		}
	}

	public Liver Harvest() {
		die.Die();
		if (liverPrefab != null) {
			return Instantiate(liverPrefab, transform.position, Quaternion.identity);
		}
		return null;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, harvestRadius);
	}
}
