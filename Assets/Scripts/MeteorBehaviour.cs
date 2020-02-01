using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MeteorBehaviour : MonoBehaviour
{

    public float speed;
    public GameObject firePrefab;

    public AudioClip shipHitSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero - gameObject.transform.position;
        gameObject.transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col) {
        // print("Trigger from meteor!");
        if(col.tag == "Wall") {
            Instantiate(firePrefab, gameObject.transform.position, Quaternion.identity);
            ShakeCamera();
            GameObject.FindGameObjectWithTag("Spaceship").GetComponent<Ship>().Damage();
            AudioSource.PlayClipAtPoint(shipHitSound, transform.position);
            Destroy(gameObject);
        } else if (col.tag == "Player") {
            col.GetComponent<PlayerScript>().Damage();
            ShakeCamera();
            Destroy(gameObject);
        } else if (col.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
            col.GetComponent<DetachableObjectBehaviour>().isDamaged = true;
            ShakeCamera();
            Destroy(gameObject);
        }
    }


    void ShakeCamera() {
        Camera.main.GetComponent<CameraShake>().shakeDuration = 0.7f;
    }
}
