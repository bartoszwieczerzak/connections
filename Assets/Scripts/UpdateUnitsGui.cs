using UnityEngine;
using TMPro;

public class UpdateUnitsGui : MonoBehaviour
{
    private Planet planet;
    private TMP_Text label;

    void Start()
    {
        planet = gameObject.GetComponent<Planet>();
        label = gameObject.GetComponentInChildren<TMP_Text>();

        label.text = planet.Units.ToString();
    }

    void Update()
    {
        label.text = planet.Units.ToString();
    }
}
