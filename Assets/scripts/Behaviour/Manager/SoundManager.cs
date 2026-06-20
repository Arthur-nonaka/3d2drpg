using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource settingsSource;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Range(0f, 1f)]
    public float settingsVolume = 1f;

    private Dictionary<string, AudioClip> sfxCache = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyVolumes();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip)
            return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float spatialBlend = 1f)
    {
        if (clip == null)
            return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }

    public void PlaySettings(AudioClip clip)
    {
        if (clip == null)
            return;
        settingsSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void SetSettingsVolume(float volume)
    {
        settingsVolume = Mathf.Clamp01(volume);
        settingsSource.volume = settingsVolume;
        PlayerPrefs.SetFloat("SettingsVolume", settingsVolume);
    }

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        settingsVolume = PlayerPrefs.GetFloat("SettingsVolume", 0.5f);
    }

    private void ApplyVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        settingsSource.volume = settingsVolume;
    }
}
