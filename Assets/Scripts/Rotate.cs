﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float rotationSpeed = 0.0f;

    void Start() {
    }

    void FixedUpdate() {
        transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
    }

    void AddRandomSpin() {
        float spinAmount = Random.Range(1.0f, 6.0f);
        bool widdershins = Random.value > 0.5f;
        Debug.Log(widdershins);

        if (widdershins) {
            spinAmount *= -1.0f;
        }

        rotationSpeed += spinAmount;
    }
}
