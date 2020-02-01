using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Tether : MonoBehaviour
{
    public bool tethered = false;
    public float tetheredDrag = 0.1f;

    Rigidbody2D myRb;
    Collider2D rail;
    SliderJoint2D joint;

    public AudioClip connectSound;
    public AudioClip disconnectSound;

    void Start() {
        joint = gameObject.AddComponent<SliderJoint2D>();
        joint.enabled = false;

        myRb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (rail) {
            joint.angle = rail.transform.eulerAngles.z;
            joint.connectedAnchor = rail.transform.position;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        rail = col;
        BoxCollider2D box = rail.GetComponent<BoxCollider2D>();
        tethered = true;
        joint.autoConfigureAngle = false;
        joint.enabled = true;
        JointTranslationLimits2D limits = joint.limits;
        limits.min = -box.size.x / 2.0f;
        limits.max = box.size.x / 2.0f;
        joint.limits = limits;
        joint.useLimits = true;

        myRb.drag = tetheredDrag;

        AudioSource.PlayClipAtPoint(connectSound, transform.position);
    }

    public void DetachTether() {
        if (tethered) {
            tethered = false;
            gameObject.GetComponent<SliderJoint2D>().enabled = false;
            rail = null;

            AudioSource.PlayClipAtPoint(disconnectSound, transform.position);
        }
    }
}
