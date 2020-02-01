using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public GameObject wrenchBase;
    public GameObject tapeBase;
    public GameObject fireExBase;

    public GameObject wrenchPrefab;
    public GameObject tapePrefab;
    public GameObject fireExPrefab;

    public PlayerScript player;

    GameObject currentWrench;
    GameObject currentTape;
    GameObject currentFireEx;
    GameObject currentActiveItem;

    // Start is called before the first frame update
    void Start()
    {
        currentWrench = InstantiateTool(wrenchPrefab, wrenchBase);
        currentTape = InstantiateTool(tapePrefab, tapeBase);
        currentFireEx = InstantiateTool(fireExPrefab, fireExBase);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWrench.GetComponent<ToolBehaviour>().isFinished) {
            player.ThisItemDied(currentWrench.GetComponent<TypeManager>().type);
            currentWrench = InstantiateTool(wrenchPrefab, wrenchBase);
        } else if(currentTape.GetComponent<ToolBehaviour>().isFinished) {
            player.ThisItemDied(currentTape.GetComponent<TypeManager>().type);
            currentTape = InstantiateTool(tapePrefab, tapeBase);
        } else if(currentFireEx.GetComponent<ToolBehaviour>().isFinished) {
            player.ThisItemDied(currentFireEx.GetComponent<TypeManager>().type);
            currentFireEx = InstantiateTool(fireExPrefab, fireExBase);
        }

        // if(player.activeItem != null){
        //     // activeItem = player.activeItem;
        //     // if(activeItem.GetComponent<ToolBehaviour>().isFinished){
        //     // TypeManager.Type thisType = activeItem.GetComponent<TypeManager>().type;
        //     // player.ThisItemDied(thisType);
        //     // if(thisType == TypeManager.Type.Wrench) {
        //     //     currentWrench = InstantiateTool(wrenchPrefab, wrenchBase);
        //     // } else if (thisType == TypeManager.Type.Tape) {
        //     //     currentTape = InstantiateTool(tapePrefab, tapeBase);
        //     // } else if (thisType == TypeManager.Type.FireEx) {
        //     //     currentFireEx = InstantiateTool(fireExPrefab, fireExBase);
        //     // }
            
        //     // }
        // }
        
    }

    GameObject InstantiateTool(GameObject obj, GameObject obj2){
        return Instantiate(obj, obj2.transform.position, Quaternion.identity);
    }
}
