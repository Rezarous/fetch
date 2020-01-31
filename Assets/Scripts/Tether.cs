using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    public bool tethered = false;

    void Start() {
    }

    void Update() {
    }

    void OnTriggerEnter2D(Collider2D col) {
        tethered = true;
        SliderJoint2D joint = gameObject.AddComponent<SliderJoint2D>();
        joint.autoConfigureAngle = false;
        joint.angle = col.transform.eulerAngles.z;
        joint.connectedAnchor = col.transform.position;
    }

    public void DetachTether() {
        tethered = false;
        Destroy(gameObject.GetComponent<SliderJoint2D>());
    }
}
