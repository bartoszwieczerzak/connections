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

    public int Units { get => units; set => units = value; }
    public Owner Owner { get => owner; set => owner = value; }

    void Start() {
        //StartCoroutine(AddTroopsCoroutine());
    }

    IEnumerator AddTroopsCoroutine() {
        yield return new WaitForSeconds(3);

        if (Owner != Owner.None) {
            Units += 1;
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

        gameObject.GetComponent<MeshRenderer>().material = Owner.Equals(Owner.Player) ? playerMaterial : enemyMaterial;
    }

    public void SendFleet(Planet targetPlanet) {
        Vector3 sourcePlanetPosition = transform.position;
        Vector3 heading = sourcePlanetPosition - targetPlanet.transform.position;

        ParticleSystem transfer = GetComponentInChildren<ParticleSystem>();
        transfer.transform.rotation = Quaternion.LookRotation(-heading);
        transfer.Play();
    }
}
