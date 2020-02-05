using UnityEngine;

public class PlayerClicks : MonoBehaviour
{
    private Planet _sourcePlanet;
    private Planet _targetPlanet;
    private int _unitsGathered = 0;

    [SerializeField] private float _unitsGatherSpeed = 2.0f;

    private void OnMouseDown()
    {
        Planet planet = GetPlanetUnderCursor();

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

    private void OnMouseUp()
    {
        Planet planet = GetPlanetUnderCursor();

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

    Planet GetPlanetUnderCursor()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

        if (hit.transform)
        {
            return hit.transform.GetComponent<Planet>();
        }

        return null;
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