﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private float initialDelay = 4.0f;

    [SerializeField, Range(3.0f, 10.0f)]
    private float maxTurnDelay = 4.0f;

    private List<Planet> planets;

    private List<Planet> aiPlanets = new List<Planet>();
    private List<Planet> playerPlanets = new List<Planet>();
    private List<Planet> uninhabitedPlanets = new List<Planet>();

    void Start()
    {
        planets = GameObject.FindObjectsOfType<Planet>().ToList<Planet>();

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(initialDelay);

        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy takes turn");

        aiPlanets.Clear();
        playerPlanets.Clear();
        uninhabitedPlanets.Clear();

        foreach (Planet planet in planets)
        {
            switch (planet.Owner)
            {
                case Owner.Ai:
                    aiPlanets.Add(planet);
                    break;
                case Owner.Player:
                    playerPlanets.Add(planet);
                    break;
                case Owner.None:
                    uninhabitedPlanets.Add(planet);
                    break;
                default:
                    Debug.LogWarning("Found planet " + planet.name + " with unknown Owner: " + planet.Owner.ToString());
                    break;
            }
        }

        Planet aiPlanet = SelectRandomPlanet(aiPlanets);
        Planet closestUninhabitedPlanet = FindClosestPlanet(aiPlanet, uninhabitedPlanets);
        if (closestUninhabitedPlanet)
        {
            Debug.Log("Sending army from " + aiPlanet.name + " to " + closestUninhabitedPlanet.name);

            Game.instance.SendArmy(Owner.Ai, aiPlanet, closestUninhabitedPlanet);
        } else
        {
            Planet closestPlayerPlanet = FindClosestPlanet(aiPlanet, playerPlanets);
            if (closestPlayerPlanet)
            {
                Debug.Log("Sending army from " + aiPlanet.name + " to " + closestPlayerPlanet.name);

                Game.instance.SendArmy(Owner.Ai, aiPlanet, closestPlayerPlanet);
            }
        }

        Debug.Log("Current state [AI: " + aiPlanets.Count.ToString() + " | Player: " + playerPlanets.Count.ToString() + " | Free: " + uninhabitedPlanets.Count.ToString() + "]");

        yield return new WaitForSeconds(Random.Range(3.0f, maxTurnDelay));
        StartCoroutine(EnemyTurn());
    }

    Planet SelectRandomPlanet(List<Planet> planets)
    {
        int idx = Random.Range(0, planets.Count);
        return planets[idx];
    }

    Planet FindClosestPlanet(Planet source, List<Planet> planets)
    {
        if (planets.Count > 0)
        {
            return planets.OrderBy(x => Vector3.Distance(source.transform.position, x.transform.position)).ToArray()[0];
        }

        return null;
    }
}
