using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/GlobalData")]
public class GlobalData : ScriptableObject {
	public float goalLiverWorth = 5;
	public AnimationCurve defaultJumpArc;
	public string deliveryScene = "LiverDeliveryArea";
	public string victoryScene = "";
	public string collectionScene = "Playmat";
}
