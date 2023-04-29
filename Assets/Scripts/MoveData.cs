using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/MoveData")]
public class MoveData : ScriptableObject {
	[SerializeField] private float minSpeed = 2f;
	public float MinSpeed => minSpeed;
	[SerializeField] private float maxSpeed = 10f;
	public float MaxSpeed => maxSpeed;
	[SerializeField] private float accelerationTime = 0f;
	[SerializeField] private float dampening = 0f;

	public float GetSpeed(float moveTime) {
		if (accelerationTime < 0.001f) {
			return maxSpeed;
		}
		else {
			var accPortion = Mathf.Clamp01(moveTime / accelerationTime);
			var speed = (minSpeed * (1 - accPortion)) + (maxSpeed * accPortion);
			return speed;
		}
	}
	
	public float ApplyDampening(float current) {
		var abs = Mathf.Abs(current);
		abs -= Mathf.Max((abs * dampening) * Time.deltaTime, 0);
		current = abs * (current < 0 ? -1 : 1);
		return current;
	}
}
