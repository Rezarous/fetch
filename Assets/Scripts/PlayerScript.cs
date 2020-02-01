using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PlayerScript : MonoBehaviour {
    public Manager manager;
    public GameObject allCollectables;
    public bool isCarryingItem = false;

    public GameObject tether;
    public float throwForce = 5.0f;
    public float springForce = 10.0f;
    public float tetherLength = 4.0f;

    public float attachedThrust = 200.0f;
    public float detachedThrust = 5.0f;

    public bool inside = true;
    public float insideDrag = 0.1f;

    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip pickup;

    public float health = 1.0f;
    public Slider healthBar;

    GameObject activeItem;
    GameObject player;
    GameObject pickableItem;
    GameObject currentDamage;

    Rigidbody2D myRb;
    Rigidbody2D tetherRb;

    
    bool isWithinACollectable = false;
    bool isItemAllowed = false;

    void Start() {
        player = gameObject;
        myRb = player.GetComponent<Rigidbody2D>();
        tetherRb = tether.GetComponent<Rigidbody2D>();

        healthBar.value = health;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            if(isWithinACollectable && !isCarryingItem){
                PickUpItem(pickableItem);
            }else if(isWithinACollectable && isCarryingItem){
                SwapItem();
            }else if(isCarryingItem){
                DropItem();
            }
        }

        if(Input.GetKey(KeyCode.F) && isCarryingItem) {
            UseItem(activeItem);
        }
    }

    void FixedUpdate() {
        Vector3 tetherDiff = tether.transform.position - transform.position;

        Camera.main.transform.position = transform.position + new Vector3(0, 0, -10);

        if (!tether.activeSelf) {
            tether.transform.position = transform.position;
            if (Input.GetMouseButtonDown(0)) {
                ThrowTether();
            }
        } else {
            Debug.DrawLine(transform.position, tether.transform.position, Color.white);
            if (Input.GetMouseButtonDown(0)) {
                tether.GetComponent<Tether>().DetachTether();
                tether.SetActive(false);
            }
        }

        if (tetherDiff.magnitude > tetherLength) {
            tetherRb.AddForce(-tetherDiff.normalized * springForce, ForceMode2D.Impulse);
            myRb.AddForce(tetherDiff.normalized * springForce, ForceMode2D.Impulse);
        }

        bool tethered = tether.GetComponent<Tether>().tethered;
        float thrust = tethered || inside ? attachedThrust : detachedThrust;
        if (manager.gameActive) {
            myRb.AddForce(new Vector3(Input.GetAxis("Horizontal") * thrust, Input.GetAxis("Vertical") * thrust, 0));
        }
    }

    void ThrowTether() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDiff = mousePos - transform.position;
        mouseDiff.z = 0;

        tether.SetActive(true);
        tetherRb.velocity = myRb.velocity;
        tetherRb.AddForce(mouseDiff.normalized * throwForce, ForceMode2D.Impulse);
        myRb.AddForce(-mouseDiff.normalized * throwForce, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Damage") {
            currentDamage = other.gameObject;
        } else if (other.tag == "Inside") {
            inside = true;
            myRb.drag = insideDrag;
        }
    }

    void OnTriggerStay2D(Collider2D obj) {
        if(obj.tag == "Collectable") {
            isWithinACollectable = true;
            pickableItem = obj.transform.gameObject;
        }
        if(obj.tag == "Damage") {
            currentDamage = obj.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        isWithinACollectable = false;
        if (other.tag == "Inside") {
            inside = false;
            myRb.drag = 0;
        }
        if (other.tag == "Damage") {
            currentDamage = null;
        }
    }

    void PickUpItem(GameObject obj) {
        AudioSource.PlayClipAtPoint(pickup, transform.position);
        activeItem = obj.gameObject;
        activeItem.transform.parent = player.transform;
        obj.GetComponent<BoxCollider2D>().enabled = false;
        PlaceItemCorrectly(activeItem);
        isCarryingItem = true;
    }
    
    void SwapItem(){
        AudioSource.PlayClipAtPoint(pickup, transform.position);
        DropItem();
        PickUpItem(pickableItem);
    }

    void DropItem(){
        activeItem.transform.parent = allCollectables.transform;
        activeItem.GetComponent<BoxCollider2D>().enabled = true;
        isCarryingItem = false;
    }

    void UseItem(GameObject item){
        isItemAllowed = ItemCheck();
        if(currentDamage != null && isItemAllowed){
            currentDamage.GetComponent<DamageController>().ReduceDamage();
        }
    }

    bool ItemCheck(){
        if(currentDamage == null){
            return false;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.FireEx && 
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Fire){
            return true;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.Wrench && 
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Detachable){
            return true;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.Tape && 
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Damageable){
            return true;
        }
        return false;
    }

    void PlaceItemCorrectly(GameObject obj){
        obj.transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
    }

    public void Damage() {
        health -= 0.34f;
        healthBar.value = health;
        if (health <= 0) {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            manager.GameOver();
        } else {
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        }
    }
}
