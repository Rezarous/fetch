using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject tether;
    public float throwForce = 3.0f;
    public float springForce = 3.0f;

    void Start() {
    }

    void Update() {
        Rigidbody2D myRb = gameObject.GetComponent<Rigidbody2D>();
        Rigidbody2D tetherRb = tether.GetComponent<Rigidbody2D>();

        Vector3 playerPos = transform.position;

        Vector3 tetherPos = tether.transform.position;
        Vector3 tetherDiff = tetherPos - playerPos;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDiff = mousePos - playerPos;
        mouseDiff.z = 0;

        Camera.main.transform.position = playerPos + new Vector3(0, 0, -10);

        if (!tether.activeSelf) {
            tether.transform.position = playerPos;
            if (Input.GetMouseButtonDown(0)) {
                tether.SetActive(true);
                tetherRb.velocity = myRb.velocity;
                tetherRb.AddForce(mouseDiff.normalized * throwForce, ForceMode2D.Impulse);
                myRb.AddForce(-mouseDiff.normalized * throwForce, ForceMode2D.Impulse);
            }
        } else {
            Debug.DrawLine(playerPos, tetherPos, Color.white);
            if (Input.GetMouseButtonDown(0)) {
                tether.GetComponent<Tether>().DetachTether();
                tether.SetActive(false);
            }
        }

        if (tetherDiff.magnitude > 5) {
            tetherRb.AddForce(-tetherDiff.normalized * throwForce, ForceMode2D.Impulse);
            myRb.AddForce(tetherDiff.normalized * throwForce, ForceMode2D.Impulse);
        }

        myRb.AddForce(new Vector3(Input.GetAxis("Horizontal") * 20, Input.GetAxis("Vertical") * 20, 0));
    }
}
