using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBehaviour : MonoBehaviour
{

    public int health;
    public bool isFinished = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        print(gameObject.name + " Instantiated");
        if(GetComponent<TypeManager>().type == TypeManager.Type.FireEx){
            health = 500;
        } else if(GetComponent<TypeManager>().type == TypeManager.Type.Tape){
            health = 1000;
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
}
