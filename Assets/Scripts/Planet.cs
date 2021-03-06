﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Planet : MonoBehaviour
{
    [SerializeField] private int _units;
    [SerializeField] private Owner _owner = Owner.None;
    [SerializeField] private int _resupplyAmount = 1;
    [SerializeField] private bool _isMainPlanet = false;
    
    private float _populationCycleTime = 3.0f;
    [Header("Planet statistics")]
    [SerializeField]
    [Range(-5, 10)]
    private int  _populationGrowth = 1;
    [SerializeField]
    [Range(1, 5)]
    private int _defenseMultiplier = 1;

    [SerializeField]
    private Sprite[] _populationGrowthAmountIcons;
    [SerializeField]
    private SpriteRenderer _populationGrowthIconRenderer;
    
    [SerializeField]
    private Sprite[] _defenseAmountIcons;
    [SerializeField]
    private SpriteRenderer _defenceIconRenderer;
    
    [Header("Prefabs")]
    [SerializeField] private Ship _playerShipPrefab;
    [SerializeField] private Ship _enemyShipPrefab;
    [SerializeField] private GameObject _supplyChainMarkerPrefab;

    [Header("Units growth")]
    [SerializeField] private int _populationGrowtMinMaxRange = 100;

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

    public bool IsMainPlanet => _isMainPlanet;
    
    private Planet _supplyingPlanet;
    private Planet _previousSupplyingPlanet;

    private bool _supplyChainAlreadyStarted;
    private GameObject _supplyChainMarkerGo;
    private Canvas _mainGuiCanvas;

    [Header("Range")]
    [SerializeField] private GameObject _rangeCircle;
    [SerializeField] private float _planetRange = 5f;

    public float PlanetRange => _planetRange;
    private float _cooldownTime;

    public bool IsCooldownActive => _cooldownTime > 0f;

    public Owner Owner => _owner;

    public void ShowRange()
    {
        _rangeCircle.SetActive(true);
    }
    
    public void HideRange()
    {
        _rangeCircle.SetActive(false);
    }
    
    public Planet SupplyingPlanet
    {
        get => _supplyingPlanet;
        set => _supplyingPlanet = value;
    }

    void Start()
    {
        _defenceIconRenderer.sprite = _defenseAmountIcons[_defenseMultiplier];
        //Debug.Log("Planet " + name + " growth: " + _populationGrowth + " [" + _populationGrowth + 5 + "]");
        _populationGrowthIconRenderer.sprite = _populationGrowthAmountIcons[_populationGrowth + 5];
    
        _mainGuiCanvas = GetComponentInChildren<Canvas>();
        _shieldLabel.text = "x" + _defenseMultiplier;
        _growthLabel.text = _populationGrowth.ToString();

        StartCoroutine(AddTroopsCoroutine());
        
        _rangeCircle.transform.localScale = new Vector3(PlanetRange, PlanetRange, 1);
    }

    void Update()
    {
        if (IsCooldownActive)
        {
            _cooldownCircle.fillAmount = _cooldownTime/100;
            _cooldownTime -= Time.deltaTime;
        }
        _shieldLabel.text = "x" + _defenseMultiplier;
        _unitsLabel.color = Owner == Owner.Player1 ? Game.Instance.PlayerColor : Owner == Owner.Player2 ? Game.Instance.EnemyColor : Game.Instance.NooneColor;
        
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
        yield return new WaitForSeconds(_populationCycleTime);

        if (Owner != Owner.None)
        {
            GrowPopulation();
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    private void GrowPopulation()
    {
        float growUnitsBy = Units * (_populationGrowth / 100f);

        growUnitsBy = Mathf.Clamp(growUnitsBy, - _populationGrowtMinMaxRange, _populationGrowtMinMaxRange);
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
                _supplyingPlanet.ResupplyUnits(_owner, _resupplyAmount);
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
        if (amount == 0) return;
        
        string animTrigger = gain ? "UnitsGain" : "UnitsLost";
        string prefixSign = "";// gain ? "+" : "-";

        Vector3 newPosition =  new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.2f,0.2f), 0) + transform.position;
        
        GameObject unitsGainLoseLabelGo = Instantiate(_unitsGainLoseLabelPrefab, newPosition, Quaternion.identity, _mainGuiCanvas.transform);
        TextMeshProUGUI unitsGainLoseLabel = unitsGainLoseLabelGo.GetComponent<TextMeshProUGUI>();
        Animator animator = unitsGainLoseLabelGo.GetComponent<Animator>();
        unitsGainLoseLabel.text = prefixSign + amount;
        animator.SetTrigger(animTrigger);
    }

    public void ResupplyUnits(Owner shipOwner, int amount)
    {
        AddUnits(amount);
        _owner = shipOwner;
    }

    public void TakeDamage(Owner shipOwner, int unitsAmount)
    {
        int unitsToRemove = Mathf.FloorToInt(unitsAmount / _defenseMultiplier);
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

        var shipPrefab = Owner == Owner.Player1 ? _playerShipPrefab : _enemyShipPrefab;
        
        Ship ship = Instantiate(shipPrefab, transform.position, shipRotation, transform);

        ship.Fly(_owner, this, targetPlanet, unitsToSend);

        ShowGainLoseText(unitsToSend, false);

        Units -= unitsToSend;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PlanetRange);
    }
}