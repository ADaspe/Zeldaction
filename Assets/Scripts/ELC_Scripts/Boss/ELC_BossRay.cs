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
    public ParticleSystem PrepareAttackPS;
    public ParticleSystem AttackPS;
    SpriteRenderer SpriteRender;
    Collider2D col;

    private void Awake()
    {
        SpriteRender = this.GetComponent<SpriteRenderer>();
        col = this.GetComponent<Collider2D>();

        col.enabled = false;
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(timeBeforeSpawn);
        PrepareAttackPS.Play();
        Debug.Log("Attack " + Time.time);
        yield return new WaitForSeconds(timeBeforeDespawn);
        col.enabled = true;
        AttackPS.Play();
        yield return new WaitForSeconds(0.2f);
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ryn")) collision.GetComponent<AXD_Health>().GetHit();
    }
}
