using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    #region Singleton

    public static AudioManager instance;

    private void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("Trying to create another instance of AudioManager!");
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion

    [SerializeField]
    private Sound[] sounds;

    private void Start()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        Play(SoundType.THEME);
    }

    public void Play(SoundType type)
    {
        Sound s = Array.Find<Sound>(sounds, sound => sound.type == type);
        if (s == null)
        {
            Debug.LogWarning("Sound " + type.ToString() + " not found!");
            return;
        }

        s.source.Play();
    }


    public void PlaySendingArmyClip()
    {
        //AudioSource source = GetComponent<AudioSource>();
        //source.PlayOneShot(sendingArmyClips[Random.Range(0, sendingArmyClips.Length)]);
    }
}
