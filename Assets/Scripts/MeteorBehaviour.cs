using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MeteorBehaviour : MonoBehaviour
{

    public float speed;
    public GameObject firePrefab;
    public Transform damagedContainer;

    Vector3 initialPose;


    void OnEnable(){
        initialPose = transform.position;
    }

    public AudioClip[] shipHitSounds;
    public AudioClip[] shipHitSoundsDim;

    void Start() {
        damagedContainer = GameObject.Find("All Damages").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero - initialPose;
        gameObject.transform.position += direction * speed * Time.deltaTime;
        RotateCorrectly();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player") {
            col.GetComponent<PlayerScript>().Damage();
        } 
        else {
            AudioSource.PlayClipAtPoint(shipHitSounds[Random.Range(0, shipHitSounds.Length)], transform.position);
            AudioHelper.PlayInside(shipHitSoundsDim[Random.Range(0, shipHitSoundsDim.Length)]);

            if (col.tag == "Wall") {
                GameObject newFire = Instantiate(firePrefab, gameObject.transform.position, Quaternion.identity);
                newFire.transform.parent = damagedContainer;
            }
            else if (col.GetComponent<TypeManager>()?.type == TypeManager.Type.Detachable) {
                col.GetComponent<DamageController>().MakeDamaged();
                col.GetComponent<DetachableObjectBehaviour>().MakeDamaged();
                ShakeCamera();
                Destroy(gameObject);
            }
            else if (col.GetComponent<TypeManager>()?.type == TypeManager.Type.Damageable) {
                col.GetComponent<DamageController>().MakeDamaged();
                col.GetComponent<DetachableObjectBehaviour>().MakeDamaged();
                ShakeCamera();
                Destroy(gameObject);
            }
            else {
                Debug.LogWarning($"MeteorBehaviour: could not resolve collision");
                Destroy(gameObject);
            }
        }

        ShakeCamera();
        Destroy(gameObject);
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
