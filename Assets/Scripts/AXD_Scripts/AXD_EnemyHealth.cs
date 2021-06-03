using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_EnemyHealth : MonoBehaviour
{
    public ELC_EnemyAI enemyAI;
    public Material deathShader;
    public SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public bool GetHit(float timeToStun)
    {
        if (!enemyAI.isStunned && !enemyAI.isProtected)
        {
            Debug.Log("Bang bang");
            enemyAI.isStunned = true;
            Invoke("CancelStun", timeToStun);
            return true;
        }
        return false;
    }

    private void CancelStun()
    {
        enemyAI.isStunned = false;
    }

    public void Pacificate()
    {
        Debug.Log("Tentative de pacification");
        // Mettre shader de dissolution

        if (enemyAI.isStunned)
        {
            Debug.Log("Pacification");

            StartCoroutine(PacificateCoroutine());
        }
    }
    
    IEnumerator PacificateCoroutine()
    {

        sr.material = deathShader;
        yield return new WaitForSeconds(1f/*Mettre le vrai temps de shader de dissolution*/);
        Destroy(this);
    }
    
}
