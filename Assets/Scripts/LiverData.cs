using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/LiverData")]
public class LiverData : ScriptableObject {

	[System.Serializable]
	public enum Quality {
		Dry = 0,
		Jaundice,
		Diseased,
		Healthy,
	}

	public Quality quality = Quality.Healthy;
	public float maxHealth = 100;
}
