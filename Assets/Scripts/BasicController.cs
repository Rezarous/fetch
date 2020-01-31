using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{

    float x;
    float y;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        x = transform.position.x;
        y = transform.position.y;

        if (Input.GetKey(KeyCode.A)){
            transform.position = new Vector3(x - speed, y, transform.position.z);
        }

        if (Input.GetKey(KeyCode.D)){
            transform.position = new Vector3(x + speed, y, transform.position.z);
        }

        if (Input.GetKey(KeyCode.W)){
            transform.position = new Vector3(x, y + speed, transform.position.z);
        }

        if (Input.GetKey(KeyCode.S)){
            transform.position = new Vector3(x, y - speed, transform.position.z);
        }
    }
}
