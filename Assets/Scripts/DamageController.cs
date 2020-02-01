using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public int damage;
    Color color;
    float r,g,b;
    float a;
    Quaternion initialRot;
    Ship ship;

    void OnEnable() {
        ship = GameObject.FindGameObjectWithTag("Spaceship").GetComponent<Ship>();
        initialRot = transform.rotation;
        if(gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Fire) {
            ship.Damage();
            damage = 100;
        } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
            damage = 0;
        } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Damageable) {
            damage = 0;
        }
    }
    
    // Called from the player
    public void ReduceDamage() {
        damage--;
        if(damage > 0) {
            if(gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Fire) {
                FadeFireAway();
            } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
                FixDetachable();
            } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Damageable) {
                FixDetachable();
            }
        }else if(damage <= 0) {
            ship.Repair();
            if(gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Fire) {
                Destroy(gameObject);
            } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
                GetComponent<DetachableObjectBehaviour>().MakeHealty();
                transform.rotation = initialRot;
            } else if (gameObject.GetComponent<TypeManager>().type == TypeManager.Type.Damageable) {
                GetComponent<DetachableObjectBehaviour>().MakeHealty();
                transform.rotation = initialRot;
            }
        }
    }

    public void MakeDamaged() {
        damage = 100;
        ship.Damage();
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
