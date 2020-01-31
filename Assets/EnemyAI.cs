using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    private Planet[] planets;

    private List<Planet> aiPlanets = new List<Planet>();
    private List<Planet> playerPlanets = new List<Planet>();
    private List<Planet> freePlanets = new List<Planet>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy takes turn");

        Planet[] planets = GameObject.FindObjectsOfType<Planet>();

        aiPlanets.Clear();
        playerPlanets.Clear();
        freePlanets.Clear();

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
                    freePlanets.Add(planet);
                    break;
                default:
                    Debug.LogWarning("Found planet " + planet.name + " with unknown Owner: " + planet.Owner.ToString());
                    break;
            }
        }

        foreach(Planet aiPlanet in aiPlanets)
        {
            Planet closest = FindClosestPlanet(aiPlanet, freePlanets);
            if (closest)
            {
                Debug.Log("Sending army from " + aiPlanet.name + " to " + closest.name);
            }
        }

        Debug.Log("Current state [AI: " + aiPlanets.Count.ToString() + " | Player: " + playerPlanets.Count.ToString() + " | Free: " + freePlanets.Count.ToString() + "]");

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(EnemyTurn());
    }

    private static float CompareListByName(Planet i1, Planet i2)
    {
        return Vector3.Dot(i1.transform.position, i2.transform.position);
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
