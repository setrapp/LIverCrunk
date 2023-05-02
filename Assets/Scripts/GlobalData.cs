using UnityEngine;
using UnityEngine.Events;
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
	public List<string> pickupFirstLiverLines;

	public void MarkPlayerToJump() {
		var jumpers = FindObjectsOfType<PlayerJumpStart>();
		if (jumpers != null) {
			foreach (var jumper in jumpers) {
				if (jumper != null && jumper.gameObject.activeSelf) {
					jumper.Jump(0.7f);
				}
			}
		}
	}

	public void MarkMotherToFill() {
		var mother = FindObjectOfType<Mother>();
		if (mother != null) {
			mother.AttemptVictory();
		}
	}

	public void AddLiverSlot()
	{
		LiveGlobals.Instance.maxHeldLivers++;

		LiveGlobals.Instance.damageRate *= 0.75f;
	}
}

[System.Serializable]
public class MotherDialog {
	public string name;
	public bool startGame;
	public bool respawn;
	public bool emptyHanded;
	public bool outside;
	public bool victory;
	public int minLivers;
	public int maxLivers;
	public List<string> lines;
	public UnityEvent OnComplete;

}
