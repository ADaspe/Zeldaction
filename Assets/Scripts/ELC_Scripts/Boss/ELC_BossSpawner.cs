using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossSpawner : MonoBehaviour
{
    public GameObject BossGO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ryn")) BossGO.SetActive(true);
        BossGO.GetComponent<ELC_BossManager>().SwitchPhase();
        this.GetComponent<Collider2D>().enabled = false;
        
    }
}
