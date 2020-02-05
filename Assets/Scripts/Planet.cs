using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Planet : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [SerializeField] private int _units = 0;
    [SerializeField] private Owner _owner = Owner.None;
    [SerializeField] private PlanetStats _planetStats;

    [SerializeField] private GameObject _hoverPlanetHighlight;
    [SerializeField] private GameObject _selectedPlanetHighlight;
    [SerializeField] private LineRenderer _moveMarker;

    public int Units
    {
        get => _units;
        set => _units = value;
    }

    public Owner Owner => _owner;

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
            _units += _planetStats.populationGrowth;
        }

        StartCoroutine(AddTroopsCoroutine());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverPlanetHighlight.transform.position = transform.position;
        _hoverPlanetHighlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverPlanetHighlight.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Owner == Owner.Player)
        {
            _selectedPlanetHighlight.SetActive(true);

            _moveMarker.SetPosition(1, Vector3.zero);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Owner == Owner.Player)
        {
            _moveMarker.SetPosition(1, -(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Owner == Owner.Player)
        {
            _selectedPlanetHighlight.SetActive(false);

            _moveMarker.SetPosition(1, Vector3.zero);
        }
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