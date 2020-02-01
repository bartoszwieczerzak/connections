using System;
using UnityEngine;

[Serializable]
public class Sound
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private SoundType type;
    [SerializeField, Range(0.0f, 1.0f)] private float volume = 1.0f;
    [SerializeField, Range(0.1f, 3.0f)] private float pitch = 1.0f;
    [SerializeField] private bool loop = false;

    private AudioSource _source;

    public AudioClip Clip
    {
        get => clip;
        set => clip = value;
    }

    public SoundType Type
    {
        get => type;
        set => type = value;
    }

    public float Volume
    {
        get => volume;
        set => volume = value;
    }

    public float Pitch
    {
        get => pitch;
        set => pitch = value;
    }

    public bool Loop
    {
        get => loop;
        set => loop = value;
    }

    public AudioSource Source
    {
        get => _source;
        set => _source = value;
    }
}

[Serializable]
public enum SoundType
{
    Theme,

    ClickConfirmed,
    ClickRejected,

    SendingArmyPlayer,
    SendingArmyEnemy,

    BattleLost,

    PlanetLost,
    PlanetTakenOver,
    PlanetSelected,

    GameWon,
    GameLost,
}