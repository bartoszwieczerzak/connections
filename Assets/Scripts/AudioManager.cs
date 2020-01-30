using UnityEngine;

public class AudioManager : MonoBehaviour {
    #region Singleton

    public static AudioManager instance;

    private void Awake() {
        if (instance) {
            Debug.LogWarning("Trying to create another instance of AudioManager!");
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion

    public AudioClip[] sendingArmyClips;


    public void PlaySendingArmyClip() {
        AudioSource source = GetComponent<AudioSource>();
        source.PlayOneShot(sendingArmyClips[Random.Range(0, sendingArmyClips.Length)]);
    }
}