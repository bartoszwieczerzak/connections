using UnityEngine;

public class PlayerClicks : MonoBehaviour
{
    private Planet highlightedPlanet;
    private Planet sourcePlanet;
    private Planet targetPlanet;
    private Planet lastTargetPlanet;

    [SerializeField] private GameObject hoverPlanetHighlight;

    [SerializeField] private GameObject selectedPlanetHighlight;

    [SerializeField] private ParticleSystem particleSystemPrefab;

    private ParticleSystem.Particle[] particles;

    private int unitsGathered = 0;
    [SerializeField] private float unitsGatherSpeed = 2.0f;

    private void Start()
    {
        particles = new ParticleSystem.Particle[particleSystemPrefab.main.maxParticles];
    }

    void Update()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag(Tag.PLANET))
            {
                highlightedPlanet = hit.transform.GetComponent<Planet>();
                hoverPlanetHighlight.transform.position = highlightedPlanet.transform.position;
                hoverPlanetHighlight.SetActive(true);
            }
        }
        else
        {
            highlightedPlanet = null;
            hoverPlanetHighlight.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && CanSelectAsSourcePlanet())
        {
            sourcePlanet = highlightedPlanet;
            selectedPlanetHighlight.transform.position = sourcePlanet.transform.position;
            selectedPlanetHighlight.SetActive(true);

            AudioManager.Instance.Play(SoundType.PLANET_SELECTED);
        }

        if (Input.GetMouseButton(0) && sourcePlanet != null)
        {
            unitsGathered += Mathf.FloorToInt(unitsGatherSpeed * Time.deltaTime);

            if (unitsGathered >= sourcePlanet.Units)
            {
                unitsGathered = sourcePlanet.Units - 1;
            }

            Debug.Log("GETTING MORE UNITS!: " + unitsGathered);
        }

        if (sourcePlanet && !targetPlanet)
        {
            LineRenderer moveMarker = sourcePlanet.GetComponentInChildren<LineRenderer>();
            moveMarker.SetPosition(1,
                -(sourcePlanet.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (CanSelectAsTargetPlanet())
            {
                targetPlanet = highlightedPlanet;
            }
            else if (sourcePlanet)
            {
                LineRenderer moveMarker = sourcePlanet.GetComponentInChildren<LineRenderer>();
                moveMarker.SetPosition(1, Vector3.zero);

                selectedPlanetHighlight.SetActive(false);

                sourcePlanet = null;
            }
        }

        if (sourcePlanet && targetPlanet)
        {
            Game.instance.SendArmy(Owner.Player, sourcePlanet, targetPlanet);

            LineRenderer moveMarker = sourcePlanet.GetComponentInChildren<LineRenderer>();
            moveMarker.SetPosition(1, Vector3.zero);

            selectedPlanetHighlight.SetActive(false);

            sourcePlanet = null;
            lastTargetPlanet = targetPlanet;
            targetPlanet = null;
        }
    }

    private bool CanSelectAsSourcePlanet()
    {
        return sourcePlanet == null && highlightedPlanet && highlightedPlanet.Owner == Owner.Player;
    }

    private bool CanSelectAsTargetPlanet()
    {
        return sourcePlanet != null && highlightedPlanet &&
               highlightedPlanet.GetInstanceID() != sourcePlanet.GetInstanceID();
    }

    void XLateUpdate()
    {
        if (sourcePlanet == null || lastTargetPlanet == null)
        {
            particleSystemPrefab.gameObject.SetActive(false);
            return;
        }

        Debug.Log("SOURCE: " + sourcePlanet.name + " TARGET: " + lastTargetPlanet.name);
        particleSystemPrefab.gameObject.SetActive(true);

        particleSystemPrefab.transform.position = sourcePlanet.transform.position;

        int length = particleSystemPrefab.GetParticles(particles);
        int i = 0;

        transform.LookAt(lastTargetPlanet.transform);

        while (i < length)
        {
            //Target is a Transform object
            Vector3 direction = lastTargetPlanet.transform.position - particles[i].position;
            direction.Normalize();

            float variableSpeed = (particleSystemPrefab.startSpeed / (particles[i].remainingLifetime + 0.1f)) +
                                  particles[i].startLifetime;
            particles[i].position += direction * (variableSpeed * Time.deltaTime);

            if (Vector3.Distance(lastTargetPlanet.transform.position, particles[i].position) < 1.0f)
            {
                particles[i].remainingLifetime = -0.1f; //Kill the particle
            }

            i++;
        }

        particleSystemPrefab.SetParticles(particles, length);
    }
}