using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_CorruptionDetection : MonoBehaviour
{
    public ELC_Interact objectToPurify;
    private ELC_Interact ToPurifyInCharManager;

    private void Start()
    {
        objectToPurify = GetComponentInParent<ELC_Interact>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Spirit"))
        {
            ToPurifyInCharManager =  collision.GetComponent<AXD_CharacterMove>().charaManager.ToPurify = objectToPurify;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit") && ToPurifyInCharManager != null && ToPurifyInCharManager == objectToPurify)
        {
            collision.GetComponent<AXD_CharacterMove>().charaManager.ToPurify = null;
        }
    }
}
