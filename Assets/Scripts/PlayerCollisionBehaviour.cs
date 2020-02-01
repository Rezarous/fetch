using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionBehaviour : MonoBehaviour
{
    public GameObject allCollectables;

    GameObject activeItem;
    GameObject player;
    public bool isCarryingItem = false;
    bool isWithinACollectable = false;

    GameObject pickableItem;
    GameObject currentDamage;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
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

    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.tag == "Damage") {
            currentDamage = obj.gameObject;
        }
    }

    void OnTriggerStay2D(Collider2D obj) {
        isWithinACollectable = true;
        pickableItem = obj.transform.gameObject;
        if(obj.tag == "Damage") {
            currentDamage = obj.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D obj){
        isWithinACollectable = false;
        if(obj.tag == "Damage") {
            currentDamage = null;
        }
    }

    void PickUpItem(GameObject obj) {
        activeItem = obj.gameObject;
        activeItem.transform.parent = player.transform;
        obj.GetComponent<BoxCollider2D>().enabled = false;
        // PlaceItemCorrectly(activeItem);
        isCarryingItem = true;
    }
    
    void SwapItem(){
        DropItem();
        PickUpItem(pickableItem);
    }

    void DropItem(){
        activeItem.transform.parent = allCollectables.transform;
        activeItem.GetComponent<BoxCollider2D>().enabled = true;
        isCarryingItem = false;
    }

    void UseItem(GameObject item){
        if(currentDamage != null){
            print(item.name + " is being used for " + currentDamage.name);
            currentDamage.GetComponent<DamageController>().ReduceHealth();
        }
        //print("using item");
    }

    // void PlaceItemCorrectly(GameObject obj){
    //     obj.transform.position = transform.position;
    // }


    // void OnTriggerEnter2D(Collider2D col)
    // {
    //     Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    //     //spriteMove = -0.1f;
    //     if(col.transform.parent.tag == "Collectable"){
    //         //print("Hit a collectable");
    //         activeItem = col.transform.parent.gameObject;
    //         activeItem.transform.parent = player.transform;
    //     }
    // }
    
}
