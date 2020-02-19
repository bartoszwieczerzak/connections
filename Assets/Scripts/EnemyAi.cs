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

    void Start()
    {
        StartCoroutine("DelayedStart");
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(_initialDelay);

        StartCoroutine("EnemyTurn");
    }

    IEnumerator EnemyTurn()
    {
        List<Planet> allAiPlanets = Game.Instance.Planets.Where(planet => planet.OwnByAi).ToList();
        List<Planet> actionablePlanets = allAiPlanets.Where(planet => !planet.IsCooldownActive).ToList();
        
        Debug.Log("--------------------------------");
        Debug.LogFormat("New AI turn starts with total {0} planets and {1} actionable!", allAiPlanets.Count, actionablePlanets.Count);

        foreach (Planet selectedPlanet in actionablePlanets)
        {
            Planet closestUninhabitedPlanet = FindRandomPlanetInRange(selectedPlanet, Owner.None);
            if (closestUninhabitedPlanet)
            {
                var unitsToSend = Mathf.Clamp(selectedPlanet.Units / 2, 1, 150);
                if (unitsToSend >= 30)
                {
                    selectedPlanet.SendShip(closestUninhabitedPlanet, unitsToSend);

                    Debug.LogFormat("- {0} takes over {1} with {2} units", selectedPlanet.name, closestUninhabitedPlanet.name, unitsToSend);

                    yield return new WaitForSeconds(1.5f);

                    continue;
                }
                else
                {
                    Debug.LogFormat("- {0} has not enough ({2}) units to takeover {1}", selectedPlanet.name, closestUninhabitedPlanet.name, unitsToSend);
                }
            }
            else
            {
                Debug.LogFormat("- {0} doesn't have neighbor to takeover", selectedPlanet.name);
            }
            
            Planet closestOwnPlanet = FindRandomPlanetInRange(selectedPlanet, Owner.Ai);
            if (closestOwnPlanet)
            {
                if (closestOwnPlanet.Units * 1.3f < selectedPlanet.Units)
                {
                    var unitsToSend = Mathf.Clamp((selectedPlanet.Units - closestOwnPlanet.Units) / 5, 1, selectedPlanet.Units - 30);
                    if (unitsToSend >= 30)
                    {
                        selectedPlanet.SendShip(closestOwnPlanet, unitsToSend);  

                        Debug.LogFormat("- {0} supports {1} with {2} units", selectedPlanet.name, closestOwnPlanet.name, unitsToSend);
                        
                        yield return new WaitForSeconds(1.5f);
                    
                        continue;
                    }
                    else
                    {
                        Debug.LogFormat("- {0} has not enough ({2}) units to support {1}", selectedPlanet.name, closestOwnPlanet.name, unitsToSend);
                    }
                }
            }
            else
            {
                Debug.LogFormat("- {0} doesn't have neighbor to support", selectedPlanet.name);
            }
            
            Planet closestPlayerPlanet = FindRandomPlanetInRange(selectedPlanet, Owner.Player);
            if (closestPlayerPlanet)
            {
                var unitsToSend = Mathf.Clamp((int) (closestPlayerPlanet.Units * 1.3f), 1, selectedPlanet.Units - 1);
                if (unitsToSend >= 30)
                {
                    selectedPlanet.SendShip(closestPlayerPlanet, unitsToSend);

                    Debug.LogFormat("- {0} attacks {1} with {2} units", selectedPlanet.name, closestPlayerPlanet.name, unitsToSend);

                    yield return new WaitForSeconds(1.5f);

                    continue;
                }
                else
                {
                    Debug.LogFormat("- {0} has not enough ({2}) units to attack {1}", selectedPlanet.name, closestPlayerPlanet.name, unitsToSend);
                }
            }
            else
            {
                Debug.LogFormat("- {0} doesn't have neighbor to attack", selectedPlanet.name);
            }
        }

        if (allAiPlanets.Count > 0)
        {
            yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));
            StartCoroutine("EnemyTurn");
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

    Planet FindRandomPlanetInRange(Planet source, Owner owner)
    {
        LayerMask mask = LayerMask.GetMask("Planets");
        int planetColliderRadious = 1;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(source.transform.position, source.PlanetRange - planetColliderRadious, mask);
        Planet[] exclude = new[] {source};
        Planet[] planetsInRange = colliders
            .Select(col => col.gameObject.GetComponent <Planet>())
            .Except(exclude)
            .Where(planet => planet.Owner == owner)
            .ToArray();

        if (planetsInRange.Length == 0) return null;

        var distanceFunc = new Func<Planet, float>(target => (source.transform.position - target.transform.position).sqrMagnitude);
        // return planetsInRange.OrderBy(distanceFunc).First();
        return SelectRandomPlanet(planetsInRange.ToList());
    }
}