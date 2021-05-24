using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossAttacks : MonoBehaviour
{
    public bool DashAttack;
    public ELC_BossManager BossMana;
    public float PrepareAttackDuration;
    public float AttackDuration;
    public Vector3 TargetDirection;

    bool isPreparingAttack;
    bool isAttacking;

    private void Awake()
    {
        BossMana = this.GetComponent<ELC_BossManager>();
    }

    void FixedUpdate()
    {
        if(isAttacking && DashAttack) //Attaque Dash
        {
            Dash();
        }
    }

    public void PrepareAttack()
    {
        BossMana.isAttacking = true;
        isPreparingAttack = true;
        Invoke("Attack", PrepareAttackDuration);
    }

    void Attack()
    {
        isAttacking = true;
        //Attaque basique
        //Activation rayons
        //L'attaque Dash se fait dans le FixedUpdate

        Invoke("EndAttack", AttackDuration);
    }

    void Dash()
    {
        //Dash code
    }

    void EndAttack()
    {
        isAttacking = false;
        BossMana.isAttacking = false;
        BossMana.BossMoves.CanMove = true;
    }


    private List<GameObject> DetectionZone(float radius, float angle, Vector3 origin)
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(origin, radius);

        List<GameObject> collidersInsideArea = new List<GameObject>();
        foreach (var collider in col)
        {
            float currentAngle = Vector2.Angle(BossMana.LastDir, collider.transform.position - origin);
            if (currentAngle <= angle && currentAngle >= -angle)
            {
                collidersInsideArea.Add(collider.gameObject);
            }
        }

        return collidersInsideArea;
    }
}
