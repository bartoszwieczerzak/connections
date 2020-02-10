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

    private TextMeshProUGUI _unitsLabel;
    public Owner Owner => _owner;

    public bool OwnByPlayer => _owner == Owner.Player;
    public bool OwnByAi => _owner == Owner.Ai;
    public bool OwnByNoone => _owner == Owner.None;

    void Start()
    {
        _unitsLabel = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        transform.localScale = new Vector2(_planetStats.size, _planetStats.size);

        StartCoroutine(AddTroopsCoroutine());
    }

    void Update()
    {
        _unitsLabel.color = OwnByPlayer ? Game.Instance.PlayerColor : OwnByAi? Game.Instance.EnemyColor : Game.Instance.NooneColor;
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