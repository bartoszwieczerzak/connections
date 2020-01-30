﻿using UnityEngine;

public class PlayerClicks : MonoBehaviour
{
    private Planet highlightedPlanet;
    private Planet sourcePlanet;
    private Planet targetPlanet;
    private Planet lastTargetPlanet;

    [SerializeField]
    private GameObject hoverPlanetHighlight;

    [SerializeField]
    private GameObject selectedPlanetHighlight;

    [SerializeField]
    private GameObject shipPrefab;
    [SerializeField]
    private ParticleSystem particleSystemPrefab;

    private ParticleSystem.Particle[] particles;

    private void Start() {
        particles = new ParticleSystem.Particle[particleSystemPrefab.main.maxParticles];
    }
    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if(hit.transform.tag == Tag.PLANET) {
                highlightedPlanet = hit.transform.GetComponent<Planet>();
                hoverPlanetHighlight.transform.position = highlightedPlanet.transform.position;
                hoverPlanetHighlight.SetActive(true);
            }
        }
        else {
            highlightedPlanet = null;
            hoverPlanetHighlight.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && CanSelectAsSourcePlanet(highlightedPlanet))
        {
            sourcePlanet = highlightedPlanet;
            selectedPlanetHighlight.transform.position = sourcePlanet.transform.position;
            selectedPlanetHighlight.SetActive(true);
        }

        if (sourcePlanet && !targetPlanet)
        {
            LineRenderer moveMarker = sourcePlanet.GetComponentInChildren<LineRenderer>();
            moveMarker.SetPosition(1, -(sourcePlanet.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (CanSelectAsTargetPlanet(highlightedPlanet))
            {
                targetPlanet = highlightedPlanet;
            } else if (sourcePlanet) {
                LineRenderer moveMarker = sourcePlanet.GetComponentInChildren<LineRenderer>();
                moveMarker.SetPosition(1, Vector3.zero);

                selectedPlanetHighlight.SetActive(false);

                sourcePlanet = null;
            }
        }

        if (sourcePlanet && targetPlanet)
        {
            if (sourcePlanet.Units > 1)
            {
                if (targetPlanet.Owner == Owner.AI)
                {
                    AttackEnemy();
                }
                else
                {
                    SendTroops();
                }

                VisualiseArmyMovement(sourcePlanet, targetPlanet);
            }

            LineRenderer moveMarker = sourcePlanet.GetComponentInChildren<LineRenderer>();
            moveMarker.SetPosition(1, Vector3.zero);

            selectedPlanetHighlight.SetActive(false);

            sourcePlanet = null;
            lastTargetPlanet = targetPlanet;
            targetPlanet = null;
        }
    }

    private bool CanSelectAsSourcePlanet(Planet highlightedPlanet)
    {
        return sourcePlanet == null && highlightedPlanet && highlightedPlanet.Owner == Owner.PLAYER;
    }

    private bool CanSelectAsTargetPlanet(Planet highlightedPlanet)
    {
        return sourcePlanet != null && highlightedPlanet && highlightedPlanet.GetInstanceID() != sourcePlanet.GetInstanceID();
    }

    void XLateUpdate() {
        if (sourcePlanet == null || lastTargetPlanet == null) {
            particleSystemPrefab.gameObject.SetActive(false);
            return;
        }
        Debug.Log("SOURCE: " + sourcePlanet.name + " TARGET: " + lastTargetPlanet.name);
        particleSystemPrefab.gameObject.SetActive(true);

        particleSystemPrefab.transform.position = sourcePlanet.transform.position;

        int length = particleSystemPrefab.GetParticles(particles);
        int i = 0;

        transform.LookAt(lastTargetPlanet.transform);

        while (i < length) {

            //Target is a Transform object
            Vector3 direction = lastTargetPlanet.transform.position - particles[i].position;
            direction.Normalize();

            float variableSpeed = (particleSystemPrefab.startSpeed / (particles[i].remainingLifetime + 0.1f)) + particles[i].startLifetime;
            particles[i].position += direction * variableSpeed * Time.deltaTime;

            if (Vector3.Distance(lastTargetPlanet.transform.position, particles[i].position) < 1.0f) {
                particles[i].remainingLifetime = -0.1f; //Kill the particle
            }

            i++;

        }

        particleSystemPrefab.SetParticles(particles, length);
    }

    private void AttackEnemy() {
        // SendShip(sourcePlanet, targetPlanet);

        int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);
        Debug.Log("HAS: " + sourcePlanet.Units + " AND WILL REMOVE: " + unitsToSend);
        sourcePlanet.RemoveUnits(unitsToSend);

        Debug.Log("LEFT: " + sourcePlanet.Units);

        if (targetPlanet.Units > unitsToSend) {
            targetPlanet.RemoveUnits(unitsToSend);
            Debug.Log("REMOVED from TARGET: " + unitsToSend);
        } else if (targetPlanet.Units < unitsToSend) {
            int toBeAdded = unitsToSend - targetPlanet.Units;
            targetPlanet.Units = 0;
            targetPlanet.AddUnits(toBeAdded);
            Debug.Log("ADDED to TARGET: " + toBeAdded);
            targetPlanet.ChangeOwnership(Owner.PLAYER);
        } else {
            targetPlanet.Units = 0;
            targetPlanet.ChangeOwnership(Owner.NONE);
        }
    }

    private void SendTroops() {
        int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);
        sourcePlanet.RemoveUnits(unitsToSend);
        targetPlanet.AddUnits(unitsToSend);
        targetPlanet.ChangeOwnership(Owner.PLAYER);
    }

    private void VisualiseArmyMovement(Planet sourcePlanet, Planet targetPlanet)
    {
        sourcePlanet.SendFleet(targetPlanet);

        AudioManager.instance.PlaySendingArmyClip();
    }
}