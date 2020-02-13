using System.Collections;
using TMPro;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private int _units;
    [SerializeField] private Owner _owner = Owner.None;
    [SerializeField] private PlanetStats _planetStats;
    [SerializeField] private int _resupplyAmount = 1;
    
    [Header("Prefabs")]
    [SerializeField] private Ship _playerShipPrefab;
    [SerializeField] private Ship _enemyShipPrefab;
    [SerializeField] private GameObject _supplyChainMarkerPrefab;
    
    [Header("Units growth")]
    [SerializeField] private float _minPopulationGrowth = 1f;
    [SerializeField] private float _maxPopulationGrowth = 100f;

    [Header("Text labels")]
    [SerializeField] private TextMeshProUGUI _unitsLabel;
    [SerializeField] private TextMeshProUGUI _shieldLabel;
    [SerializeField] private TextMeshProUGUI _growthLabel;
    public int Units
    {
        get => _units;
        set
        {
            if (value == 0)
            {
                _owner = Owner.None;
            }

            _units = value;
        }
    }

    private Planet _supplyingPlanet;
    private Planet _previousSupplyingPlanet;

    private bool _supplyChainAlreadyStarted = false;
    private GameObject _supplyChainMarkerGo;


    public Owner Owner => _owner;

    public bool OwnByPlayer => _owner == Owner.Player;
    public bool OwnByAi => _owner == Owner.Ai;
    public bool OwnByNoone => _owner == Owner.None;

    public Planet SupplyingPlanet
    {
        get => _supplyingPlanet;
        set => _supplyingPlanet = value;
    }

    void Start()
    {
        _shieldLabel.text = "x" + _planetStats.DefenseMultiplier;
        _growthLabel.text = "+" + _planetStats.PopulationGrowth + "/" + _planetStats.PopulationCycleTime + "s";

        StartCoroutine(AddTroopsCoroutine());
    }

    void Update()
    {
        _shieldLabel.text = "x" + _planetStats.DefenseMultiplier;
        _unitsLabel.color = OwnByPlayer ? Game.Instance.PlayerColor : OwnByAi ? Game.Instance.EnemyColor : Game.Instance.NooneColor;

        Debug.Log("_supplyingPlanet: " + _supplyingPlanet + " _previousSupplyingPlanet: " + _previousSupplyingPlanet + " _supplyChainAlreadyStarted: " + _supplyChainAlreadyStarted);
        if (_previousSupplyingPlanet && _previousSupplyingPlanet != _supplyingPlanet)
        {
            if (_supplyChainMarkerGo) Destroy(_supplyChainMarkerGo);
            _previousSupplyingPlanet = _supplyingPlanet;
            _supplyChainAlreadyStarted = false;
        }

        if (_supplyingPlanet && !_supplyChainAlreadyStarted)
        {
            _supplyChainMarkerGo = Instantiate(_supplyChainMarkerPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            var markerLineRenderer = _supplyChainMarkerGo.GetComponent<LineRenderer>();
            markerLineRenderer.SetPositions(new[] {_supplyingPlanet.transform.position, transform.position});
            _previousSupplyingPlanet = _supplyingPlanet;
            StartCoroutine(SendSupplyCoroutine());
            _supplyChainAlreadyStarted = true;
        }
    }

    private IEnumerator AddTroopsCoroutine()
    {
        yield return new WaitForSeconds(_planetStats.PopulationCycleTime);

        if (Owner != Owner.None)
        {
            GrowPopulation();
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    private void GrowPopulation()
    {
        float growUnitsBy = Units * (_planetStats.PopulationGrowth / 100f);
        
        growUnitsBy = Mathf.Clamp(growUnitsBy, _minPopulationGrowth, _maxPopulationGrowth);
        Units += Mathf.FloorToInt(growUnitsBy);

        Units = Mathf.Clamp(Units, 0, int.MaxValue);
    }

    private IEnumerator SendSupplyCoroutine()
    {
        yield return new WaitForSeconds( /*_planetStats.populationCycleTime*/2f);

        if (_supplyingPlanet && _supplyingPlanet.Owner == _owner)
        {
            if (Units > 1)
            {
                Units -= _resupplyAmount;
                _supplyingPlanet.ResupplyUnits(_resupplyAmount);
            }
        }
        else
        {
            _supplyingPlanet = null;
            _supplyChainAlreadyStarted = false;
            Destroy(_supplyChainMarkerGo);
        }

        StartCoroutine(SendSupplyCoroutine());
    }

    public void ResupplyUnits(int amount)
    {
        Units += amount;
    }

    public void TakeDamage(Owner shipOwner, int unitsAmount)
    {
        var unitsToRemove = unitsAmount / _planetStats.DefenseMultiplier;
        var unitsLeft = Mathf.FloorToInt(Units - unitsToRemove);
        if (unitsLeft < 0)
        {
            _owner = shipOwner;
            Units = Mathf.Abs(unitsLeft);
        }
        else
        {
            Units = unitsLeft;
        }

        Units = Mathf.Clamp(Units, 0, int.MaxValue);
    }

    public void SendShip(Planet targetPlanet, int unitsToSend)
    {
        if (Units <= unitsToSend) return;

        Vector2 offset = targetPlanet.transform.position - transform.position;
        Quaternion shipRotation = Quaternion.LookRotation(Vector3.forward, offset) * Quaternion.Euler(0, 0, 90);

        var shipPrefab = OwnByPlayer ? _playerShipPrefab : _enemyShipPrefab;
        Ship ship = Instantiate(shipPrefab, transform.position, shipRotation, transform);

        ship.Fly(_owner, this, targetPlanet, unitsToSend);

        Units -= unitsToSend;
    }
}