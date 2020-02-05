using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerClicks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Planet _sourcePlanet;
    private Planet _targetPlanet;
    private int _unitsGathered = 0;

    [SerializeField] private float _unitsGatherSpeed = 2.0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        Planet planet = gameObject.GetComponent<Planet>();

        if (CanSelectAsSourcePlanet(planet))
        {
            _sourcePlanet = planet;

            AudioManager.Instance.Play(SoundType.PlanetSelected);
        }

        if (_sourcePlanet != null)
        {
            _unitsGathered += Mathf.FloorToInt(_unitsGatherSpeed * Time.deltaTime);

            if (_unitsGathered >= _sourcePlanet.Units)
            {
                _unitsGathered = _sourcePlanet.Units - 1;
            }

            Debug.Log("GETTING MORE UNITS!: " + _unitsGathered);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Planet planet = GetPlanetFromEventData(eventData);

        if (CanSelectAsTargetPlanet(_sourcePlanet, planet))
        {
            _targetPlanet = planet;
        }

        if (_sourcePlanet && _targetPlanet)
        {
            Game.Instance.SendArmy(Owner.Player, _sourcePlanet, _targetPlanet);
        }

        _sourcePlanet = null;
        _targetPlanet = null;
    }

    Planet GetPlanetFromEventData(PointerEventData eventData)
    {
        GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
        Planet planet = gameObject.GetComponentInParent<Planet>();

        return planet;
    }

    bool CanSelectAsSourcePlanet(Planet planet)
    {
        return planet != null && planet.Owner == Owner.Player;
    }

    bool CanSelectAsTargetPlanet(Planet sourcePlanet, Planet targetPlanet)
    {
        return sourcePlanet != null && targetPlanet != null && targetPlanet.GetInstanceID() != _sourcePlanet.GetInstanceID();
    }
}