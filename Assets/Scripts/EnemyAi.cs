using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private float _initialDelay = 3.0f;
    [SerializeField, Range(3.0f, 10.0f)] private float _minTurnDelay = 5.0f;
    [SerializeField, Range(3.0f, 25.0f)] private float _maxTurnDelay = 8.0f;

    private List<Planet> _planets;
    private readonly List<Planet> _aiPlanets = new List<Planet>();
    private readonly List<Planet> _playerPlanets = new List<Planet>();
    private readonly List<Planet> _uninhabitedPlanets = new List<Planet>();

    void Start()
    {
        _planets = FindObjectsOfType<Planet>().ToList();

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(_initialDelay);

        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        // Debug.Log("Enemy takes turn");

        _aiPlanets.Clear();
        _playerPlanets.Clear();
        _uninhabitedPlanets.Clear();

        foreach (Planet planet in _planets)
        {
            switch (planet.Owner)
            {
                case Owner.Ai:
                    _aiPlanets.Add(planet);
                    break;
                case Owner.Player:
                    _playerPlanets.Add(planet);
                    break;
                case Owner.None:
                    _uninhabitedPlanets.Add(planet);
                    break;
                default:
                    // Debug.LogWarning("Found planet " + planet.name + " with unknown Owner: " + planet.Owner.ToString());
                    break;
            }
        }

        Planet aiPlanet = SelectRandomPlanet(_aiPlanets);
        if (aiPlanet)
        {
            Planet closestUninhabitedPlanet = FindClosestPlanet(aiPlanet, _uninhabitedPlanets);
            if (closestUninhabitedPlanet)
            {
                // Debug.Log("Sending army from " + aiPlanet.name + " to " + closestUninhabitedPlanet.name);

                // GameActions.Instance.SendUnits(Owner.Ai, aiPlanet, closestUninhabitedPlanet);
            }
            else
            {
                Planet closestPlayerPlanet = FindClosestPlanet(aiPlanet, _playerPlanets);
                if (closestPlayerPlanet)
                {
                    // Debug.Log("Sending army from " + aiPlanet.name + " to " + closestPlayerPlanet.name);

                    // GameActions.Instance.SendUnits(Owner.Ai, aiPlanet, closestPlayerPlanet);
                }
            }

            // Debug.Log("Current state [AI: " + _aiPlanets.Count.ToString() + " | Player: " + _playerPlanets.Count.ToString() + " | Free: " + _uninhabitedPlanets.Count.ToString() + "]");

            yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));
            StartCoroutine(EnemyTurn());
        }
        else
        {
            // Debug.Log("AI lost it's last planet! Ending AI fighting coroutine!");
        }
    }

    Planet SelectRandomPlanet(List<Planet> planets)
    {
        if (planets.Count > 0)
        {
            int idx = Random.Range(0, planets.Count);
            return planets[idx];
        }

        return null;
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