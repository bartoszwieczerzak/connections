﻿using System.Collections.Generic;
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

            VisualiseArmyMovement(source, target);
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

            AudioManager.Instance.Play(SoundType.BATTLE_LOST);
        }
        else if (targetPlanet.Units < unitsToSend)
        {
            int toBeAdded = unitsToSend - targetPlanet.Units;
            targetPlanet.Units = 0;
            targetPlanet.AddUnits(toBeAdded);
            Debug.Log("ADDED to TARGET: " + toBeAdded);
            targetPlanet.ChangeOwnership(who);

            AudioManager.Instance.Play(SoundType.PLANET_TAKENOVER);
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

    private void VisualiseArmyMovement(Planet sourcePlanet, Planet targetPlanet)
    {
        sourcePlanet.SendFleet(targetPlanet);

        AudioManager.Instance.Play(SoundType.SENDING_ARMY_PLAYER);
    }
}