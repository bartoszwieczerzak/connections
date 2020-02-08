using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public void OnButtonHighlighted()
    {
        AudioManager.Instance.Play(SoundType.ButtonHighlighted);
    }

    public void OnButtonClicked()
    {
        AudioManager.Instance.Play(SoundType.ClickConfirmed);
    }
}
