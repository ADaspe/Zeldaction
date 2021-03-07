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

    public void MiaShield()
    {
        Debug.Log("MiaShield");
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
            if(nearestEnemy == null || Vector2.Distance(this.transform.position, enemies[i].transform.position) < Vector2.Distance(this.transform.position, nearestEnemy.transform.position))
            {
                RaycastHit2D wallHit = Physics2D.Raycast(this.transform.position, nearestEnemy.transform.position - this.transform.position, Vector2.Distance(this.transform.position, nearestEnemy.transform.position), gameManager.GlobalObstaclesMask);
                if(wallHit.collider == null) nearestEnemy = enemies[i].gameObject;
            }
        }
        Debug.Log("Mia attaque " + nearestEnemy);
    }
}
