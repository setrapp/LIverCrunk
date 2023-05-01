using UnityEngine;

public class AutoWin : MonoBehaviour {
	void Start() {
#if UNITY_EDITOR
		LiveGlobals.Instance.data.goalLiverWorth = 0.5f;
#else
		Destroy(this);
#endif 
	}
}
