using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [SerializeField]
    private AudioClip clip;

    [SerializeField]
    private SoundType type;

    [SerializeField, Range(0.0f, 1.0f)]
    private float volume = 1.0f;

    [SerializeField, Range(0.1f, 3.0f)]
    private float pitch = 1.0f;

    [SerializeField]
    private bool loop = false;

    private AudioSource source;

    public AudioClip Clip { get => clip; set => clip = value; }
    public SoundType Type { get => type; set => type = value; }
    public float Volume { get => volume; set => volume = value; }
    public float Pitch { get => pitch; set => pitch = value; }
    public bool Loop { get => loop; set => loop = value; }
    public AudioSource Source { get => source; set => source = value; }
}

[System.Serializable]
public enum SoundType
{
    THEME,

    CLICK_CONFIRMED,
    CLICK_REJECTED,

    SENDING_ARMY_PLAYER,
    SENDING_ARMY_ENEMY,

    BATTLE_LOST,

    PLANET_LOST,
    PLANET_TAKENOVER,
    PLANET_SELECTED,

    GAME_WON,
    GAME_LOST,
}
