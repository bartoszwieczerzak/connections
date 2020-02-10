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
    
    private Planet _supplyingPlanet;
    private Planet _previousSupplyingPlanet;
    
    private bool _supplyChainAlreadyStarted = false;
    [SerializeField]
    private GameObject _supplyChainMarkerPrefab;
    private GameObject _supplyChainMarkerGo;
    
    [SerializeField]
    private TextMeshProUGUI _unitsLabel;
    [SerializeField]
    private TextMeshProUGUI _shieldLabel;
    [SerializeField]
    private TextMeshProUGUI _growthLabel;
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
        //_planetStats.populationCycleTime
        _shieldLabel.text = _planetStats.defenseBonus.ToString();
        _growthLabel.text = _planetStats.populationGrowth.ToString();
        transform.localScale = new Vector2(_planetStats.size, _planetStats.size);

        StartCoroutine(AddTroopsCoroutine());
    }

    void Update()
    {
        _unitsLabel.color = OwnByPlayer ? Game.Instance.PlayerColor : OwnByAi? Game.Instance.EnemyColor : Game.Instance.NooneColor;

        Debug.Log("_supplyingPlanet: " + _supplyingPlanet + " _previousSupplyingPlanet: " + _previousSupplyingPlanet + " _supplyChainAlreadyStarted: " + _supplyChainAlreadyStarted);
        if (_previousSupplyingPlanet && _previousSupplyingPlanet != _supplyingPlanet)
        {
            if (_supplyChainMarkerGo)  Destroy(_supplyChainMarkerGo);
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
        yield return new WaitForSeconds(_planetStats.populationCycleTime);

        if (Owner != Owner.None)
        {
            _units += _planetStats.populationGrowth;
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    private IEnumerator SendSupplyCoroutine()
    {
        yield return new WaitForSeconds(/*_planetStats.populationCycleTime*/2f);

        if (_supplyingPlanet && _supplyingPlanet.Owner == _owner)
        {
            if (_units > 1)
            {
                RemoveUnits(1);
                _supplyingPlanet.AddUnits(1);                
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