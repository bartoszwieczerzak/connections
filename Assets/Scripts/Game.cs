﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        // DontDestroyOnLoad(this);
    }

    #endregion

    [Header("Player colors")] [SerializeField]
    private Color _playerColor;

    [SerializeField] private Color _enemyColor;
    [SerializeField] private Color _nooneColor;
    [Header("Panels")] [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    [Header("Hud elements")] [SerializeField]
    private Slider _unitsSlider;

    [SerializeField] private TextMeshProUGUI _playerUnitsCountText;
    [SerializeField] private TextMeshProUGUI _aiUnitsCountText;
    [SerializeField] private TextMeshProUGUI _playerPlanetsCountText;
    [SerializeField] private TextMeshProUGUI _noonePlanetsCountText;
    [SerializeField] private TextMeshProUGUI _aiPlanetsCountText;

    private readonly List<Planet> _planets = new List<Planet>();
    public readonly List<Ship> Ships = new List<Ship>();

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
        var noonePlanetsCount = _planets.Count(p => p.Owner == Owner.None);
        var aiPlanetsCount = _planets.Count(p => p.Owner == Owner.Ai);

        _playerPlanetsCountText.text = "P: " + playerPlanetsCount;
        _noonePlanetsCountText.text = noonePlanetsCount.ToString();
        _aiPlanetsCountText.text = "P: " + aiPlanetsCount;

        var playerPlanetsUnitsCount = _planets.FindAll(p => p.Owner == Owner.Player).Sum(p => p.Units);
        var aiPlanetsUnitsCount = _planets.FindAll(p => p.Owner == Owner.Ai).Sum(p => p.Units);

        var playerUnitsInShips = Ships.FindAll(s => s.ShipOwner == Owner.Player).Sum(s => s.UnitsAmount);
        var aiUnitsInShips = Ships.FindAll(s => s.ShipOwner == Owner.Ai).Sum(s => s.UnitsAmount);

        var totalPlayerUnits = playerPlanetsUnitsCount + playerUnitsInShips;
        var totalAiUnits = aiPlanetsUnitsCount + aiUnitsInShips;
        var totalUnits = totalPlayerUnits + totalAiUnits;

        float ratio = (float)totalPlayerUnits / totalUnits;
        
        _unitsSlider.value = ratio;
        _playerUnitsCountText.text = "U: " + totalPlayerUnits;
        _aiUnitsCountText.text = "U: " + totalAiUnits;

        if (playerPlanetsCount == 0 && aiPlanetsCount == 0)
        {
            GameDraw();
        }
        else if (aiPlanetsCount == 0)
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