using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerInputs : MonoBehaviour
{
    #region Singleton

    public static PlayerInputs Instance;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Trying to create another instance of PlayerInputs object!");
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion

    private Planet _hoverPlanet;
    private Planet _sourcePlanet;
    private Planet _targetPlanet;
    private float _unitsGathered = 1;

    private LineRenderer _markerLineRenderer;

    private Camera _mainCamera;

    [SerializeField] private GameObject _hoverPlanetHighlight;
    [SerializeField] private GameObject _selectedPlanetHighlight;
    [SerializeField] private float _unitsGatherSpeed = 1.0f;
    [SerializeField] private GameObject _unitsGatheredText;

    public int UnitsGathered => Mathf.FloorToInt(_unitsGathered);

    private void Start()
    {
        _mainCamera = Camera.main;
        _markerLineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        ShowMouseTrailMarker();
        HoverOverPlanet();

        if (Input.GetMouseButtonDown(0))
        {
            PickSourcePlanet();
        }

        if (Input.GetMouseButton(0))
        {
            UnitsChargeUp();
        }

        if (Input.GetMouseButtonUp(0))
        {
            PickTargetPlanet();
            SendUnits();
            _unitsGathered = 1f;
            _unitsGatheredText.SetActive(false);
            _sourcePlanet = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            PickSourcePlanet();
        }

        if (Input.GetMouseButtonUp(1))
        {
            PickTargetPlanet();
            SetupSupplyChain();
            _sourcePlanet = null;
        }
    }

    private void ShowHighlight(Vector3 position)
    {
        _hoverPlanetHighlight.transform.position = position;
        _hoverPlanetHighlight.SetActive(true);
    }

    private void HideHighlight()
    {
        _hoverPlanetHighlight.SetActive(false);
    }

    private void HoverOverPlanet()
    {
        Vector2 ray = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        
        if (hit.collider && hit.transform.CompareTag(Tag.Planet))
        {
            _hoverPlanet = hit.transform.GetComponent<Planet>();
            ShowHighlight(_hoverPlanet.transform.position);
        }
        else
        {
            _hoverPlanet = null;
            HideHighlight();
        }
    }

    private void PickSourcePlanet()
    {
        if (!_hoverPlanet || !_hoverPlanet.OwnByPlayer) return;

        _sourcePlanet = _hoverPlanet;
        ShowHighlight(_hoverPlanet.transform.position);

        AudioManager.Instance.Play(SoundType.PlanetSelected);
    }

    private void UnitsChargeUp()
    {
        if (!_sourcePlanet || _unitsGathered >= _sourcePlanet.Units - 1) return;

        var currentSpeed = _unitsGatherSpeed + Mathf.FloorToInt(_unitsGathered / 5);
        currentSpeed = Mathf.Clamp(currentSpeed, 0, 10);
        _unitsGathered += currentSpeed * Time.deltaTime;
        _unitsGatheredText.transform.position = _sourcePlanet.transform.position;
        _unitsGatheredText.SetActive(true);
    }

    private void ShowMouseTrailMarker()
    {
        Vector3[] positions = _sourcePlanet ? new[] {_sourcePlanet.transform.position, _mainCamera.ScreenToWorldPoint(Input.mousePosition)} : new[] {Vector3.zero, Vector3.zero};
        _markerLineRenderer.SetPositions(positions);
    }

    private void PickTargetPlanet()
    {
        if (_sourcePlanet && _hoverPlanet && _hoverPlanet != _sourcePlanet)
        {
            _targetPlanet = _hoverPlanet;
        }
        else if (_sourcePlanet)
        {
            HideHighlight();

            _sourcePlanet = null;
        }
    }

    private void SendUnits()
    {
        if (!_sourcePlanet || !_targetPlanet) return;
        _sourcePlanet.SendShip(_targetPlanet, UnitsGathered);

        _selectedPlanetHighlight.SetActive(false);

        _sourcePlanet = null;
        _targetPlanet = null;
    }

    private void SetupSupplyChain()
    {
        if (!_sourcePlanet || !_targetPlanet) return;

        _sourcePlanet.SupplyingPlanet = _targetPlanet;
    }
}