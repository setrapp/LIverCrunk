using UnityEngine;
using UnityEngine.Events;

public class NpcDie : MonoBehaviour {
	public UnityEvent OnDie;
	public void Die() {
		OnDie.Invoke();
		Destroy(gameObject);
	}
}
