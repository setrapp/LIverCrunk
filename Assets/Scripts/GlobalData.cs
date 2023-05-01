using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Crunk/GlobalData")]
public class GlobalData : ScriptableObject {
	public float goalLiverWorth = -1;
	public int humanReplenishThreshold = 3;
	public AnimationCurve defaultJumpArc;
	public string deliveryScene = "LiverDeliveryArea";
	public string victoryScene = "";
	public string collectionScene = "Playmat";
	public List<MotherDialog> dialogs;
}

[System.Serializable]
public class MotherDialog {
	public string name;
	public bool keepAfterUse;
	public bool startGame;
	public bool respawn;
	public bool emptyHanded;
	public int minLivers;
	public int maxLivers;
	public List<string> lines;

}
