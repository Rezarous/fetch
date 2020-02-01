using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
    public bool gameActive = true;
    public Slider healthBar;
    public GameObject gameOver;

    float shipDamage = 0.0f;

    void Start() {
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.J)) {
            DamageShip();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            RepairShip();
        }
    }

    void DamageShip() {
        shipDamage += 0.1f;
        healthBar.value = shipDamage;
        if (shipDamage >= 1.0f) {
            gameOver.SetActive(true);
            gameActive = false;
        }
    }

    void RepairShip() {
        if (shipDamage >= 0.1f) {
            shipDamage -= 0.1f;
        }
        healthBar.value = shipDamage;
    }
}
