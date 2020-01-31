using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private int units = 0;

    [SerializeField]
    private Owner owner = Owner.None;

    [SerializeField]
    private Material playerMaterial;

    [SerializeField]
    private Material enemyMaterial;

    [SerializeField]
    private PlanetStats stats;

    public int Units { get => units; set => units = value; }
    public Owner Owner { get => owner; set => owner = value; }

    void Start() {
        transform.localScale = new Vector3(stats.size, stats.size, stats.size);

        StartCoroutine(AddTroopsCoroutine());
    }

    IEnumerator AddTroopsCoroutine() {
        yield return new WaitForSeconds(stats.populationCycleTime);

        if (Owner != Owner.None) {
            Units += stats.populationGrowth;
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    public void AddUnits(int amount) {
        units += amount;
    }

    public void RemoveUnits(int amount) {
        units -= amount;

        if (units < 0) {
            units = 0;
        }
    }

    public void ChangeOwnership(Owner newOwner) {
        owner = newOwner;
// here we need to somehow mark player and non-player planet
        //gameObject.GetComponent<MeshRenderer>().material = Owner.Equals(Owner.Player) ? playerMaterial : enemyMaterial;
    }

    public void SendFleet(Planet targetPlanet) {
        Vector3 sourcePlanetPosition = transform.position;
        Vector3 heading = sourcePlanetPosition - targetPlanet.transform.position;

        ParticleSystem transfer = GetComponentInChildren<ParticleSystem>();
        transfer.transform.rotation = Quaternion.LookRotation(-heading);
        transfer.Play();
    }
}
