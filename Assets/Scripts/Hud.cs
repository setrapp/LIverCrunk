using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Hud : MonoBehaviour {
	private static Hud instance = null;
	public static Hud Instance => instance;
	
	public Animator anim = null;
	public Image healthFill;
	public Image liverFill;
	public float minHealthPortion = 0.04f; // Going any further looks bad for spill;

	public Image spillFx;
	public Image liverSpillFx;
	public float maxSpillPos;
	public float minSpillPos;

	private float cachedHealthPortion = 1f;
	private float cachedLiverPortion = 1f;
	private int cachedLiverCount = 0;

	//public List<GameObject> HudLivers = null;
	public HudLiver hudLiver = null;
	
	void Start() {
		if (instance != null && instance != this) {
			Debug.LogError("Multiple Hud in the scene... There can only be one");
		}
		instance = this;
	}

	public void AddLiver(Liver liver) {
		anim.SetTrigger("LiverGain");
	}

	public void UpdateHealth(float healthPortion, List<Liver> livers) {
		healthPortion = Mathf.Clamp01(healthPortion);
		healthFill.fillAmount = Mathf.Max(healthPortion, minHealthPortion);

		var liverPortion = 0f;
		if (livers.Count > 0) {
			for (int i = 0; i < livers.Count; i++) {
				liverPortion += livers[i].HealthPortion;
			}
			liverPortion = Mathf.Clamp01(liverPortion / livers.Count);
		}

		liverFill.fillAmount = Mathf.Max(liverPortion, minHealthPortion);

		healthFill.enabled = healthPortion > 0;
		liverFill.enabled = liverPortion > 0;

		spillFx.enabled = healthPortion > 0 && cachedHealthPortion - healthPortion > 0;
		cachedHealthPortion = healthPortion;
		Vector3 spillPos = spillFx.transform.localPosition;
		spillPos.x = computeSpillPos(healthPortion);
		spillFx.transform.localPosition = spillPos;

		liverSpillFx.enabled = liverPortion > 0 && cachedLiverPortion - liverPortion > 0 && (cachedLiverCount == livers.Count);
		cachedLiverPortion = liverPortion;
		cachedLiverCount = livers.Count;
		Vector3 liverSpillPos = liverSpillFx.transform.localPosition;
		liverSpillPos.x = computeSpillPos(liverPortion);
		liverSpillFx.transform.localPosition = liverSpillPos;

		hudLiver.ToggleVisibility(livers.Count > 0);

		//for (int i = 0; i < livers.Count && i < 
		if (livers.Count > 0 && livers[0] != null) {
			hudLiver.UpdateHealth(livers[0].HealthPortion);
		}
	}

	float computeSpillPos(float portion) {
		var adjustedHealthPortion = Mathf.Clamp01((portion - minHealthPortion) * (1 / (1 - minHealthPortion)));
		return (minSpillPos * (1 - adjustedHealthPortion)) + (maxSpillPos * adjustedHealthPortion);
	}
}
