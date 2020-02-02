using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{

    public GameObject player;
    public GameObject indicator;
    public GameObject damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAndDrawRay(damage);
    }

    void CalculateAndDrawRay(GameObject obj){

        Vector2 origin = new Vector2(player.transform.position.x, player.transform.position.y);
        int layerMask = 1 << 12;
        Vector3 direction = obj.transform.position - player.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 500.0f, layerMask);

        if (hit.collider != null)
        {
            if(hit.collider.tag == "ScreenEdge") { 
                Debug.Log( hit.collider.name + "  " + hit.point);
                if(hit.point != new Vector2(player.transform.position.x, player.transform.position.y)){
                    // Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), hit.point, Quaternion.identity);
                    //indicator.transform.position = hit.point;
                    obj.GetComponent<DamageController>().ShowIndicator(hit.point);
                } 
            }
        }

    }

}
