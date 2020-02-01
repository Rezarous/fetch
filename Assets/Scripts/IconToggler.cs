using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconToggler : MonoBehaviour {
    public Text powerIcon;
    public PlayerScript player;
    public Tether tether;

    void Start() {
    }

    void Update() {
        bool isPowered = player.inside || tether.tethered;
        powerIcon.enabled = isPowered;
    }
}
