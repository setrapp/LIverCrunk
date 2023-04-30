using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Hud : MonoBehaviour {
	private static Hud instance = null;
	public static Hud Instance => instance;
	
	public Animator anim = null;
	public Image healthFill;
	public float minHealthPortion = 0.04f; // Going any further looks bad for spill;

	public Image spillFx;
	public float maxSpillPos;
	public float minSpillPos;

	private float cachedHealthPortion = 1f;

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

		healthFill.enabled = healthPortion > 0;

		spillFx.enabled = healthPortion > 0 && cachedHealthPortion - healthPortion > 0;
		cachedHealthPortion = healthPortion;
		Vector3 spillPos = spillFx.transform.localPosition;
		var adjustedHealthPortion = Mathf.Clamp01((healthPortion - minHealthPortion) * (1 / (1 - minHealthPortion)));
		spillPos.x = (minSpillPos * (1 - adjustedHealthPortion)) + (maxSpillPos * adjustedHealthPortion);
		spillFx.transform.localPosition = spillPos;

		hudLiver.ToggleVisibility(livers.Count > 0);

		//for (int i = 0; i < livers.Count && i < 
		if (livers.Count > 0 && livers[0] != null) {
			hudLiver.UpdateHealth(livers[0].HealthPortion);
		}
	}
}
