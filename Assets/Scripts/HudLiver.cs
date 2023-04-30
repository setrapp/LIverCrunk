using UnityEngine;
using UnityEngine.UI;

public class HudLiver : MonoBehaviour {
	public Image goodHealth;
	public Image fairHealth;
	public Image roughHealth;
	public Image failedHealth;

	public Image spillFx;

	private float cachedHealthPortion = 1;

	public void UpdateHealth(float healthPortion) {
		var goodHealthColor = goodHealth.color;
		var fairHealthColor = fairHealth.color;
		var roughHealthColor = roughHealth.color;
		var failedHealthColor = failedHealth.color;

		goodHealthColor.a = fairHealthColor.a = roughHealthColor.a = failedHealthColor.a = 0f;

		if (healthPortion > 0.66f) {
			goodHealthColor.a = (healthPortion - 0.66f) / 0.33f;
			fairHealthColor.a = 1 - goodHealthColor.a;
		}
		else if (healthPortion <= 0.66f && healthPortion > 0.33f) {
			fairHealthColor.a = (healthPortion - 0.33f) / 0.33f;
			roughHealthColor.a = 1 - fairHealthColor.a;
		}
		else if (healthPortion <= 0.33f) {
			roughHealthColor.a = healthPortion / 0.33f;
			failedHealthColor.a = 1 - roughHealthColor.a;
		}

		goodHealth.color = goodHealthColor;
		fairHealth.color = fairHealthColor;
		roughHealth.color = roughHealthColor;
		failedHealth.color = failedHealthColor;

		spillFx.enabled = cachedHealthPortion - healthPortion > 0.001f;
	}

	public void ToggleVisibility(bool visible) {
		goodHealth.gameObject.SetActive(visible);
		fairHealth.gameObject.SetActive(visible);
		roughHealth.gameObject.SetActive(visible);
		failedHealth.gameObject.SetActive(visible);
	}
}
