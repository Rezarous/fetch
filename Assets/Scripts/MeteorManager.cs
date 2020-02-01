using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorManager : MonoBehaviour
{

    public GameObject meteorPrefab;
    private GameObject currentMeteor;

    // Start is called before the first frame update
    void Start()
    {
        SpawnMeteor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnMeteor(){
        float x = Random.Range(15,20);
        float y = Random.Range(15,20);
        currentMeteor = Instantiate(meteorPrefab, new Vector3(x, y, 0), Quaternion.identity);
    }

}
