﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehaviour : MonoBehaviour
{

    public float speed;
    public GameObject firePrefab;

    Vector3 initialPose;

    void OnEnable(){
        initialPose = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero - initialPose;
        gameObject.transform.position += direction * speed * Time.deltaTime;
        RotateCorrectly();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Wall") {
            Instantiate(firePrefab, gameObject.transform.position, Quaternion.identity);
            ShakeCamera();
            Destroy(gameObject);
        } else if (col.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
            col.GetComponent<DetachableObjectBehaviour>().MakeDamaged();
            ShakeCamera();
            Destroy(gameObject);
        }
    }

    void RotateCorrectly(){
        Vector3 moveDirection = Vector3.zero - initialPose; 
        if (moveDirection != Vector3.zero) 
        {
            float meteorAngle = (Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg) + 135;
            transform.rotation = Quaternion.AngleAxis(meteorAngle, Vector3.forward);
        }
    }


    void ShakeCamera() {
        Camera.main.GetComponent<CameraShake>().shakeDuration = 0.7f;
    }
}
