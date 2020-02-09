using TMPro;
using UnityEngine;

public class UpdateUnitsGui : MonoBehaviour
{
    private Planet _planet;
    private TMP_Text _unitsLabel;

    void Start()
    {
        _planet = gameObject.GetComponent<Planet>();
        _unitsLabel = gameObject.GetComponentInChildren<TMP_Text>();

        _unitsLabel.text = _planet.Units.ToString();
    }

    void Update()
    {
        _unitsLabel.text = _planet.Units.ToString();
    }
}