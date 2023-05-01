using UnityEngine;

public class SlimeDropper : MonoBehaviour {
	public ParticleSystem slime = null;
	public float startDelay = 0;
	public float duration = 1;

	void Awake()
	{
		slime.startDelay = startDelay;
		var slimeMain = slime.main;
		slimeMain.duration = duration;
	}
}
