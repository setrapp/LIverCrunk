using UnityEngine;

public class HarvestPoint : MonoBehaviour{
	public int harvestId = -1;
	public GameObject vessel;
	public float harvestRadius = 10f;
	public float minHarvestHeight = 3f;
	public NpcDie die = null;
	public Liver liverPrefab = null;

	void Awake() {
		if (harvestId < 0) { harvestId = 1000 + vessel.transform.GetSiblingIndex(); }
		if (LiveGlobals.Instance != null) {
			if (LiveGlobals.Instance.harvestedLiverIds.Contains(harvestId)) {
				Destroy(vessel);
			} else {
				LiveGlobals.Instance.RegisterLiverVessel(this);
			}
		}
	}

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
			var newLiver = Instantiate(liverPrefab, transform.position, Quaternion.identity);
			LiveGlobals.Instance.HarvestLiver(harvestId, newLiver);
			return newLiver;
		}
		return null;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, harvestRadius);
	}
}
