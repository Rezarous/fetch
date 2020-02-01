using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    public bool tethered = false;

    void Start() {
        SliderJoint2D joint = gameObject.AddComponent<SliderJoint2D>();
        joint.enabled = false;
    }

    void Update() {
    }

    void OnTriggerEnter2D(Collider2D col) {
        BoxCollider2D box = col.GetComponent<BoxCollider2D>();
        tethered = true;
        SliderJoint2D joint = gameObject.GetComponent<SliderJoint2D>();
        joint.autoConfigureAngle = false;
        joint.angle = col.transform.eulerAngles.z;
        joint.connectedAnchor = col.transform.position;
        joint.enabled = true;
        JointTranslationLimits2D limits = joint.limits;
        limits.min = -box.size.x / 2.0f;
        limits.max = box.size.x / 2.0f;
        joint.limits = limits;
        joint.useLimits = true;
    }

    public void DetachTether() {
        if (tethered) {
            tethered = false;
            gameObject.GetComponent<SliderJoint2D>().enabled = false;
        }
    }
}
