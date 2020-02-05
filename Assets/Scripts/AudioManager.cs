using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance;

    [SerializeField]
    private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Trying to create another instance of AudioManager!");
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion

    [SerializeField] private Sound[] _sounds;

    private void Start()
    {
        foreach (Sound s in _sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.loop = s.Loop;

            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(s.MixerGroup);
            if (groups.Length > 0)
            {
                s.Source.outputAudioMixerGroup = groups[0];
            }
        }

        Play(SoundType.Theme);
    }

    public void Play(SoundType type)
    {
        Sound s = Array.Find(_sounds, sound => sound.Type == type);
        if (s == null)
        {
            Debug.LogWarning("Sound " + type.ToString() + " not found!");
            return;
        }

        Debug.Log("Playing " + type.ToString() + " sound");
        s.Source.Play();
    }
}