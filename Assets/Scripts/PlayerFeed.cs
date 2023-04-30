using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GroundCheck), typeof(Rigidbody))]
public class PlayerFeed : MonoBehaviour {
	private static PlayerFeed instance = null;
	public static PlayerFeed Instance => instance;

	private GroundCheck groundCheck;
	private Rigidbody body;

	public float snapTime = 0.25f;
	public float feedCooldown = 0.5f;
	private float lastFeedTime = 0;

	private bool feeding = false;
	public bool IsFeeding => feeding;

	public int maxLivers = 1;
	public List<Liver> EatenLivers = new List<Liver>();

	private HarvestPoint harvestTarget = null;

	public bool CanFeed => !IsFeeding && !groundCheck.OnGround && EatenLivers.Count < maxLivers && (Time.time - lastFeedTime) > feedCooldown;

	void Start() {
		groundCheck = GetComponent<GroundCheck>();
		body = GetComponent<Rigidbody>();

		if (instance != null && instance != this) {
			Debug.LogError("Multiple PlayerFeed in the scene... There can only be one");
		}
		instance = this;
	}

	public void Feed(HarvestPoint harvest)
	{
		feeding = true;
		body.isKinematic = true;
		StartCoroutine(feed(harvest));
	}

	private IEnumerator feed(HarvestPoint harvest) {
		harvestTarget = harvest;
		var startPos = transform.position;
		var elapsed = 0f;

		if (snapTime > 0) {
			while (elapsed < snapTime) {
				var portion = elapsed / snapTime;
				transform.position = (startPos * (1 - portion)) + (harvest.transform.position * portion);
				yield return null;
				elapsed += Time.deltaTime;
			}
		}

		transform.position = harvest.transform.position;
	}

	public void TakeHarvest() {
		if (harvestTarget != null) {
			var liver = harvestTarget.Harvest();
			if (liver != null) {
				liver.AwardPlayer(this);
				EatenLivers.Add(liver);
			}
		}
	}

	public void EndFeed() {
		feeding = false;
		lastFeedTime = Time.time;
		body.isKinematic = false;
		harvestTarget = null;
	}

	public string GetAnimParam() {
		if (feeding) {
			return "Harvest";
		}

		return "Idle";
	}
}
