using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance;

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

    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private Sound[] _sounds = null;

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
            else
            {
                Debug.LogWarningFormat("No matching Mixer Group found for name: {0}", s.MixerGroup);
            }
        }

        Play(SoundType.Theme);
    }

    public void Play(SoundType type)
    {
        Sound s = Array.Find(_sounds, sound => sound.Type == type);
        if (s == null)
        {
            Debug.LogWarningFormat("Sound {0} not found!", type.ToString());
            return;
        }

        Debug.LogFormat("Playing {0} sound", type.ToString());
        s.Source.Play();
    }
}