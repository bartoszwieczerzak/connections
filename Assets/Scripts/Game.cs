using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    #region Singleton

    public static Game instance;

    private void Awake() {
        if (instance) {
            Debug.LogWarning("Trying to create another instance of Game object!");
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

    #endregion

    [SerializeField] private Color playerColor;

    public Color PlayerColor => playerColor;
    public Color EnemyColor => enemyColor;

    [SerializeField] private Color enemyColor;
    
    private readonly List<Planet> planets = new List<Planet>();
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    void Start() {
        GameObject[] planetGameObjects = GameObject.FindGameObjectsWithTag(Tag.PLANET);

        foreach (GameObject go in planetGameObjects) {
            planets.Add(go.GetComponent<Planet>());
        }
    }

    void Update() {
        var playerPlanetsCount = planets.Count(p => p.Owner == Owner.Player);
        var enemyPlanetsCount = planets.Count(p => p.Owner == Owner.Ai);
        var noonePlanetsCount = planets.Count(p => p.Owner == Owner.None);

        if (playerPlanetsCount == 0 && enemyPlanetsCount == 0) {
            GameDraw();
        }
        else if (enemyPlanetsCount == 0) {
            GameWin();
        }
        else if (playerPlanetsCount == 0) {
            GameOver();
        }
    }

    private void GameDraw() {
        Debug.Log("That's a draw!");
        ;
    }

    private void GameOver() {
        Debug.Log("Game Over man!");
        losePanel.SetActive(true);

        //AudioManager.instance.Play(SoundType.GAME_LOST);
    }

    private void GameWin() {
        Debug.Log("You win!");
        winPanel.SetActive(true);

        //AudioManager.instance.Play(SoundType.GAME_WON);
    }
}