using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Tether : MonoBehaviour
{
    public bool tethered = false;
    PlayerScript player;

    Rigidbody2D myRb;

    public AudioClip connectSound;
    public AudioClip disconnectSound;

    void Start() {
        myRb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        tethered = true;
        player.SetRail(col.gameObject);
        AudioSource.PlayClipAtPoint(connectSound, transform.position);
        gameObject.SetActive(false);
    }

    public void DetachTether() {
        if (tethered) {
            tethered = false;
            AudioSource.PlayClipAtPoint(disconnectSound, transform.position);
        }
    }
}
