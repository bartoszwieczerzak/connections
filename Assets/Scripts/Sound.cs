using System;
using UnityEngine;

[Serializable]
public class Sound
{
    [SerializeField] private AudioClip _clip;
    [SerializeField] private SoundType _type;
    [SerializeField, Range(0.0f, 1.0f)] private float _volume = 1.0f;
    [SerializeField, Range(0.1f, 3.0f)] private float _pitch = 1.0f;
    [SerializeField] private bool _loop = false;

    private AudioSource _source;

    public AudioClip Clip
    {
        get => _clip;
        set => _clip = value;
    }

    public SoundType Type
    {
        get => _type;
        set => _type = value;
    }

    public float Volume
    {
        get => _volume;
        set => _volume = value;
    }

    public float Pitch
    {
        get => _pitch;
        set => _pitch = value;
    }

    public bool Loop
    {
        get => _loop;
        set => _loop = value;
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