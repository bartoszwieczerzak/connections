﻿using System;
using UnityEngine;

[Serializable]
public class Sound
{
    [SerializeField] private AudioClip _clip = default;
    [SerializeField] private SoundType _type = default;
    [SerializeField] private string _mixerGroup = "Master";
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

    public string MixerGroup
    {
        get => _mixerGroup;
        set => _mixerGroup = value;
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

    PlanetAcquired,
    ButtonHighlighted,
}