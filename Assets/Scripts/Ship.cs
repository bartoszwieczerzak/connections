using TMPro;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Owner _shipOwner;
    private Planet _sourcePlanet;
    private Planet _targetPlanet;
    private int _unitsAmount;

    private TMP_Text _unitsLabel;

    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private GameObject fightsEffect;

    public int UnitsAmount => _unitsAmount;
    public Owner ShipOwner => _shipOwner;

    private int _unitsToRemove;
    private float _flyTime = 1f;
    public void Fly(Owner shipOwner, Planet sourcePlanet, Planet targetPlanet, int unitsAmount)
    {
        
        Game.Instance.Ships.Add(this);

        _unitsLabel = GetComponentInChildren<TMP_Text>();

        _shipOwner = shipOwner;
        _sourcePlanet = sourcePlanet;
        _targetPlanet = targetPlanet;
        _unitsAmount = unitsAmount;

        transform.position = _sourcePlanet.transform.position;
    }

    void Update()
    {
        _flyTime += Time.deltaTime;
        if (_flyTime >= 1f)
        {
            _flyTime = 0f;
            _unitsAmount -= _unitsToRemove;

            _unitsToRemove++;
        }
        
        _unitsLabel.text = _unitsAmount.ToString();

        if (_unitsAmount <= 0)
        {
            _unitsAmount = 0;

            Game.Instance.Ships.Remove(this);
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        var newPosition = Vector2.MoveTowards(transform.position, _targetPlanet.transform.position, _speed * Time.deltaTime);

        transform.position = newPosition;

        if (Vector2.Distance(newPosition, _targetPlanet.transform.position) <= 0.1f)
        {
            if (_targetPlanet.Owner == _shipOwner)
            {
                _targetPlanet.ResupplyUnits(UnitsAmount);
            }
            else if (_targetPlanet.Owner != Owner.None)
            {
                _targetPlanet.TakeDamage(_shipOwner, UnitsAmount);
                
                GameObject fights = Instantiate(fightsEffect, transform.position, Quaternion.identity);
                Destroy(fights, 2f);
            }
            else
            {
                _targetPlanet.TakeDamage(_shipOwner, UnitsAmount);
            }

            Game.Instance.Ships.Remove(this);
            Destroy(gameObject);
        }
    }
}