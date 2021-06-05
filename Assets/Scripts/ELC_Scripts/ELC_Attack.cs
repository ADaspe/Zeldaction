using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Attack : MonoBehaviour
{
    public AXD_CharacterVariablesSO CharStats;
    [SerializeField]
    ELC_CharacterManager CharManager;
    public GameObject ShieldGO;
    [SerializeField]
    ELC_GameManager gameManager;
    //public float enemyDetectionRadius;
    public bool ShieldOn;
    public float NextShield;
    public bool spiritAttack;
    private GameObject nearestEnemy;
    private ELC_Attack SpiritAttackScript;
    public float attackTogetherCooldown;
    public string defaultMask;
    public string dashMask;
    ParticleSystem ShieldPS;


    private void Start()
    {
        SpiritAttackScript = CharManager.SpiritGO.GetComponent<ELC_Attack>();
        if (CompareTag("Ryn"))
        {
            ShieldPS = ShieldGO.GetComponent<ParticleSystem>();
        }
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
        if (CharManager.RynMove.canMove && !ShieldOn)
        {
            if(NextShield <= Time.time)
            {
                RynActivateShield();
            }
            else
            {
                CharManager.gameManager.audioManager.Play("Capacity_Cooldown");
            }
        }
        else if (!CharManager.RynMove.canMove && ShieldOn)
        {
            RynLoseShield();
        }
    }

    public void SpiritDashAttack()
    {
        CharManager.nextDash = Time.time + CharManager.stats.DashCoolDown;
        StartCoroutine(DashCoroutine());
    }

    public void AttackTogether()
    {

        if (attackTogetherCooldown > Time.time)
        {
            CharManager.gameManager.audioManager.Play("Capacity_Cooldown");
            return;
        }

        attackTogetherCooldown = Time.time + CharManager.stats.AttackCooldown;
        //Debug.Log("Attack Together");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(this.transform.position, CharStats.AttackTogetherRange, gameManager.EnemiesMask);
        //gameManager.audioManager.Play("");
        
        for (int i = 0; i < enemies.Length; i++)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(this.transform.position, enemies[i].transform.position - this.transform.position, Vector2.Distance(this.transform.position, enemies[i].transform.position), gameManager.GlobalObstaclesMask);

            float angle = Vector3.Angle(CharManager.RynMove.LastDirection, enemies[i].gameObject.transform.position - this.gameObject.transform.position);
            //Debug.Log(angle);
            if (angle > CharStats.TogetherAttackDetectionAngle) return;

            if (i == 0 && wallHit.collider == null)
            {
                nearestEnemy = enemies[i].gameObject;
            }
            else if (wallHit.collider == null && nearestEnemy != null && Vector2.Distance(this.transform.position, enemies[i].transform.position) < Vector2.Distance(this.transform.position, nearestEnemy.transform.position))
            {
                nearestEnemy = enemies[i].gameObject;
            }
        }
        if (nearestEnemy != null)
        {
            spiritAttack = true;
            StartCoroutine(ResetAfterSeconds(0.05f));
        }
    }

    public void RynActivateShield()
    {
        CharManager.gameManager.audioManager.Play("Ryn_ShieldOn");
        CharManager.AnimationManager.isAttacking = true;
        ShieldOn = true;
        CharManager.RynMove.canMove = false;
        CharManager.RynMove.rawInputMovement = Vector2.zero;
        NextShield = Time.time + CharManager.stats.ShieldDuration + CharManager.stats.ShieldCooldown;
        var mainShieldPS = ShieldPS.main;
        mainShieldPS.duration = CharManager.stats.ShieldDuration;
        mainShieldPS.startLifetime = CharManager.stats.ShieldDuration;
        ShieldPS.Play();

        Invoke("RynLoseShield", CharManager.stats.ShieldDuration);
    }

    public void RynLoseShield()
    {
        if (ShieldOn)
        {
            CharManager.gameManager.audioManager.Play("ShieldDown");
            NextShield = Time.time + CharManager.stats.ShieldCooldown;
            ShieldOn = false;
            CharManager.RynMove.canMove = true;
        }
    }

    public IEnumerator DashCoroutine()
    {
        CharManager.gameManager.audioManager.Play("Spirit_Dash");
        gameObject.layer = LayerMask.NameToLayer(dashMask.ToString());
        CharManager.spiritMove.wasDashingWhenColliding = true;
        CharManager.spiritMove.isDashing = true;
        CharManager.spiritMove.rb.velocity = CharManager.spiritMove.LastDirection * (CharManager.stats.DashDistance / CharManager.stats.DashTime);
        yield return new WaitForSeconds(CharManager.stats.DashTime);
        StopDashCoroutine();
    }

    public void StopDashCoroutine()
    {
        StopCoroutine("DashCoroutine");
        CharManager.spiritMove.currentSpeed = CharManager.stats.SpiritSpeed;
        CharManager.spiritMove.isDashing = false;
        CharManager.spiritMove.wasDashingWhenColliding = false;
        gameObject.layer = LayerMask.NameToLayer(defaultMask);
        CharManager.spiritMove.rb.velocity = Vector2.zero;
    }

    public void SpiritAttackTogether(Vector3 targetPos, float duration)
    {
        CharManager.AnimationManager.isAttacking = true;
        Vector3 direction = targetPos - this.gameObject.transform.position;
        direction = direction.normalized * (direction.magnitude / duration);
        this.transform.Translate(direction * Time.deltaTime);
        bool canTouch = true;
        Collider2D[] col = Physics2D.OverlapCircleAll(CharManager.SpiritGO.transform.position, 0.5f, CharManager.stats.EnemiesLayerMask);

        foreach (var item in col)
        {
            ELC_EnemyAI enemyScript = item.GetComponent<ELC_EnemyAI>();
            List<GameObject> GO = item.GetComponent<ELC_EnemyAI>().DetectionZone(enemyScript.EnemyStats.attackAreaRadius, enemyScript.EnemyStats.attackAreaAngle, item.transform.position + (Vector3)(direction.normalized * enemyScript.EnemyStats.attackAreaPositionFromEnemy));
            foreach (var GameO in GO)
            {
                if(GameO.CompareTag("Spirit"))
                {
                    canTouch = false;
                    spiritAttack = false;
                }
            }
            if (canTouch) item.gameObject.GetComponent<AXD_EnemyHealth>().GetHit(CharManager.stats.StunTime);
        }
    }

    private IEnumerator ResetAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        spiritAttack = false;
        nearestEnemy = null;
    }

    

}




