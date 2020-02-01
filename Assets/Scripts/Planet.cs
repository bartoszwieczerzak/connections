using System.Collections;
using TMPro;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private int units = 0;
    [SerializeField] private Owner owner = Owner.None;
    [SerializeField] private PlanetStats planetStats;

    public int Units
    {
        get => units;
        set => units = value;
    }

    public Owner Owner => owner;

    void Start()
    {
        transform.localScale = new Vector2(planetStats.size, planetStats.size);

        StartCoroutine(AddTroopsCoroutine());
    }

    private IEnumerator AddTroopsCoroutine()
    {
        yield return new WaitForSeconds(planetStats.populationCycleTime);

        if (Owner == Owner.None) yield break;
        units += planetStats.populationGrowth;
        StartCoroutine(AddTroopsCoroutine());
    }

    public void AddUnits(int amount)
    {
        units += amount;
    }

    public void RemoveUnits(int amount)
    {
        units -= amount;
        units = Mathf.Clamp(units, 0, int.MaxValue);
    }

    public void ChangeOwnership(Owner newOwner)
    {
        owner = newOwner;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().color =
            owner.Equals(Owner.Player) ? Game.instance.PlayerColor : Game.instance.EnemyColor;
    }

    public void SendFleet(Planet targetPlanet)
    {
        Vector3 sourcePlanetPosition = transform.position;
        Vector3 heading = sourcePlanetPosition - targetPlanet.transform.position;

        ParticleSystem transfer = GetComponentInChildren<ParticleSystem>();
        transfer.transform.rotation = Quaternion.LookRotation(-heading);
        transfer.Play();
    }
}