using UnityEngine;
using UnityEngine.SceneManagement;

public class LiverDeliveryArea : MonoBehaviour {
	public LiveGlobals globalsPrefab = null;
	public Canvas mainMenu = null;
	public Canvas playHud = null;
	public GameObject fallingPlayer = null;
	public GameObject birthingPlayer = null;
	public Mother mother = null;

	private PlayerDisabler playerDisabler = null;

	void Start() {
		mainMenu.gameObject.SetActive(false);
		playHud.gameObject.SetActive(false);
		fallingPlayer.SetActive(false);
		birthingPlayer.SetActive(false);

		bool firstScene = false;
		if (globalsPrefab != null) {
			firstScene = globalsPrefab.InitIntoScene();
		}

		//playerDisabler = FindObjectOfType<PlayerDisabler>();

		if (firstScene) {
			mainMenu.gameObject.SetActive(true);
			mother.state = MotherState.StartGame;
			//birthingPlayer.SetActive(true);
			//if (playerDisabler != null ) { playerDisabler.ToggleDisable(true); }
		} else {
			playHud.gameObject.SetActive(true);
			var motherText = playHud.GetComponentInChildren<MotherText>();
			if (LiveGlobals.Instance.respawning) {
				//birthingPlayer.SetActive(true);
				LiveGlobals.Instance.respawning = false;
				mother.state = MotherState.Respawn;
				mother.BarfPlayer();
			} else {
				fallingPlayer.SetActive(true);
				mother.state = MotherState.Delivery;
				mother.DeliverLivers();
			}
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
