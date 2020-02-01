using TMPro;
using UnityEngine;

public class UpdateUnitsGui : MonoBehaviour
{
    private Planet _planet;
    private TMP_Text _label;

    void Start()
    {
        _planet = gameObject.GetComponent<Planet>();
        _label = gameObject.GetComponentInChildren<TMP_Text>();

        _label.text = _planet.Units.ToString();
    }

    void Update()
    {
        _label.text = _planet.Units.ToString();
    }
}