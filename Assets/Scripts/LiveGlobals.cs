using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LiveGlobals : MonoBehaviour {
	private static LiveGlobals instance = null;
	public static LiveGlobals Instance => instance;
	public GlobalData data = null;

	public bool autoWin = false;
	public float GoalLiverWorth => autoWin ? 0.5f : data?.goalLiverWorth > 0 ? data.goalLiverWorth : ((float)vesselIds.Count) - 0.5f;

	public List<int> vesselIds = null;
	public List<int> harvestedLiverIds = null;
	public int maxHeldLivers = 1;
	public List<Liver> heldLivers = null;
	public List<Liver> givenLivers = null;
	public int priorLiversGiven = 0;
	public bool respawning = false;
	public Liver worstLiverHeld = null;
	public bool sufficientLivers = false;

	public float damageRate = 1f;
	public Player player = null;

	public static string DeliveryScene => Instance?.data?.deliveryScene;
	public static string VictoryScene => Instance?.data?.victoryScene;
	private string overrideCollectionScene = null;
	public static string CollectionScene {
		get {
#if UNITY_EDITOR
			if (Instance?.overrideCollectionScene != null) {
				return Instance.overrideCollectionScene;
			}
#endif
			return Instance?.data?.collectionScene;
		}
	}

	public bool InitIntoScene() {
		if (instance != null && instance != this) {
			return false;
		}

		instance = Instantiate(gameObject).GetComponent<LiveGlobals>();
		Object.DontDestroyOnLoad(instance);

#if UNITY_EDITOR
			if (Application.isPlaying) {
				var sceneName = SceneManager.GetActiveScene().name;
				if (sceneName != data?.deliveryScene && sceneName != data?.collectionScene && sceneName != data?.victoryScene) {
					overrideCollectionScene = sceneName;
					Debug.Log($"[Editor Only] Using {sceneName} scene for Liver Collection");
				}
			}
#endif
			return true;
	}

	public bool InitWithoutScene() {
		instance = this;
		return true;
	}

	public void RegisterLiverVessel(HarvestPoint vessel) {
		if (vessel == null) { return; }
		if (!vesselIds.Contains(vessel.harvestId)) {
			vesselIds.Add(vessel.harvestId);
		} else {
			Debug.LogError($"Attempting to register a second Liver Vessel with ID {vessel.harvestId}", vessel.vessel);
		}
	}

	public void HarvestLiver(int id, Liver liver) {
		if (liver != null) {
			heldLivers.Add(liver);
			if (worstLiverHeld == null || liver.Worth < worstLiverHeld.Worth) {
				worstLiverHeld = liver;
			}

			if (givenLivers.Count < 1) {
				var motherText = FindObjectOfType<MotherText>();
				if (motherText != null) {
					motherText.AddLines(data.pickupFirstLiverLines);
				}
			}
		}
	}

	public MotherDialog GetMotherDialog(MotherState state) {
		MotherDialog bestDialog = null;
		var earlyOut = false;
		foreach (var dialog in data.dialogs) {
			if (earlyOut) { break; }
			if (dialog.outside) { continue; }
			switch (state) {
				case MotherState.StartGame:
					if (dialog.startGame) { bestDialog = dialog; }
					break;
				case MotherState.Respawn:
					if (dialog.respawn
							&& dialog.minLivers <= priorLiversGiven && dialog.maxLivers >= priorLiversGiven
							&& (bestDialog == null || (dialog.minLivers > bestDialog.minLivers || dialog.maxLivers < bestDialog.maxLivers)))
					{
						bestDialog = dialog;
					}
					break;
				case MotherState.Delivery:
					if (dialog.victory && sufficientLivers) {
						earlyOut = true;
						bestDialog = dialog;
					}
					else if (dialog.emptyHanded && heldLivers.Count < 1) {
						earlyOut = true;
						bestDialog = dialog;
					}
					else if (!dialog.startGame && !dialog.respawn && !dialog.emptyHanded && !dialog.victory
							&& dialog.minLivers <= givenLivers.Count && dialog.maxLivers >= givenLivers.Count
							&& (bestDialog == null || (dialog.minLivers > bestDialog.minLivers || dialog.maxLivers < bestDialog.maxLivers)))
					{
						//todo check quality
						bestDialog = dialog;
					}
					break;
			}
		}

		return bestDialog;
	}

	public void AttemptReplenishHumans() {
		if (harvestedLiverIds.Count >= data.humanReplenishThreshold) {
			// TODO only matters if we care about quality
		}
	}

	public void Respawn() {
		//harvestedLiverIds.Clear();
		heldLivers.Clear();
		//givenLivers.Clear();
		priorLiversGiven = 0;
		respawning = true;
	}

	void OnDestroy() {
		if (instance == this) {
			instance = null;
		}
	}

}
