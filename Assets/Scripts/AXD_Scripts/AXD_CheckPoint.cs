using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_CheckPoint : MonoBehaviour
{
    public int checkPointIndex;
    [HideInInspector]
    public SpriteRenderer parentSR;

    private void Start()
    {
        parentSR = GetComponentInParent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ryn"))
        {
            //Appliquer un feedback
            ELC_CharacterManager tempManager = collision.GetComponent<AXD_CharacterMove>().charaManager;
            if(tempManager.lastCheckPoint != this)
            {
                GameObject.FindObjectOfType<AudioManager>().Play("Autel");
            }

            tempManager.lastCheckPoint = this;
        }
    }

    public Vector2 GetSpawnPosition()
    {
        return transform.position;
    }
}
