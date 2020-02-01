using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    int damage = 100;
    Color color;
    float r,g,b;
    float a;

    // Start is called before the first frame update
    void Start()
    {
        // health = 100;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    // Called from the player
    public void ReduceDamage() {
        damage--;
        if(damage > 0) {
            if(gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Fire) {
                FadeFireAway();
            } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
                FixDetachable();
            }
        }else if( damage <= 0) {
            if(gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Fire) {
                Destroy(gameObject);
            } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
                GetComponent<DetachableObjectBehaviour>().MakeHealty();
            }
        }
    }

    void FixDetachable() {
        transform.RotateAround(transform.position, transform.forward, 3.6f);
    }

    void FadeFireAway() {
        color = GetComponent<SpriteRenderer>().color;
        r = color.r;
        g = color.g;
        b = color.b;
        a = damage/200.0f;
        color = new Color(r, g, b, a);
        GetComponent<SpriteRenderer>().color = color;
    }

}
