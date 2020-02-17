using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private AudioManager _audioManager;

    void Start()
    {
        float musicVolume = 0f;
        audioMixer.GetFloat("MusicVolume", out musicVolume);
        musicSlider.value = musicVolume;

        float sfxVolume = 0f;
        audioMixer.GetFloat("SfxVolume", out sfxVolume);
        sfxSlider.value = sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SfxVolume", volume);
    }
}