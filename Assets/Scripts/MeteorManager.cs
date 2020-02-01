using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorManager : MonoBehaviour
{

    public GameObject meteorPrefab;
    private GameObject currentMeteor;
    float lastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        SpawnMeteor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastSpawn + 5.0) {
            SpawnMeteor();
        }
    }

    void SpawnMeteor(){
        float x = Random.Range(10,25);
        float y = Random.Range(10,25);
        currentMeteor = Instantiate(meteorPrefab, new Vector3(x, y, 0), Quaternion.identity);
        lastSpawn = Time.time;
    }

}
