using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LiveGlobals : MonoBehaviour {
	private static LiveGlobals instance = null;
	public static LiveGlobals Instance => instance;
	public GlobalData data = null;

	public List<int> harvestedLiverIds = null;
	public List<Liver> heldLivers = null;
	public List<Liver> givenLivers = null;
	public bool respawning = false;

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

	public void HarvestLiver(int id) {
		harvestedLiverIds.Add(id);
	}

	public void Respawn() {
		//harvestedLiverIds.Clear();
		heldLivers.Clear();
		//givenLivers.Clear();
		respawning = true;
	}

}
