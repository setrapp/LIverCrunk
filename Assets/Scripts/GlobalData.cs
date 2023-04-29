using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/GlobalData")]
public class GlobalData : ScriptableObject {
	private static GlobalData instance = null;
	public static GlobalData Instance => instance;

	public AnimationCurve defaultJumpArc;

	void OnEnable() {
		if (instance != null && instance != this) {
			Debug.Log("Multiple GlobalData exist... there can only be one", this);
		}

		instance = this;
	}
}
