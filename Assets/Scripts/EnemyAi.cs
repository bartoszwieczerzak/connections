using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private float _initialDelay = 3.0f;
    [SerializeField, Range(3.0f, 10.0f)] private float _minTurnDelay = 5.0f;
    [SerializeField, Range(3.0f, 25.0f)] private float _maxTurnDelay = 8.0f;

    private readonly List<Planet> _aiPlanets = new List<Planet>();
    private readonly List<Planet> _playerPlanets = new List<Planet>();
    private readonly List<Planet> _uninhabitedPlanets = new List<Planet>();

    void Start()
    {
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

        foreach (Planet planet in Game.Instance.Planets)
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
                    Debug.LogWarning("Found planet " + planet.name + " with unknown Owner: " + planet.Owner);
                    break;
            }
        }

        Planet aiPlanet = SelectRandomPlanet(_aiPlanets);
        if (aiPlanet)
        {
            Planet closestUninhabitedPlanet = FindClosestPlanet(aiPlanet, _uninhabitedPlanets);
            var unitsToSend = aiPlanet.Units / 2;
            if (closestUninhabitedPlanet)
            {
                aiPlanet.SendShip(closestUninhabitedPlanet, unitsToSend);
            }
            else
            {
                Planet closestPlayerPlanet = FindClosestPlanet(aiPlanet, _playerPlanets);
                if (closestPlayerPlanet)
                {
                    aiPlanet.SendShip(closestPlayerPlanet, unitsToSend);
                }
            }

            yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));
            StartCoroutine(EnemyTurn());
        }
        else
        {
            Debug.Log("AI lost it's last planet! Ending AI fighting coroutine!");
        }
    }

    Planet SelectRandomPlanet(List<Planet> planets)
    {
        if (planets.Count <= 0) return null;

        int idx = Random.Range(0, planets.Count);
        return planets[idx];
    }

    Planet FindClosestPlanet(Planet source, List<Planet> planets)
    {
        if (planets.Count <= 0) return null;

        var distanceFunc = new Func<Planet, float>(p => Vector3.Distance(source.transform.position, p.transform.position));
        return planets.OrderBy(distanceFunc).First();
    }
}