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

    [SerializeField] private Color _playerColor = default;
    [SerializeField] private Color _enemyColor = default;
    [SerializeField] private GameObject _winPanel = default;
    [SerializeField] private GameObject _losePanel = default;
    
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
                // SendTroops method
                if (target.Owner == Owner.None)
                {
                    PlaySound(who, SoundType.PlanetAcquired);
                }

                int unitsToSend = PlayerClicks.Instance.UnitsGathered;
                Debug.Log("SENDING " + unitsToSend + " units!");
                source.RemoveUnits(unitsToSend);
                target.AddUnits(unitsToSend);
                target.ChangeOwnership(who);
            }
            else
            {
                // AttackEnemy method
                int unitsToSend = Mathf.FloorToInt(source.Units / 2);
                Debug.Log("HAS: " + source.Units + " AND WILL REMOVE: " + unitsToSend);
                source.RemoveUnits(unitsToSend);

                Debug.Log("LEFT: " + source.Units);

                if (target.Units > unitsToSend)
                {
                    target.RemoveUnits(unitsToSend);
                    Debug.Log("REMOVED from TARGET: " + unitsToSend);

                    PlaySound(who, SoundType.BattleLost);
                }
                else if (target.Units < unitsToSend)
                {
                    int toBeAdded = unitsToSend - target.Units;
                    target.Units = 0;
                    target.AddUnits(toBeAdded);
                    Debug.Log("ADDED to TARGET: " + toBeAdded);
                    target.ChangeOwnership(who);

                    PlaySound(who, SoundType.PlanetTakenOver);
                    PlaySound(who, SoundType.PlanetLost);
                }
                else
                {
                    target.Units = 0;
                    target.ChangeOwnership(Owner.None);

                    PlaySound(who, SoundType.BattleLost);
                }
            }

            source.SendFleet(target);

            PlaySound(who, SoundType.SendingArmyPlayer);
        }
    }

    private void PlaySound(Owner who, SoundType sound)
    {
        if (who == Owner.Ai)
        {
            AudioManager.Instance.Play(sound);
        }
        
        if (who == Owner.Player)
        {
            AudioManager.Instance.Play(sound);
        }
    }
}