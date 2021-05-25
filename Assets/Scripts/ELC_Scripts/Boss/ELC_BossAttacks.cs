using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossAttacks : MonoBehaviour
{
    [HideInInspector]
    public ELC_BossManager BossMana;
    private float PrepareAttackDuration;
    private float AttackDuration;
    private float Cooldown;
    private float AttackRadius;
    private float AttackAngle;
    public float OriginDistOfAttackDetector;
    public Vector3 TargetDirection;

    [Header("Phase 1")]
    public float BasicAttackPreparationTime;
    public float BasicAttackDuration;
    public float BasicAttackCooldown;
    public float BasicAttackRadius;
    public float BasicAttackAngle;

    [Header("Phase 2")]
    public float DashPreparationTime;
    public float DashDistance;
    public float DashDuration;
    public float DashCooldown;
    public float DashDetectionRadius;
    public float DashDetectionAngle;


    [Header("Phase 3")]
    public float RayPreparationTime;

    bool isPreparingAttack;
    bool isAttacking;

    private void Awake()
    {
        BossMana = this.GetComponent<ELC_BossManager>();
    }

    void FixedUpdate()
    {
        if(isAttacking && BossMana.CurrentPhase == 2) //Attaque Dash
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
        switch (BossMana.CurrentPhase)
        {
            case 1:
                PrepareAttackDuration = BasicAttackPreparationTime;
                AttackDuration = BasicAttackDuration;
                Cooldown = BasicAttackCooldown;
                AttackRadius = BasicAttackRadius;
                AttackAngle = BasicAttackAngle;
                break;
            case 2:
                PrepareAttackDuration = DashPreparationTime;
                AttackDuration = DashDuration;
                Cooldown = DashCooldown;
                AttackRadius = DashDetectionRadius;
                AttackAngle = DashDetectionAngle;
                break;
            case 3:
                PrepareAttackDuration = RayPreparationTime;
                break;
            default:
                break;
        }

        isAttacking = true;
        //Attaque basique
        //Activation rayons
        //L'attaque Dash se fait dans le FixedUpdate
        BossMana.canAttack = false;
        StartCoroutine("CooldownsAttack");
        Invoke("EndAttack", AttackDuration);
    }

    void Dash()
    {
        Vector3 origin = this.transform.position + BossMana.LastDir * OriginDistOfAttackDetector;

        this.GetComponent<Rigidbody2D>().velocity = BossMana.LastDir * (DashDistance / DashDuration);
        List<GameObject> detected = DetectionZone(AttackRadius, AttackAngle, origin);
        foreach (GameObject DetectedGO in detected)
        {
            if(DetectedGO.CompareTag("Ryn"))
            {
                DetectedGO.GetComponent<AXD_Health>().GetHit();
            }
        }
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

    IEnumerator CooldownsAttack()
    {
        yield return new WaitForSeconds(Cooldown);
        BossMana.canAttack = true;
    }
}
