using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float rotationSpeed = 1.0f;

    void Start() {
    }

    void FixedUpdate() {
        transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
    }
}
