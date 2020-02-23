using System;
using UnityEngine;
using UnityEngine.Audio;

public class UserSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(GetMusicVolume()));
        audioMixer.SetFloat("SfxVolume", LinearToDecibel(GetSfxVolume()));
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 0.75f);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(volume));
    }

    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat("SfxVolume", 1f);
    }

    public void SetSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat("SfxVolume", volume);
        audioMixer.SetFloat("SfxVolume", LinearToDecibel(volume));
    }

    private float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0.0f)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }
}