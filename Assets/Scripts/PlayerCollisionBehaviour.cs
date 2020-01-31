using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionBehaviour : MonoBehaviour
{
    public GameObject allCollectables;

    GameObject activeItem;
    GameObject player;
    bool isCarryingItem = false;
    bool isWithinACollectable = false;

    GameObject pickableItem;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.transform.parent.gameObject;
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
    }

    void OnTriggerStay2D(Collider2D obj) {
        isWithinACollectable = true;
        pickableItem = obj.transform.gameObject;
    }

    void OnTriggerExit2D(Collider2D obj){
        isWithinACollectable = false;
    }

    void PickUpItem(GameObject obj) {
        activeItem = obj.transform.parent.gameObject;
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
        activeItem.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
        isCarryingItem = false;
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
