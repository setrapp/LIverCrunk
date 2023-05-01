using UnityEngine;

[CreateAssetMenu(menuName = "Crunk/JumpData")]
public class JumpData : ScriptableObject {
	public bool useGlobalJumpArc = false;
	public AnimationCurve jumpArc = null;
	public AnimationCurve JumpArc => useGlobalJumpArc && LiveGlobals.Instance?.data != null ? LiveGlobals.Instance.data.defaultJumpArc : jumpArc;
	public float minHeight = 5f;
	public float maxHeight = 15f;
	public float maxJumpDuration = 1f;

	public (float, bool) GetHeight(float jumpDuration, float holdDuration) {
		var lastKeyframe = GetLastKeyframe();
		var finalTime = lastKeyframe.time;
		var finalHeight = lastKeyframe.value;

		if (Mathf.Abs(finalTime) < 0.001f || Mathf.Abs(finalHeight) < 0.001f) {
			return (0, true);
		}

		var holdPortion = 1f;
		if (maxJumpDuration > 0) {
			holdPortion = holdDuration / maxJumpDuration;
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

		var height = JumpArc.Evaluate(scaledJumpTime) * (heightScale / finalHeight);
		return (height, jumpDone);
	}

	public Keyframe GetLastKeyframe() {
		// TODO Do AnimationCurves sort keyframes by time?
		var lastKeyframe = 0;
		for (int i = 0; i < JumpArc.keys.Length; i++) {
			if (JumpArc.keys[i].time > JumpArc.keys[lastKeyframe].time) {
				lastKeyframe = i;
			}
		}

		return JumpArc.keys[lastKeyframe];
	}
}
