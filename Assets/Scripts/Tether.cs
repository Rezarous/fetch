using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    public bool tethered = false;
    PlayerScript player;

    Rigidbody2D myRb;

    void Start() {
        myRb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        tethered = true;
        player.SetRail(col.gameObject);
    }

    public void DetachTether() {
        if (tethered) {
            tethered = false;
        }
    }
}
