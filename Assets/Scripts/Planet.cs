using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private int units = 0;

    [SerializeField]
    private Owner owner = Owner.NONE;

    public int Units { get => units; set => units = value; }

    void Update()
    {
        
    }

    public void AddUnits(int amount) {
        Units += amount;
    }

    public void RemoveUnits(int amount) {
        Units -= amount;

        if (amount < 0) amount = 0;
    }

    public void ChangeOwnership(Owner newOwner) {
        owner = newOwner;
    }
}
