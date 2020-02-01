﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorManager : MonoBehaviour
{

    public GameObject meteorPrefab;
    private GameObject currentMeteor;
    float nextSalvoTime;
    int nextSalvoAmount;

    void Start() {
        nextSalvoTime = Time.time + Random.Range(5.0f, 10.0f);
        nextSalvoAmount = 1;
    }

    void Update() {
        if (Time.time > nextSalvoTime) {
            SpawnSalvo();
        }
    }

    void SpawnSalvo() {
        for (int i = 0; i < nextSalvoAmount; i += 1) {
            SpawnMeteor();
        }
        nextSalvoAmount = Random.Range(nextSalvoAmount, nextSalvoAmount + 2);
        nextSalvoTime = Time.time + Random.Range(20.0f, 40.0f);
    }

    void SpawnMeteor(){
        float radius = Random.Range(12,20);
        float angle = Random.Range(0, Mathf.PI*2);
        currentMeteor = Instantiate(meteorPrefab, OnCircle(radius, angle), Quaternion.identity);
    }

    Vector3 OnCircle(float r, float angle) {
        Vector3 pose = new Vector3(Mathf.Sin(angle)*r, Mathf.Cos(angle)*r, 0);
        return pose;
    }
}
