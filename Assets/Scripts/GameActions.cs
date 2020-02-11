using UnityEngine;

public class GameActions : MonoBehaviour
{
    #region Singleton

    public static GameActions Instance;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Trying to create another instance of GameActions object!");
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion
    
    [SerializeField] private Ship shipPrefab = default;
    
    public void SendUnits(Owner who, Planet source, Planet target, int amount)
    {
        if (source.Units <= amount) return;

        Vector3 offset = target.transform.position - source.transform.position;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward,offset) * Quaternion.Euler(0, 0, 90);

        Ship ship = Instantiate(shipPrefab, source.transform.position, rotation);

        ship.Fly(who, source, target, amount);

        source.RemoveUnits(amount);
    }
    
    public void Disembark(Owner who, Planet target, int amount)
    {
        if (target.Owner == who || target.Owner == Owner.None)
        {
            target.AddUnits(amount);
            target.ChangeOwnership(who);
        }
        else
        {
            var unitsLeft = target.Units - amount;
        
            if (unitsLeft < 0)
            {
                target.RemoveUnits(target.Units);
                target.AddUnits(Mathf.Abs(unitsLeft));
                target.ChangeOwnership(who);
            }
            else if (unitsLeft == 0)
            {
                target.RemoveUnits(target.Units);
                target.ChangeOwnership(Owner.None);
            }
            else
            {
                target.RemoveUnits(amount);
            }
        }
    }

    /*public void SendUnits2(Owner who, Planet source, Planet target, int amount)
    {
        if (source.Units > amount)
        {
            if (who != source.Owner)
            {
                // Debug.LogWarning(source.Owner + " cannot send army from " + source.name + "!");
                return;
            }

            if (target.Owner == who || target.Owner == Owner.None)
            {
                // SendTroops method
                if (target.Owner == Owner.None)
                {
                    PlaySound(who, SoundType.PlanetAcquired);
                }

                // Debug.Log("SENDING " + unitsToSend + " units!");
                source.RemoveUnits(amount);
                target.AddUnits(amount);
                target.ChangeOwnership(who);
            }
            else
            {
                // Debug.Log("HAS: " + source.Units + " AND WILL REMOVE: " + unitsToSend);
                source.RemoveUnits(amount);

                // Debug.Log("LEFT: " + source.Units);

                if (target.Units > amount)
                {
                    target.RemoveUnits(amount);
                    // Debug.Log("REMOVED from TARGET: " + unitsToSend);

                    PlaySound(who, SoundType.BattleLost);
                }
                else if (target.Units < amount)
                {
                    int toBeAdded = amount - target.Units;
                    target.Units = 0;
                    target.AddUnits(toBeAdded);
                    // Debug.Log("ADDED to TARGET: " + toBeAdded);
                    target.ChangeOwnership(who);

                    PlaySound(who, SoundType.PlanetTakenOver);
                    PlaySound(who, SoundType.PlanetLost);
                }
                else
                {
                    target.Units = 0;
                    target.ChangeOwnership(Owner.None);

                    PlaySound(who, SoundType.BattleLost);
                }
            }

            source.SendFleet(target);

            PlaySound(who, SoundType.SendingArmyPlayer);
        }
    }*/

    private void PlaySound(Owner who, SoundType sound)
    {
        if (who == Owner.Ai)
        {
            AudioManager.Instance.Play(sound);
        }

        if (who == Owner.Player)
        {
            AudioManager.Instance.Play(sound);
        }
    }
}