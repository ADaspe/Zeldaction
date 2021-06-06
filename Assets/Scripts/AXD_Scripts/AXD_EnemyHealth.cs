using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_EnemyHealth : MonoBehaviour
{
    public ELC_EnemyAI enemyAI;
    public Material deathShader;
    public SpriteRenderer sr;
    private Animator anims;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anims = this.GetComponent<Animator>();
    }

    public bool GetHit(float timeToStun)
    {
        if (!enemyAI.isStunned && !enemyAI.isProtected)
        {
            if (enemyAI.type == ELC_EnemyAI.EnemyType.BASIC)
            {
                enemyAI.gameMana.audioManager.Play("Basic_Paralyzed");
            }else if (enemyAI.type == ELC_EnemyAI.EnemyType.SHIELD)
            {
                enemyAI.gameMana.audioManager.Play("DS_Paralyzed");
            }
            enemyAI.isStunned = true;
            anims.SetBool("isStun", true);
            Invoke("CancelStun", timeToStun);
            return true;
        }
        return false;
    }

    private void CancelStun()
    {
        enemyAI.isStunned = false;
        anims.SetBool("isStun", false);
    }

    public void Pacificate()
    {
        enemyAI.gameMana.audioManager.Play("Pacification");
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
        CancelInvoke();
        if ( enemyAI.type == ELC_EnemyAI.EnemyType.SHIELD)
        {
            enemyAI.gameMana.audioManager.Play("DS_Pacification");
        }
        sr.material = deathShader;
        yield return new WaitForSeconds(1f/*Mettre le vrai temps de shader de dissolution*/);
        Destroy(this.gameObject);
    }
    
}
