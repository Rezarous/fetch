using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public bool gameActive = true;
    public GameObject gameOver;
    public GameObject victory;
    MeteorManager meteorManager;
    float victoryTime;
    bool finalSalvoFired = false;

    void Start() {
        meteorManager = gameObject.GetComponent<MeteorManager>();
    }

    void Update() {
        if (finalSalvoFired == false && meteorManager.salvosRemaining == 0) {
            finalSalvoFired = true;
            victoryTime = Time.time + 20;
        }

        if (Time.time > victoryTime && gameActive && finalSalvoFired) {
            Victory();
        }
    }

    void Victory() {
        victory.SetActive(true);
        gameActive = false;
    }

    public void GameOver() {
        gameOver.SetActive(true);
        gameActive = false;
    }
}
