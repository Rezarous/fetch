using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public bool gameActive = true;
    public GameObject gameOver;
    public GameObject victory;
    public GameObject startScreen;
    public GameObject introScreen;
    MeteorManager meteorManager;
    float victoryTime;
    bool finalSalvoFired = false;

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

    float startTime;

    void Start() {
        meteorManager = gameObject.GetComponent<MeteorManager>();

        currentWrench = InstantiateTool(wrenchPrefab, wrenchBase);
        currentTape = InstantiateTool(tapePrefab, tapeBase);
        currentFireEx = InstantiateTool(fireExPrefab, fireExBase);
    }

    public void PressStart() {
        meteorManager.Begin();
        startScreen.SetActive(false);
        introScreen.SetActive(true);
        startTime = Time.time;
    }

    void Update() {
        if (Time.time > startTime + 10) {
            introScreen.SetActive(false);
        }
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

        if (finalSalvoFired == false && meteorManager.salvosRemaining == 0) {
            finalSalvoFired = true;
            victoryTime = Time.time + 20;
        }

        if (Time.time > victoryTime && gameActive && finalSalvoFired) {
            Victory();
        }
    }

    void Victory() {
        victory.SetActive(true);
        gameActive = false;
        player.GoodBoy();
    }

    public void GameOver() {
        gameOver.SetActive(true);
        gameActive = false;
    }

    GameObject InstantiateTool(GameObject obj, GameObject obj2){
        return Instantiate(obj, obj2.transform.position, Quaternion.identity);
    }
}
