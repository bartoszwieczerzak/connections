﻿using UnityEngine;

public class PlayerClicks : MonoBehaviour
{
    private Planet _highlightedPlanet;
    private Planet _sourcePlanet;
    private Planet _targetPlanet;
    private Planet _lastTargetPlanet;
    private ParticleSystem.Particle[] _particles;
    private int _unitsGathered = 0;
    
    [SerializeField] private GameObject hoverPlanetHighlight;
    [SerializeField] private GameObject selectedPlanetHighlight;
    [SerializeField] private ParticleSystem particleSystemPrefab;
    [SerializeField] private float unitsGatherSpeed = 2.0f;

    private void Start()
    {
        _particles = new ParticleSystem.Particle[particleSystemPrefab.main.maxParticles];
    }

    void Update()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag(Tag.Planet))
            {
                _highlightedPlanet = hit.transform.GetComponent<Planet>();
                hoverPlanetHighlight.transform.position = _highlightedPlanet.transform.position;
                hoverPlanetHighlight.SetActive(true);
            }
        }
        else
        {
            _highlightedPlanet = null;
            hoverPlanetHighlight.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && CanSelectAsSourcePlanet())
        {
            _sourcePlanet = _highlightedPlanet;
            selectedPlanetHighlight.transform.position = _sourcePlanet.transform.position;
            selectedPlanetHighlight.SetActive(true);

            AudioManager.Instance.Play(SoundType.PlanetSelected);
        }

        if (Input.GetMouseButton(0) && _sourcePlanet != null)
        {
            _unitsGathered += Mathf.FloorToInt(unitsGatherSpeed * Time.deltaTime);

            if (_unitsGathered >= _sourcePlanet.Units)
            {
                _unitsGathered = _sourcePlanet.Units - 1;
            }

            Debug.Log("GETTING MORE UNITS!: " + _unitsGathered);
        }

        if (_sourcePlanet && !_targetPlanet)
        {
            LineRenderer moveMarker = _sourcePlanet.GetComponentInChildren<LineRenderer>();
            moveMarker.SetPosition(1,
                -(_sourcePlanet.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (CanSelectAsTargetPlanet())
            {
                _targetPlanet = _highlightedPlanet;
            }
            else if (_sourcePlanet)
            {
                LineRenderer moveMarker = _sourcePlanet.GetComponentInChildren<LineRenderer>();
                moveMarker.SetPosition(1, Vector3.zero);

                selectedPlanetHighlight.SetActive(false);

                _sourcePlanet = null;
            }
        }

        if (_sourcePlanet && _targetPlanet)
        {
            Game.instance.SendArmy(Owner.Player, _sourcePlanet, _targetPlanet);

            LineRenderer moveMarker = _sourcePlanet.GetComponentInChildren<LineRenderer>();
            moveMarker.SetPosition(1, Vector3.zero);

            selectedPlanetHighlight.SetActive(false);

            _sourcePlanet = null;
            _lastTargetPlanet = _targetPlanet;
            _targetPlanet = null;
        }
    }

    private bool CanSelectAsSourcePlanet()
    {
        return _sourcePlanet == null && _highlightedPlanet && _highlightedPlanet.Owner == Owner.Player;
    }

    private bool CanSelectAsTargetPlanet()
    {
        return _sourcePlanet != null && _highlightedPlanet &&
               _highlightedPlanet.GetInstanceID() != _sourcePlanet.GetInstanceID();
    }

    void XLateUpdate()
    {
        if (_sourcePlanet == null || _lastTargetPlanet == null)
        {
            particleSystemPrefab.gameObject.SetActive(false);
            return;
        }

        Debug.Log("SOURCE: " + _sourcePlanet.name + " TARGET: " + _lastTargetPlanet.name);
        particleSystemPrefab.gameObject.SetActive(true);

        particleSystemPrefab.transform.position = _sourcePlanet.transform.position;

        int length = particleSystemPrefab.GetParticles(_particles);
        int i = 0;

        transform.LookAt(_lastTargetPlanet.transform);

        while (i < length)
        {
            //Target is a Transform object
            Vector3 direction = _lastTargetPlanet.transform.position - _particles[i].position;
            direction.Normalize();

            float variableSpeed = (particleSystemPrefab.startSpeed / (_particles[i].remainingLifetime + 0.1f)) +
                                  _particles[i].startLifetime;
            _particles[i].position += direction * (variableSpeed * Time.deltaTime);

            if (Vector3.Distance(_lastTargetPlanet.transform.position, _particles[i].position) < 1.0f)
            {
                _particles[i].remainingLifetime = -0.1f; //Kill the particle
            }

            i++;
        }

        particleSystemPrefab.SetParticles(_particles, length);
    }
}