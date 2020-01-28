using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private int units = 0;

    [SerializeField]
    private Owner owner = Owner.NONE;

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

        if (Owner != Owner.NONE) {
            Units += 1;
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    public void AddUnits(int amount) {
        Units += amount;
    }

    public void RemoveUnits(int amount) {
        Units -= amount;

        if (Units < 0) {
            Units = 0;
        }
    }

    public void ChangeOwnership(Owner newOwner) {
        Owner = newOwner;

        if (Owner.Equals(Owner.PLAYER)) {
            gameObject.GetComponent<MeshRenderer>().material = playerMaterial;
        } else {
            gameObject.GetComponent<MeshRenderer>().material = enemyMaterial;
        }
    }
}
