using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour {
    public Slider healthBar;
    public Manager manager;
    float shipHealth = 1.0f;

    public AudioClip damageWarningSound;
    public AudioClip lowHealthWarningSound;
    public AudioClip playerWarningSound;
    public AudioClip playerWarningSoundSevere;

    void Start() {
        healthBar.value = shipHealth;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.J)) {
            AudioHelper.PlayInside(damageWarningSound);
            AudioHelper.PlayOutside(playerWarningSound);
            Damage();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            Repair();
        }
    }

    public void Damage() {
        shipHealth -= 0.1f;
        healthBar.value = shipHealth;
        if (shipHealth <= 0.0f) {
            manager.GameOver();
        } else if (shipHealth <= 0.2) {
            AudioHelper.PlayInside(lowHealthWarningSound);
            AudioHelper.PlayOutside(playerWarningSound);
            AudioHelper.PlayOutside(playerWarningSoundSevere);
        }
    }

    public void Repair() {
        if (shipHealth <= 0.9f) {
            shipHealth += 0.1f;
        }
        healthBar.value = shipHealth;
    }
}
