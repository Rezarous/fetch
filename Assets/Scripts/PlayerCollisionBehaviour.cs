using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionBehaviour : MonoBehaviour
{
    public GameObject allCollectables;
    public bool isCarryingItem = false;


    GameObject activeItem;
    GameObject player;
    GameObject pickableItem;
    GameObject currentDamage;
    
    bool isWithinACollectable = false;
    bool isItemAllowed = false;
    
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
        if(obj.tag == "Collectable"){
            isWithinACollectable = true;
            pickableItem = obj.transform.gameObject;
        }
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
        PlaceItemCorrectly(activeItem);
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
        isItemAllowed = ItemCheck();
        if(currentDamage != null && isItemAllowed){
            print(item.name + " is being used for " + currentDamage.name);
            currentDamage.GetComponent<DamageController>().ReduceHealth();
        }
    }

    bool ItemCheck(){
        if(currentDamage == null){
            return false;
        }
        else if (activeItem.name == "FireEx" && currentDamage.name == "Fire"){
            return true;
        }
        else if (activeItem.name == "Wrench" && currentDamage.name == "Bolt"){
            return true;
        }
        return false;
    }



    void PlaceItemCorrectly(GameObject obj){
        obj.transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
    }


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
