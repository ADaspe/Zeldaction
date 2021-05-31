using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossRay : MonoBehaviour
{
    public int Index;

    [HideInInspector]
    public float timeBeforeSpawn;
    [HideInInspector]
    public float timeBeforeDespawn;
    SpriteRenderer SpriteRender;
    Collider2D col;

    private void Awake()
    {
        SpriteRender = this.GetComponent<SpriteRenderer>();
        col = this.GetComponent<Collider2D>();

        SpriteRender.enabled = false;
        col.enabled = false;
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(timeBeforeSpawn);
        SpriteRender.enabled = true;
        col.enabled = true;
        yield return new WaitForSeconds(timeBeforeDespawn);
        SpriteRender.enabled = false;
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ryn")) collision.GetComponent<AXD_Health>().GetHit();
    }
}
