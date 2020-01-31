using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    private List<Planet> planets;

    private List<Planet> aiPlanets = new List<Planet>();
    private List<Planet> playerPlanets = new List<Planet>();
    private List<Planet> freePlanets = new List<Planet>();

    // Start is called before the first frame update
    void Start()
    {
        planets = GameObject.FindObjectsOfType<Planet>().ToList<Planet>();

        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy takes turn");

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

        yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        StartCoroutine(EnemyTurn());
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
