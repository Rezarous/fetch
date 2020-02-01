using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Camera_Controller : MonoBehaviour {

    public Transform player;
    public float horizontalResolution = 1920;
    public float amplifier;

    void OnGUI()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;
        Camera.main.orthographicSize = horizontalResolution / currentAspect / amplifier;
    }

    void LateUpdate()
    {
        if(!GetComponent<CameraShake>().isShaking) {
            transform.position = new Vector3(player.position.x, player.position.y, -10);
        }
        
    }





}

