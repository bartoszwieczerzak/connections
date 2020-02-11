using UnityEngine;

public class Ship : MonoBehaviour
{
    private Owner _who;
    private Planet _source;
    private Planet _target;
    private int _amount;
    private bool _fly = false;
    private bool _arrived = false;

    public float speed = 1.0f;

    public void Fly(Owner who, Planet source, Planet target, int amount)
    {
        _who = who;
        _source = source;
        _target = target;
        _amount = amount;
        
        transform.position = _source.transform.position;
        
        _fly = true;
    }

    void LateUpdate()
    {
        if (_fly)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, _target.transform.position, speed * Time.deltaTime);

            transform.position = newPosition;

            _arrived = Vector3.Distance(newPosition, _target.transform.position) <= 0.1f;
        }

        if (_arrived)
        {
            GameActions.Instance.Disembark(_who, _target, _amount);
            
            Destroy(gameObject);
        }
    }
}
