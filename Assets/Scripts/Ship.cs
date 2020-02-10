using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Owner who;
    private Planet source;
    private Planet target;
    private int amount;
    private bool fly = false;
    private bool arrived = false;

    public float speed = 1.0f;

    public void Fly(Owner who, Planet source, Planet target, int amount)
    {
        transform.position = source.transform.position;

        this.who = who;
        this.source = source;
        this.target = target;
        this.amount = amount;
        
        fly = true;
    }

    void LateUpdate()
    {
        if (fly)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            arrived = (transform.position == target.transform.position);
        }

        if (arrived)
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
            
            Destroy(gameObject);
        }
    }
}
