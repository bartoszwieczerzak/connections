using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private float _initialDelay = 3.0f;
    [SerializeField, Range(1.0f, 10.0f)] private float _minTurnDelay = 5.0f;
    [SerializeField, Range(1.0f, 25.0f)] private float _maxTurnDelay = 8.0f;
    
    [SerializeField] private int _minUnitsToSend = 50;
    [SerializeField] private int _maxUnitsToSend = 150;

    [SerializeField] private Owner _actor = Owner.Player2;
    private Owner[] _enemies;

    [Header("Logging")]
    [SerializeField] private bool enableLog = true;

    void Start()
    {
        List<Owner> enemies = Enum.GetValues(typeof(Owner)).Cast<Owner>().ToList();
        enemies.Remove(_actor);
        enemies.Remove(Owner.None);
        _enemies = enemies.ToArray();
        
        StartCoroutine("DelayedStart");
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(_initialDelay);

        StartCoroutine("EnemyTurn");
    }

    IEnumerator EnemyTurn()
    {
        List<Planet> allAiPlanets = Game.Instance.Planets.Where(planet => planet.Owner == _actor).ToList();
        List<Planet> actionablePlanets = allAiPlanets.Where(planet => !planet.IsCooldownActive).ToList();
        
        LogFormat("--------------------------------");
        LogFormat("New AI turn starts with total {0} planets and {1} actionable!", allAiPlanets.Count, actionablePlanets.Count);

        foreach (Planet selectedPlanet in actionablePlanets)
        {
            Planet closestPlayerPlanet = FindRandomPlanetInRange(selectedPlanet, _enemies);
            if (closestPlayerPlanet)
            {
                if (selectedPlanet.Units > _minUnitsToSend)
                {
                    int unitsToSend = (int) (closestPlayerPlanet.Units * 1.3f);
                    unitsToSend = Mathf.Clamp(unitsToSend, _minUnitsToSend, selectedPlanet.Units - 1);

                    selectedPlanet.SendShip(closestPlayerPlanet, unitsToSend);

                    LogFormat("- {0} attacks {1} with {2} units", selectedPlanet.name, closestPlayerPlanet.name, unitsToSend);

                    yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));

                    continue;
                }
                else
                {
                    LogFormat("- {0} has not enough ({2}) units to attack {1}", selectedPlanet.name, closestPlayerPlanet.name, selectedPlanet.Units);
                }
            }
            else
            {
                LogFormat("- {0} doesn't have neighbor to attack", selectedPlanet.name);
            }
            
            Planet closestUninhabitedPlanet = FindRandomPlanetInRange(selectedPlanet, Owner.None);
            if (closestUninhabitedPlanet)
            {
                if (selectedPlanet.Units > _minUnitsToSend)
                {
                    int unitsToSend = Random.Range(_minUnitsToSend, Mathf.Max(_minUnitsToSend, _maxUnitsToSend / 2));
                    unitsToSend = Mathf.Clamp(unitsToSend, _minUnitsToSend, selectedPlanet.Units);

                    selectedPlanet.SendShip(closestUninhabitedPlanet, unitsToSend);

                    LogFormat("- {0} takes over {1} with {2} units", selectedPlanet.name, closestUninhabitedPlanet.name, unitsToSend);

                    yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));

                    continue;
                }
                else
                {
                    LogFormat("- {0} has not enough ({2}) units to takeover {1}", selectedPlanet.name, closestUninhabitedPlanet.name, selectedPlanet.Units);
                }
            }
            else
            {
                LogFormat("- {0} doesn't have neighbor to takeover", selectedPlanet.name);
            }
            
            Planet closestOwnPlanet = FindRandomPlanetInRange(selectedPlanet, _actor);
            if (closestOwnPlanet)
            {
                if (selectedPlanet.Units > _minUnitsToSend && closestOwnPlanet.Units * 1.3f < selectedPlanet.Units)
                {
                    int unitsToSend = (selectedPlanet.Units - closestOwnPlanet.Units) / 3;
                    unitsToSend = Mathf.Clamp(unitsToSend, _minUnitsToSend, selectedPlanet.Units - _minUnitsToSend);

                    selectedPlanet.SendShip(closestOwnPlanet, unitsToSend);  

                    LogFormat("- {0} supports {1} with {2} units", selectedPlanet.name, closestOwnPlanet.name, unitsToSend);
                    
                    yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));
                
                    continue;
                }
                else
                {
                    LogFormat("- {0} has not enough ({2}) units to support {1}", selectedPlanet.name, closestOwnPlanet.name, selectedPlanet.Units);
                }
            }
            else
            {
                LogFormat("- {0} doesn't have neighbor to support", selectedPlanet.name);
            }
        }

        if (allAiPlanets.Count > 0)
        {
            yield return new WaitForSeconds(Random.Range(_minTurnDelay, _maxTurnDelay));
            StartCoroutine("EnemyTurn");
        }
        else
        {
            LogFormat("AI lost it's last planet! Ending AI fighting coroutine!");
        }
    }

    Planet SelectRandomPlanet(List<Planet> planets)
    {
        if (planets.Count <= 0) return null;

        int idx = Random.Range(0, planets.Count);
        return planets[idx];
    }

    Planet FindRandomPlanetInRange(Planet source, params Owner[] owners)
    {
        LayerMask mask = LayerMask.GetMask("Planets");

        float scanRange = source.PlanetRange - source.GetComponent<CircleCollider2D>().radius;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(source.transform.position, scanRange, mask);
        Planet[] exclude = new[] {source};
        Planet[] planetsInRange = colliders
            .Select(col => col.gameObject.GetComponent <Planet>())
            .Except(exclude)
            .Where(planet => owners.Any(owner => owner == planet.Owner))
            .ToArray();

        if (planetsInRange.Length == 0) return null;

        var distanceFunc = new Func<Planet, float>(target => (source.transform.position - target.transform.position).sqrMagnitude);
        // return planetsInRange.OrderBy(distanceFunc).First();
        return SelectRandomPlanet(planetsInRange.ToList());
    }
    
    
    private void LogFormat(string format, params object[] args)
    {
        if (enableLog)
        {
            Debug.LogFormat(format, args);
        }
    }
}