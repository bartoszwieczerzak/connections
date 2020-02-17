using System;
using UnityEngine;
using UnityEngine.Audio;

public class UserSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        audioMixer.SetFloat("MusicVolume", GetMusicVolume());
        audioMixer.SetFloat("SfxVolume", GetSfxVolume());
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", -10f);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat("SfxVolume", 0f);
    }

    public void SetSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }
}