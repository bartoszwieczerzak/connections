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
        musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 0.75f));
        sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SfxVolume", 1f));
    }
}