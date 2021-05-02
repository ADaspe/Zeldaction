using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Attack : MonoBehaviour
{
    public AXD_CharacterVariablesSO CharStats;
    [SerializeField]
    ELC_CharacterManager CharManager;
    [SerializeField]
    ELC_GameManager gameManager;
    //public float enemyDetectionRadius;
    public bool ShieldOn;
    public float NextShield;
    private bool spiritAttack;
    private GameObject nearestEnemy;
    private ELC_Attack SpiritAttackScript;
    public float attackTogetherCooldown;
    public LayerMask defaultMask;
    public LayerMask dashMask;


    private void Start()
    {
        SpiritAttackScript = CharManager.SpiritGO.GetComponent<ELC_Attack>();
    }
    private void FixedUpdate()
    {
        if(spiritAttack)
        {
            SpiritAttackScript.SpiritAttackTogether(nearestEnemy.transform.position, 0.05f);
        }
    }

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
        if (attackTogetherCooldown > Time.time) return;

        attackTogetherCooldown = Time.time + CharManager.stats.AttackCooldown;
        //Debug.Log("Attack Together");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(this.transform.position, CharStats.AttackTogetherRange, gameManager.EnemiesMask);

        
        for (int i = 0; i < enemies.Length; i++)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(this.transform.position, enemies[i].transform.position - this.transform.position, Vector2.Distance(this.transform.position, enemies[i].transform.position), gameManager.GlobalObstaclesMask);

            if (i == 0 && wallHit.collider == null) nearestEnemy = enemies[i].gameObject;
            else if (wallHit.collider == null && nearestEnemy != null && Vector2.Distance(this.transform.position, enemies[i].transform.position) < Vector2.Distance(this.transform.position, nearestEnemy.transform.position))
            {
                nearestEnemy = enemies[i].gameObject;
            }
        }
        if (nearestEnemy != null)
        {
            spiritAttack = true;
            StartCoroutine(ResetAfterSeconds(0.05f));
            Debug.Log("Ryn attaque " + nearestEnemy);
        }
    }

    public void RynActivateShield()
    {
        Debug.Log("Shield On !");
        ShieldOn = true;
        CharManager.RynMove.canMove = false;
        CharManager.RynMove.rawInputMovement = Vector2.zero;
        NextShield = Time.time + CharManager.stats.ShieldDuration + CharManager.stats.ShieldCooldown;
        Invoke("RynLoseShield", CharManager.stats.ShieldDuration);
    }

    public void RynLoseShield()
    {
        Debug.Log("Lose Shield");
        if (ShieldOn)
        {
            NextShield = Time.time + CharManager.stats.ShieldCooldown;
            ShieldOn = false;
            CharManager.RynMove.canMove = true;
        }
    }

    public IEnumerator DashCoroutine()
    {
        gameObject.layer = dashMask;
        CharManager.spiritMove.wasDashingWhenColliding = true;
        CharManager.spiritMove.isDashing = true;
        CharManager.spiritMove.rb.velocity = CharManager.spiritMove.LastDirection * (CharManager.stats.DashDistance / CharManager.stats.DashTime);
        yield return new WaitForSeconds(CharManager.stats.DashTime);
        CharManager.spiritMove.currentSpeed = CharManager.stats.SpiritSpeed;
        CharManager.spiritMove.isDashing = false;
        CharManager.spiritMove.wasDashingWhenColliding = false;
        gameObject.layer = defaultMask;
    }

    public void SpiritAttackTogether(Vector3 targetPos, float duration)
    {
        Vector3 direction = targetPos - this.gameObject.transform.position;
        direction = direction.normalized * (direction.magnitude / duration);
        this.transform.Translate(direction * Time.deltaTime);
    }

    private IEnumerator ResetAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        spiritAttack = false;
        nearestEnemy = null;
    }
}




