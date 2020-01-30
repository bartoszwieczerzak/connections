using System.Collections.Generic;
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

    private readonly List<Planet> planets = new List<Planet>();

    void Start() {
        GameObject[] planetGameObjects = GameObject.FindGameObjectsWithTag(Tag.PLANET);

        foreach (GameObject go in planetGameObjects) {
            planets.Add(go.GetComponent<Planet>());
        }
    }

    void Update() {
        foreach (Planet planet in planets) {
            if (planet.Owner == Owner.Player && planet.Units > 0) {
                return;
            }

            GameOver();
        }
    }

    private void GameOver() {
        Debug.Log("Game Over man!");
    }
}