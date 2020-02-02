using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBehaviour : MonoBehaviour
{

    public int health;
    public bool isFinished = false;

    public Sprite fire2;
    public Sprite fire1;
    public Sprite tape2;
    public Sprite tape1;

    // Start is called before the first frame update
    void OnEnable()
    {
        print(gameObject.name + " Instantiated");
        if(GetComponent<TypeManager>().type == TypeManager.Type.FireEx){
            health = 300;
        } else if(GetComponent<TypeManager>().type == TypeManager.Type.Tape){
            health = 300;
        } else {
            health = 100;
        }
        
        isFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0){
            isFinished = true;
        }
        ChangeSprite();
    }

    public void UseAndReduce() {
        if(GetComponent<TypeManager>().type == TypeManager.Type.Wrench){
            // Nothing :)
        } else {
            health--;
        }
        
    }

    public void DestroyTool() {
        Destroy(gameObject);
    }

    public void ChangeSprite() {
        if(GetComponent<TypeManager>().type == TypeManager.Type.FireEx){
            if(health <= 200 && health > 100){
                GetComponent<SpriteRenderer>().sprite = fire2;
            } else if(health <= 100 ){
                GetComponent<SpriteRenderer>().sprite = fire1;
            }
        }

        if(GetComponent<TypeManager>().type == TypeManager.Type.Tape){
            if(health <= 200 && health > 100){
                GetComponent<SpriteRenderer>().sprite = tape2;
            } else if(health <= 100 ){
                GetComponent<SpriteRenderer>().sprite = tape1;
            }
        }

    }

}
