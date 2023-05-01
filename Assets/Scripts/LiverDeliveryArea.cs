using UnityEngine;
using UnityEngine.SceneManagement;

public class LiverDeliveryArea : MonoBehaviour {
	public LiveGlobals globalsPrefab = null;
	public Canvas mainMenu = null;
	public Canvas playHud = null;

	private PlayerDisabler playerDisabler = null;

	void Start() {
		mainMenu.gameObject.SetActive(false);
		playHud.gameObject.SetActive(false);

		bool firstScene = false;
		if (globalsPrefab != null) {
			firstScene = globalsPrefab.InitIntoScene();
		}

		playerDisabler = FindObjectOfType<PlayerDisabler>();

		if (firstScene) {
			mainMenu.gameObject.SetActive(true);
			if (playerDisabler != null ) { playerDisabler.ToggleDisable(true); }
		} else {
			playHud.gameObject.SetActive(true);
		}
	}

	public void StartGame() {
		GoCollect();
	}

	public void GoCollect() {
		SceneManager.LoadScene(LiveGlobals.CollectionScene);
	}

	public void Quit() {
		Application.Quit();
	}
}
