using TMPro;
using UnityEngine;

public class GatheredUnitsGuiUpdate : MonoBehaviour
{
    private TMP_Text _gatheredUnitsLabel;

    void Start()
    {
        _gatheredUnitsLabel = gameObject.GetComponent<TMP_Text>();

        _gatheredUnitsLabel.text = PlayerInputs.Instance.UnitsGathered.ToString();
    }

    void Update()
    {
        _gatheredUnitsLabel.text = PlayerInputs.Instance.UnitsGathered.ToString();
    }
}