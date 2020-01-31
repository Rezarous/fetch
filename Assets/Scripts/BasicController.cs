using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{

    public float speed;

    Quaternion initialRot;

    // Start is called before the first frame update
    void Start()
    {
        initialRot = transform.rotation;   
    }

    // Update is called once per frame
    void Update()
    {
        // x = transform.position.x;
        // y = transform.position.y;

        if (Input.GetKey(KeyCode.A)){
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D)){
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W)){
            transform.position += Vector3.up * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S)){
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    void LateUpdate() {
        transform.rotation = initialRot;
    }
}
