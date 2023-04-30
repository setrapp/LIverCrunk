using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public GlobalData globals = null;
	private Vector3 startPos = Vector3.zero;

	private bool godMode = false;
	public float healthSeconds = 100;
	public int maxLivers = 1;
	public List<Liver> EatenLivers = new List<Liver>();
	public Transform liverSack = null;

	public bool LiverFull => EatenLivers.Count >= maxLivers;

	void Awake() {
		if (globals != null) {
			globals.Init();
		}

		startPos = transform.position;
	}

	void Reset(bool hard) {
		transform.position = startPos;
		if (hard) { }
	}

	public void StashLiver(Liver liver) {
		liver.gameObject.SetActive(false);
		liver.transform.SetParent(liverSack);
		EatenLivers.Add(liver);

		HUD.Instance.AddLiver(liver);
	}

	public void ApplyDamage(float damage) {
		if (!godMode) {
			if (damage < healthSeconds) {
				healthSeconds -= damage;
			} else {
				
			}
		}
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown("r")) {
			Reset(false);
		}
#endif
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (!Application.isPlaying && GlobalData.Instance != globals) {
			globals.Init();
		}
	}
#endif
}
