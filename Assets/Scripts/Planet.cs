using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private int units = 0;

    [SerializeField]
    private Owner owner = Owner.NONE;

    void Update()
    {
        
    }

    public void AddUnits(int amount) {
        units += amount;
    }

    public void RemoveUnits(int amount) {
        units -= amount;

        if (amount < 0) amount = 0;
    }

    public void ChangeOwnership(Owner newOwner) {
        owner = newOwner;
    }
}
