using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvents : MonoBehaviour
{

    public void Event_PlayWalkStep()
    {
        var pitch = Random.Range(0.7f, 1.3f);
        AudioManager.Instance.PlayWalkClip(pitch);
    }
    public void Event_PlayRunStep()
    {
        var pitch = Random.Range(0.35f, 0.65f);
        AudioManager.Instance.PlayRunClip(pitch);
    }

    public void Event_PlayRunHighPitchStep()
    {
        var pitch = Random.Range(1.2f, 1.5f);
        AudioManager.Instance.PlayRunClip(pitch);
    }

}
