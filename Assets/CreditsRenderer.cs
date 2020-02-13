using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsRenderer : MonoBehaviour
{
    [SerializeField] private string _credits;
    [SerializeField] private TextMeshProUGUI body;

    private void Start()
    {
        StartCoroutine(TypeSentence(_credits));
    }

    IEnumerator TypeSentence (string sentence)
    {
        body.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            body.text += letter;
            yield return null;
        }
    }
}
