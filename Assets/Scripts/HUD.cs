using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	private static HUD instance = null;
	public static HUD Instance => instance;
	
	public Animator anim = null;
	public Image healthFill;

	//public List<GameObject> hudLivers = null;
	
	void Start() {
		if (instance != null && instance != this) {
			Debug.LogError("Multiple HUD in the scene... There can only be one");
		}
		instance = this;
	}

	public void AddLiver(Liver liver) {
		anim.SetTrigger("LiverGain");
	}

	public void DepleteHealth(float portion) {
		healthFill.fillAmount -= portion;
	}
}
