using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_MushExplosionDetection : MonoBehaviour
{
    public AXD_Mushroom mushroom;
    private void Start()
    {
        mushroom = GetComponent<AXD_Mushroom>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checker si la collision est avec un truc du bon layer
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            mushroom.Explode();
        }
    }
}
