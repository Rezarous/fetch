using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public int damage;
    Color color;
    float r, g, b;
    float a;
    Quaternion initialRot;
    Ship ship;

    public AudioClip reAttachSound;
    public AudioClip repairSound;

    void OnEnable() {
        ship = GameObject.FindGameObjectWithTag("Spaceship").GetComponent<Ship>();
        initialRot = transform.localRotation;

        switch(gameObject.GetComponent<TypeManager>().type) {
            case TypeManager.Type.Fire:
                ship.Damage();
                damage = 100;
                break;
            case TypeManager.Type.Detachable:
            case TypeManager.Type.Damageable:
                damage = 0;
                break;
        }
    }

    // Called from the player
    public void ReduceDamage() {
        damage--;

        switch (gameObject.GetComponent<TypeManager>().type) {
            case TypeManager.Type.Fire:
                if (damage > 0) {
                    FadeFireAway();
                }
                else {
                    Destroy(gameObject);
                }
                break;
            case TypeManager.Type.Detachable:
            case TypeManager.Type.Damageable:
                if (damage > 0) {
                    FixDetachable();
                }
                else {
                    if (reAttachSound != null) {
                        AudioHelper.PlayInside(reAttachSound);
                        AudioHelper.PlayOutside(reAttachSound);
                    }
                    GetComponent<DetachableObjectBehaviour>().MakeHealty();
                    transform.localRotation = initialRot;
                }
                break;
        }

        if (repairSound != null) {
            AudioHelper.PlayInside(repairSound);
            AudioHelper.PlayOutside(repairSound);
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
        a = damage / 200.0f;
        color = new Color(r, g, b, a);
        GetComponent<SpriteRenderer>().color = color;
    }

}
