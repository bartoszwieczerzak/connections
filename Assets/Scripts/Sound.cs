using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound 
{
    public AudioClip clip;

    public SoundType type;

    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;

    [Range(0.1f, 3.0f)]
    public float pitch = 1.0f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public enum SoundType
{
    THEME,

    CLICK_CONFIRMED,
    CLICK_REJECTED,

    SENDING_ARMY_PLAYER,
    SENDING_ARMY_ENEMY,

    PLANET_LOST,
    PLANET_TAKENOVER,
    PLANET_SELECTED,

    GAME_WIN,
    GAME_LOST,
}
