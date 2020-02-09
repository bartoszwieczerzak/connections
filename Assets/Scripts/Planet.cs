using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Planet : MonoBehaviour
{
    [SerializeField] private int _units = 0;
    [SerializeField] private Owner _owner = Owner.None;
    [SerializeField] private PlanetStats _planetStats = null;

    public int Units
    {
        get => _units;
        set => _units = value;
    }

    public Owner Owner => _owner;

    public bool OwnByPlayer => _owner == Owner.Player;
    public bool OwnByAi => _owner == Owner.Ai;
    public bool OwnByNoone => _owner == Owner.None;

    void Start()
    {
        transform.localScale = new Vector2(_planetStats.size, _planetStats.size);

        StartCoroutine(AddTroopsCoroutine());
    }

    private IEnumerator AddTroopsCoroutine()
    {
        yield return new WaitForSeconds(_planetStats.populationCycleTime);

        if (Owner != Owner.None)
        {
            //_units += _planetStats.populationGrowth;
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    public void AddUnits(int amount)
    {
        _units += amount;
    }

    public void RemoveUnits(int amount)
    {
        _units -= amount;
        _units = Mathf.Clamp(_units, 0, int.MaxValue);
    }

    public void ChangeOwnership(Owner newOwner)
    {
        _owner = newOwner;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().color =
            _owner.Equals(Owner.Player) ? Game.Instance.PlayerColor : Game.Instance.EnemyColor;
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