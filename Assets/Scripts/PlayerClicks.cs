using UnityEngine;

public class PlayerClicks : MonoBehaviour
{
    private Planet highlightedPlanet;
    private Planet sourcePlanet;
    private Planet targetPlanet;

    [SerializeField]
    private GameObject shipPrefab;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if(hit.transform.tag == Tag.PLANET) {
                highlightedPlanet = hit.transform.GetComponent<Planet>();
            }
        }
        else {
            highlightedPlanet = null;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (highlightedPlanet) {
                if (!sourcePlanet) {
                    sourcePlanet = highlightedPlanet;

                    Debug.Log("Setting source to " + sourcePlanet.name);
                } else {
                    targetPlanet = highlightedPlanet;

                    Debug.Log("Setting target to " + targetPlanet.name);
                }

                if (sourcePlanet && targetPlanet) {                    
                    if (targetPlanet.Owner == Owner.AI) {
                        Debug.Log("Attack!");

                        SendShip(sourcePlanet);

                        if (sourcePlanet.Units > 1) {
                            int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);
                            sourcePlanet.RemoveUnits(unitsToSend);

                            if (targetPlanet.Units > unitsToSend) {
                                targetPlanet.RemoveUnits(unitsToSend);
                            } else if (targetPlanet.Units < unitsToSend) {
                                targetPlanet.AddUnits(targetPlanet.Units - unitsToSend);
                                targetPlanet.ChangeOwnership(Owner.PLAYER);
                            } else {
                                targetPlanet.Units = 0;
                                targetPlanet.ChangeOwnership(Owner.NONE);
                            }
                        }
                    } else {
                        Debug.Log("Send more troops!");
                        if (sourcePlanet.Units > 1) {
                            int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);
                            sourcePlanet.RemoveUnits(unitsToSend);
                            targetPlanet.AddUnits(unitsToSend);
                            targetPlanet.ChangeOwnership(Owner.PLAYER);
                        }
                    }

                    sourcePlanet = null;
                    targetPlanet = null;
                }
            }
        }
    }

    private void SendShip(Planet sourcePlanet) {

        if (sourcePlanet.Units > 1) {
            int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);

            sourcePlanet.RemoveUnits(unitsToSend);
            targetPlanet.AddUnits(unitsToSend);

            Vector3 deploymentPosition = sourcePlanet.transform.position + new Vector3(1f, 1f, 0f);

            GameObject shipGo = Instantiate(shipPrefab, deploymentPosition, Quaternion.identity);
            Ship ship = shipGo.GetComponent<Ship>();
            ship.crew = unitsToSend;
            ship.source = sourcePlanet;
            ship.target = targetPlanet;
            ship.Attack();
        }
    }
}
