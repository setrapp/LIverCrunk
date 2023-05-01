using UnityEngine;
using UnityEngine.SceneManagement;

public class LiveGlobals : MonoBehaviour {
	private static LiveGlobals instance = null;
	public static LiveGlobals Instance => instance;
	public GlobalData data = null;

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

}
