using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBackground : MonoBehaviour {
    public float rotationSpeed = 1.0f;

    void Start() {
    }

    void FixedUpdate() {
        transform.Rotate(0, rotationSpeed * Time.fixedDeltaTime, 0);
    }
}
