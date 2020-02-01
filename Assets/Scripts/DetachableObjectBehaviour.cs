using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachableObjectBehaviour : MonoBehaviour
{
    public bool isDamaged;
    public Sprite healthySprite;
    public Sprite damagedSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDamaged){
            GetComponent<SpriteRenderer>().sprite = healthySprite;
        } else {
            GetComponent<SpriteRenderer>().sprite = damagedSprite;
        }
    }

    public void MakeHealty() {
        GetComponent<SpriteRenderer>().sprite = healthySprite;
        isDamaged = false;
    }

    public void MakeDamaged() {
        GetComponent<SpriteRenderer>().sprite = damagedSprite;
        isDamaged = true;
    }
}
