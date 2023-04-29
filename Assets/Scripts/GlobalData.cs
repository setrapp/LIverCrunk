using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/GlobalData")]
public class GlobalData : ScriptableObject {
	private static GlobalData instance = null;
	public static GlobalData Instance => instance;

	public AnimationCurve defaultJumpArc;

	public void Init() {
		instance = this;
	}
}
