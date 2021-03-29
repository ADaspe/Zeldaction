using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Attack : MonoBehaviour
{
    [SerializeField]
    ELC_CharacterManager CharManager;
    [SerializeField]
    ELC_GameManager gameManager;
    public float enemyDetectionRadius;
    public bool ShieldOn;
    public float NextShield;

    public void RynShield()
    {
        Debug.Log("MiaShield");
        if (CharManager.RynMove.canMove && !ShieldOn)
        {
            NextShield = Time.time + CharManager.stats.ShieldDuration + CharManager.stats.ShieldCooldown;
            StartCoroutine(ShieldCoroutine());
        }
        else if (!CharManager.RynMove.canMove && ShieldOn)
        {
            RynLoseShield();
        }
    }

    public void SpiritDashAttack()
    {
        Debug.Log("Spirit Dash");
    }

    public void AttackTogether()
    {
        //Debug.Log("Attack Together");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(this.transform.position, enemyDetectionRadius, gameManager.EnemiesMask);

        GameObject nearestEnemy = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (i == 0) nearestEnemy = enemies[i].gameObject;
            if(nearestEnemy == null || Vector2.Distance(this.transform.position, enemies[i].transform.position) < Vector2.Distance(this.transform.position, nearestEnemy.transform.position))
            {
                RaycastHit2D wallHit = Physics2D.Raycast(this.transform.position, nearestEnemy.transform.position - this.transform.position, Vector2.Distance(this.transform.position, nearestEnemy.transform.position), gameManager.GlobalObstaclesMask);
                if(wallHit.collider == null) nearestEnemy = enemies[i].gameObject;
            }
        }
        Debug.Log("Mia attaque " + nearestEnemy);
    }

    public void RynLoseShield()
    {
        StopCoroutine(ShieldCoroutine());
        NextShield = Time.time + CharManager.stats.ShieldCooldown;
        ShieldOn = false;
        CharManager.RynMove.canMove = true;
    }

    public IEnumerator ShieldCoroutine()
    {
        CharManager.RynMove.canMove = false;
        yield return new WaitForSeconds(CharManager.stats.ShieldDuration);
        CharManager.RynMove.canMove = true;
    }
}




