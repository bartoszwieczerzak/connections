using System;
using UnityEngine;

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

    [SerializeField] private Sound[] sounds;

    private void Start()
    {
        foreach (Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }

        Play(SoundType.THEME);
    }

    public void Play(SoundType type)
    {
        Sound s = Array.Find<Sound>(sounds, sound => sound.Type == type);
        if (s == null)
        {
            Debug.LogWarning("Sound " + type.ToString() + " not found!");
            return;
        }

        Debug.Log("Playing " + type.ToString() + " sound");
        s.Source.Play();
    }
}