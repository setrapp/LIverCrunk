using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource soundEffectSource;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource swallowSource;
    [SerializeField] private AudioSource crackleSource;
    float volVelocity = 0.0f;
    float smoothTime = 1.5f;


    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip swallowClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    IEnumerator Start()
    {
        var maxAudioTime = backgroundSource.clip.length;
        backgroundSource.time = Random.Range(0, maxAudioTime - 1);

        backgroundSource.volume = 0;

        var timer = 0.0f;
        while (timer < 0.7f)
        {
            backgroundSource.volume = timer / 0.7f;
            timer += Time.deltaTime;
            yield return null;
        }

        backgroundSource.volume = 1;
    }

    void Update()
    {
        if (Hud.Instance != null) {
            if (Hud.Instance.liverFill.fillAmount > 0.1f)
            {
                crackleSource.volume = Mathf.SmoothDamp(crackleSource.volume, Mathf.InverseLerp(0.75f, 0.0f, Hud.Instance.liverFill.fillAmount), ref volVelocity, smoothTime);

            }
            else
            {
                crackleSource.volume = Mathf.SmoothDamp(crackleSource.volume, Mathf.InverseLerp(0.75f, 0.0f, Hud.Instance.healthFill.fillAmount), ref volVelocity, smoothTime);
            }
        }
    }

    public void PlaySwallowClip()
    {
        //soundEffectSource.pitch = Random.Range(1.45f, 1.7f);
        swallowSource.PlayOneShot(swallowClip);
    }

    public void PlayWalkClip(float pitch)
    {
        soundEffectSource.pitch = pitch;
        soundEffectSource.PlayOneShot(walkClip);
    }
    public void PlayRunClip(float pitch)
    {
        soundEffectSource.pitch = pitch;
        soundEffectSource.PlayOneShot(runClip);
    }

    public void PlayJumpClip()
    {
        if (playingJumpingClip)
            return;

        StartCoroutine(playingJumpClip());
    }
    public void PlayLandClip()
    {
        if (playingLandingClip)
            return;

        StartCoroutine(playingLandClip());
    }
    private bool playingJumpingClip;
    IEnumerator playingJumpClip()
    {
        playingJumpingClip = true;

        jumpSource.pitch = Random.Range(1.6f, 1.8f);
        jumpSource.PlayOneShot(jumpClip);

        var timer = 0.0f;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        playingJumpingClip = false;
    }


    private bool playingLandingClip;
    IEnumerator playingLandClip()
    {
        playingLandingClip = true;

        jumpSource.pitch = Random.Range(1.45f, 1.7f);
        jumpSource.PlayOneShot(landClip);

        var timer = 0.0f;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        playingLandingClip = false;
    }
}
