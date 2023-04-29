using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/MoveData")]
public class MoveData : ScriptableObject {
	[SerializeField] private float maxSpeed = 10;
	public float MaxSpeed => maxSpeed;
	[SerializeField] private float dampening = 0;

	public float GetSpeed() {
		return maxSpeed;
	}
	
	public float ApplyDampening(float current) {
		var abs = Mathf.Abs(current);
		abs -= Mathf.Max((abs * dampening) * Time.deltaTime, 0);
		current = abs * (current < 0 ? -1 : 1);
		return current;
	}
}
