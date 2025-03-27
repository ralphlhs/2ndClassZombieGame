using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    [System.Serializable]

    public struct NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    public NamedAudioClip[] bgmClipsList;
    public NamedAudioClip[] sfxClipsList;

    private Coroutine currentBGMCoroutine;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeAudioClips();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void InitializeAudioClips()
    {
        foreach (var bgm in bgmClipsList)
        {
            if (!bgmClips.ContainsKey(bgm.name))
            {
                bgmClips.Add(bgm.name, bgm.clip);
            }
        }
        foreach (var sfx in sfxClipsList)
        {
            if (!sfxClips.ContainsKey(sfx.name))
            {
                sfxClips.Add(sfx.name, sfx.clip);
            }
        }
    }

    public void PlayBGM(string name, float fadeDuration = 1.0f)
    {
        if (bgmClips.ContainsKey(name))
        {
            if (currentBGMCoroutine != null)
            {
                StopCoroutine(currentBGMCoroutine);
            }
            currentBGMCoroutine = StartCoroutine(FadeOutBGM(fadeDuration, () =>
            {
                bgmSource.spatialBlend = 0f;
                bgmSource.clip = bgmClips[name];
                bgmSource.Play();
                currentBGMCoroutine = StartCoroutine(FadeInBGM(fadeDuration));
            }));
        }
    }

    public void PlaySfx(string name, Vector3 position, float spatialBlend_2d3d)
    {
        if (sfxClips.ContainsKey(name))
        {
            sfxSource.spatialBlend = spatialBlend_2d3d;
            //sfxSource.PlayOneShot(sfxClips[name]);
            AudioSource.PlayClipAtPoint(sfxClips[name], position); //Ư����ġ���� ���� ���
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    private IEnumerator FadeOutBGM(float duration, Action onFadeComplete)
    {
        float startVolume = bgmSource.volume;
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, i / duration);
            yield return null;
        }
        bgmSource.volume = 0;
        onFadeComplete?.Invoke();
    }

    private IEnumerator FadeInBGM(float duration)
    {
        float startVolume = 0f;
        bgmSource.volume = 0;
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0.1f, i / duration);
            yield return null;
        }
        bgmSource.volume = 0.1f;
    }
}
