using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _unitsGainLoseLabelPrefab;
    [SerializeField] private Image _cooldownCircle;

    [Header("Cooldown")]
    [SerializeField] private int _cooldownMaxUnits = 1000;
    [SerializeField] private float _cooldownMaxTime = 10.0f;
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

    private bool _supplyChainAlreadyStarted;
    private GameObject _supplyChainMarkerGo;
    private Canvas _mainGuiCanvas;
    
    private float _cooldownTime = 0f;

    public bool IsCooldownActive => _cooldownTime > 0f;

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
        _mainGuiCanvas = GetComponentInChildren<Canvas>();
        _shieldLabel.text = "x" + _planetStats.DefenseMultiplier;
        _growthLabel.text = "+" + _planetStats.PopulationGrowth + "/" + _planetStats.PopulationCycleTime + "s";

        StartCoroutine(AddTroopsCoroutine());
    }

    void Update()
    {
        if (IsCooldownActive)
        {
            _cooldownCircle.fillAmount = _cooldownTime/100;
            _cooldownTime -= Time.deltaTime;
        }
        _shieldLabel.text = "x" + _planetStats.DefenseMultiplier;
        _unitsLabel.color = OwnByPlayer ? Game.Instance.PlayerColor : OwnByAi ? Game.Instance.EnemyColor : Game.Instance.NooneColor;
        
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

    public void ActivateCooldown(float cooldownTime)
    {
        _cooldownTime = cooldownTime;
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
        int unitsgrow = Mathf.FloorToInt(growUnitsBy);

        AddUnits(unitsgrow);
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

    private void AddUnits(int amount)
    {
        Units += amount;

        Units = Mathf.Clamp(Units, 0, int.MaxValue);

        ShowGainLoseText(amount, true);
    }

    private void ShowGainLoseText(int amount, bool gain)
    {
        string animTrigger = gain ? "UnitsGain" : "UnitsLost";
        string prefixSign = gain ? "+" : "-";

        Vector3 newPosition =  new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.2f,0.2f), 0) + transform.position;
        
        GameObject unitsGainLoseLabelGo = Instantiate(_unitsGainLoseLabelPrefab, newPosition, Quaternion.identity, _mainGuiCanvas.transform);
        TextMeshProUGUI unitsGainLoseLabel = unitsGainLoseLabelGo.GetComponent<TextMeshProUGUI>();
        Animator animator = unitsGainLoseLabelGo.GetComponent<Animator>();
        unitsGainLoseLabel.text = prefixSign + amount;
        animator.SetTrigger(animTrigger);
    }

    public void ResupplyUnits(int amount)
    {
        AddUnits(amount);
    }

    public void TakeDamage(Owner shipOwner, int unitsAmount)
    {
        int unitsToRemove = Mathf.FloorToInt(unitsAmount / _planetStats.DefenseMultiplier);
        var unitsLeft = Units - unitsToRemove;

        ShowGainLoseText(unitsToRemove, false);

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
        if (Units <= unitsToSend) unitsToSend = Units - 1;

        var unitsForCooldown = Mathf.Clamp(unitsToSend, 1, _cooldownMaxUnits);
        var cooldownTime = unitsForCooldown / _cooldownMaxTime;
        ActivateCooldown(cooldownTime);
        
        Vector2 offset = targetPlanet.transform.position - transform.position;
        Quaternion shipRotation = Quaternion.LookRotation(Vector3.forward, offset) * Quaternion.Euler(0, 0, 90);

        var shipPrefab = OwnByPlayer ? _playerShipPrefab : _enemyShipPrefab;
        
        Ship ship = Instantiate(shipPrefab, transform.position, shipRotation, transform);

        ship.Fly(_owner, this, targetPlanet, unitsToSend);

        ShowGainLoseText(unitsToSend, false);

        Units -= unitsToSend;
    }
}