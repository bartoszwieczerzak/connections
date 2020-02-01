using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Singleton

    public static Game Instance;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Trying to create another instance of Game object!");
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion

    [SerializeField] private Color _playerColor;
    [SerializeField] private Color _enemyColor;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    
    private readonly List<Planet> _planets = new List<Planet>();
    
    public Color PlayerColor => _playerColor;
    public Color EnemyColor => _enemyColor;
    void Start()
    {
        GameObject[] planetGameObjects = GameObject.FindGameObjectsWithTag(Tag.Planet);

        foreach (GameObject go in planetGameObjects)
        {
            _planets.Add(go.GetComponent<Planet>());
        }
    }

    void Update()
    {
        var playerPlanetsCount = _planets.Count(p => p.Owner == Owner.Player);
        var enemyPlanetsCount = _planets.Count(p => p.Owner == Owner.Ai);
        var noonePlanetsCount = _planets.Count(p => p.Owner == Owner.None);

        if (playerPlanetsCount == 0 && enemyPlanetsCount == 0)
        {
            GameDraw();
        }
        else if (enemyPlanetsCount == 0)
        {
            GameWin();
        }
        else if (playerPlanetsCount == 0)
        {
            GameOver();
        }
    }

    private void GameDraw()
    {
        Debug.Log("That's a draw!");
        ;
    }

    private void GameOver()
    {
        Debug.Log("Game Over man!");
        _losePanel.SetActive(true);

        //AudioManager.instance.Play(SoundType.GAME_LOST);
    }

    private void GameWin()
    {
        Debug.Log("You win!");
        _winPanel.SetActive(true);

        //AudioManager.instance.Play(SoundType.GAME_WON);
    }

    public void SendArmy(Owner who, Planet source, Planet target)
    {
        if (source.Units > 1)
        {
            if (who != source.Owner)
            {
                Debug.LogWarning(source.Owner + " cannot send army from " + source.name + "!");
                return;
            }

            if (target.Owner == who || target.Owner == Owner.None)
            {
                SendTroops(who, source, target);
            }
            else
            {
                AttackEnemy(who, source, target);
            }

            source.SendFleet(target);
            AudioManager.Instance.Play(SoundType.SendingArmyPlayer);
        }
    }

    private void AttackEnemy(Owner who, Planet sourcePlanet, Planet targetPlanet)
    {
        int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);
        Debug.Log("HAS: " + sourcePlanet.Units + " AND WILL REMOVE: " + unitsToSend);
        sourcePlanet.RemoveUnits(unitsToSend);

        Debug.Log("LEFT: " + sourcePlanet.Units);

        if (targetPlanet.Units > unitsToSend)
        {
            targetPlanet.RemoveUnits(unitsToSend);
            Debug.Log("REMOVED from TARGET: " + unitsToSend);

            AudioManager.Instance.Play(SoundType.BattleLost);
        }
        else if (targetPlanet.Units < unitsToSend)
        {
            int toBeAdded = unitsToSend - targetPlanet.Units;
            targetPlanet.Units = 0;
            targetPlanet.AddUnits(toBeAdded);
            Debug.Log("ADDED to TARGET: " + toBeAdded);
            targetPlanet.ChangeOwnership(who);

            AudioManager.Instance.Play(SoundType.PlanetTakenOver);
        }
        else
        {
            targetPlanet.Units = 0;
            targetPlanet.ChangeOwnership(Owner.None);
        }
    }

    private void SendTroops(Owner who, Planet sourcePlanet, Planet targetPlanet)
    {
        int unitsToSend = Mathf.FloorToInt(sourcePlanet.Units / 2);
        sourcePlanet.RemoveUnits(unitsToSend);
        targetPlanet.AddUnits(unitsToSend);
        targetPlanet.ChangeOwnership(who);
    }
}