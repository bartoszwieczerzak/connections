using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnObjects : MonoBehaviour
{
    private Planet highlightedPlanet;
    private Planet sourcePlanet;
    private Planet targetPlanet;

    public Ship shipPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if(hit.transform.tag == "PLANET")
            {
                highlightedPlanet = hit.transform.GetComponent<Planet>();

                //Debug.Log("Highlighted planer " + highlightedPlanet.name);
            }
        }
        else
        {
            highlightedPlanet = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (highlightedPlanet)
            {
                if (!sourcePlanet)
                {
                    sourcePlanet = highlightedPlanet;

                    Debug.Log("Setting source to " + sourcePlanet.name);
                }
                else
                {
                    targetPlanet = highlightedPlanet;

                    Debug.Log("Setting target to " + targetPlanet.name);
                }

                if (sourcePlanet && targetPlanet)
                {
                    Debug.Log("Attack!");

                    if (sourcePlanet.Units>1) {
                        int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);

                        sourcePlanet.RemoveUnits(unitsToSend);
                        targetPlanet.AddUnits(unitsToSend);

                        Vector3 deploymentPosition = sourcePlanet.transform.position + new Vector3(1f, 1f, 0f);

                        Ship ship = Instantiate(shipPrefab, deploymentPosition, Quaternion.identity);
                        ship.crew = unitsToSend;
                        ship.source = sourcePlanet;
                        ship.target = targetPlanet;
                        ship.Attack();
                    }

                    sourcePlanet = null;
                    targetPlanet = null;
                }
            }
        }
    }
}
