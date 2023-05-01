using UnityEngine;

public class AutoWin : MonoBehaviour {
	void Start() {
#if UNITY_EDITOR
		LiveGlobals.Instance.autoWin = true;
#else
		Destroy(this);
#endif 
	}
}
