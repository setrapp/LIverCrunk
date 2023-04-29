using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/JumpData")]
public class JumpData : ScriptableObject {
	public AnimationCurve jumpArc = null;
	public float minHeight = 5f;
	public float maxHeight = 15f;
	public float maxJumpDuration = 1f;
	public float maxHoldDuration = 0.5f;

	public (float, bool) GetHeight(float jumpDuration, float holdDuration) {
		// TODO Do AnimationCurves sort keyframes by time?
		var last_keyframe = 0;
		for (int i = 0; i < jumpArc.keys.Length; i++) {
			if (jumpArc.keys[i].time > jumpArc.keys[last_keyframe].time) {
				last_keyframe = i;
			}
		}

		var finalTime = jumpArc.keys[last_keyframe].time;
		var finalHeight = jumpArc.keys[last_keyframe].value;

		if (Mathf.Abs(finalTime) < 0.001f || Mathf.Abs(finalHeight) < 0.001f) {
			return (0, true);
		}

		var holdPortion = 1f;
		if (maxHoldDuration > 0) {
			holdPortion = holdDuration / maxHoldDuration;
		}

		var jumpPortion = 1f;
		if (maxJumpDuration > 0) {
			jumpPortion = jumpDuration / maxJumpDuration;
		}

		var heightScale = (minHeight * (1 - holdPortion)) + (maxHeight * holdPortion);
		var scaledJumpTime = jumpPortion / finalTime;
		bool jumpDone = false;
		if (scaledJumpTime >= finalTime) {
			scaledJumpTime = finalTime;
			jumpDone = true;
		}

		var height = jumpArc.Evaluate(scaledJumpTime) * (heightScale / finalHeight);
		return (height, jumpDone);
	}
}
