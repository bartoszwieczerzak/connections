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
    [SerializeField] private Color _nooneColor;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    
    private readonly List<Planet> _planets = new List<Planet>();

    public Color PlayerColor => _playerColor;
    public Color EnemyColor => _enemyColor;
    public Color NooneColor => _nooneColor;

    public List<Planet> Planets => _planets;

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
        CheckWinLoseCondition();
    }

    private void CheckWinLoseCondition()
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
        // Debug.Log("That's a draw!");
    }

    private void GameOver()
    {
        // Debug.Log("Game Over man!");
        _losePanel.SetActive(true);

        //AudioManager.instance.Play(SoundType.GAME_LOST);
    }

    private void GameWin()
    {
        // Debug.Log("You win!");
        _winPanel.SetActive(true);

        //AudioManager.instance.Play(SoundType.GAME_WON);
    }
}