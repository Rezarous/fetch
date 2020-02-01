using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    int health;
    Color color;
    float r,g,b;
    float a;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(health > 20) {
            TransitionToDeath();
        }else if( health <= 20) {
            Destroy(gameObject);
        }
    }


    void TransitionToDeath() {
        color = GetComponent<SpriteRenderer>().color;
        r = color.r;
        g = color.g;
        b = color.b;
        a = health/100.0f;
        color = new Color(r, g, b, a);
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ReduceHealth() {
        health--;
    }
}
