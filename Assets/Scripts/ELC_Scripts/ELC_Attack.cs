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
        Debug.Log("Ryn'O'Shield");
        if (CharManager.RynMove.canMove && !ShieldOn)
        {
            if(NextShield <= Time.time)
            {
                RynActivateShield();
            }
        }
        else if (!CharManager.RynMove.canMove && ShieldOn)
        {
            RynLoseShield();
        }
    }

    public void SpiritDashAttack()
    {
        Debug.Log("Spirit Dash");
        StartCoroutine(DashCoroutine());
    }

    public void AttackTogether()
    {
        //Debug.Log("Attack Together");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(this.transform.position, enemyDetectionRadius, gameManager.EnemiesMask);

        GameObject nearestEnemy = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            if(nearestEnemy == null || Vector2.Distance(this.transform.position, enemies[i].transform.position) < Vector2.Distance(this.transform.position, nearestEnemy.transform.position))
            {
                RaycastHit2D wallHit = Physics2D.Raycast(this.transform.position, nearestEnemy.transform.position - this.transform.position, Vector2.Distance(this.transform.position, nearestEnemy.transform.position), gameManager.GlobalObstaclesMask);
                if(wallHit.collider == null) nearestEnemy = enemies[i].gameObject;
            }
        }
        Debug.Log("Ryn attaque " + nearestEnemy);
    }

    public void RynActivateShield()
    {
        CharManager.RynMove.canMove = false;
        CharManager.RynMove.rawInputMovement = Vector2.zero;
        NextShield = Time.time + CharManager.stats.ShieldDuration + CharManager.stats.ShieldCooldown;
        Invoke("RynLoseShield", CharManager.stats.ShieldDuration);
    }

    public void RynLoseShield()
    {
        if (ShieldOn)
        {
            NextShield = Time.time + CharManager.stats.ShieldCooldown;
            ShieldOn = false;
            CharManager.RynMove.canMove = true;
        }
    }

    public IEnumerator DashCoroutine()
    {
        CharManager.spiritMove.isDashing = true;
        CharManager.spiritMove.rb.velocity = CharManager.spiritMove.LastDirection * (CharManager.stats.DashDistance / CharManager.stats.DashTime);
        yield return new WaitForSeconds(CharManager.stats.DashTime);
        CharManager.spiritMove.speed = CharManager.stats.SpiritSpeed;
        CharManager.spiritMove.isDashing = false;
    }
}




