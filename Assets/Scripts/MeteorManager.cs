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
        if(Input.GetMouseButtonDown(0)){
            SpawnMeteor();
        }   
    }

    void SpawnMeteor(){
        float radius = Random.Range(12,20);
        float angle = Random.Range(0, Mathf.PI*2);
        currentMeteor = Instantiate(meteorPrefab, OnCircle(radius, angle), Quaternion.identity);
    }

    Vector3 OnCircle(float r, float angle) {
        Vector3 pose = new Vector3(Mathf.Sin(angle)*r, Mathf.Cos(angle)*r, 0);
        return pose;
    }

}
