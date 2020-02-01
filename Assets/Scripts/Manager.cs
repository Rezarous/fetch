using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public bool gameActive = true;
    public GameObject gameOver;
    public GameObject victory;

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
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
