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
    public float RayAttackPreparationTime;
    public float RayAttackCooldown;
    [Range(0f, 3f)]
    public float RaySpawnTime;
    [Range(0f,5f)]
    public float RayDespawnTime;
    [HideInInspector]
    public ELC_BossRay[] Rays;

    //bool isPreparingAttack;
    bool isAttacking;

    private void Awake()
    {
        BossMana = this.GetComponent<ELC_BossManager>();

        PrepareAttackDuration = BasicAttackPreparationTime;
        AttackDuration = BasicAttackDuration;
        Cooldown = BasicAttackCooldown;
        AttackRadius = BasicAttackRadius;
        AttackAngle = BasicAttackAngle;
    }

    private void Start()
    {
        foreach (ELC_BossRay BossRay in Rays)
        {
            BossRay.timeBeforeSpawn = RaySpawnTime * BossRay.Index;
            BossRay.timeBeforeDespawn = RayDespawnTime;
        }
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
                PrepareAttackDuration = RayAttackPreparationTime;
                Cooldown = RayAttackCooldown;
                break;
            default:
                break;
        }
        Invoke("Attack", PrepareAttackDuration);
    }

    void Attack()
    {
        switch (BossMana.CurrentPhase)
        {
            case 1:
                BasicAttack();
                break;
            case 2:
                //L'attaque Dash se fait dans le FixedUpdate
                break;
            case 3:
                Ray();
                break;
            default:
                break;
        }
        

        isAttacking = true;
        BossMana.canAttack = false;
        
        Invoke("EndAttack", AttackDuration);
    }

    void Dash()
    {
        RaycastHit2D WallDetector = Physics2D.Raycast(this.transform.position, BossMana.LastDir.normalized, 2f, BossMana.ObstaclesMask);
        if(WallDetector.collider != null)
        {
            DashCrashOnWall();
            return;
        }

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
        StartCoroutine("CooldownsAttack");
        isAttacking = false;
        BossMana.isAttacking = false;
        if(BossMana.CurrentPhase != 3) BossMana.BossMoves.CanMove = true;
    }

    void BasicAttack()
    {
        Vector3 origin = this.transform.position + BossMana.LastDir * OriginDistOfAttackDetector;
        List<GameObject> detected = DetectionZone(AttackRadius, AttackAngle, origin);
        foreach (GameObject DetectedGO in detected)
        {
            if (DetectedGO.CompareTag("Ryn"))
            {
                DetectedGO.GetComponent<AXD_Health>().GetHit();
            }
        }
    }

    public IEnumerator RayPhase()
    {
        yield return new WaitForSeconds(RayAttackCooldown);

        PrepareAttack();

        yield return new WaitForSeconds(RayAttackPreparationTime + RaySpawnTime + RayDespawnTime);

        StartCoroutine(RayPhase());
    }

    void DashCrashOnWall()
    {
        CancelInvoke("EndAttack");
        EndAttack();
        Debug.Log("Le boss s'est crash contre un mur : il est raplapla");
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

    
    private void Ray()
    {
        foreach (ELC_BossRay BossRay in Rays)
        {
            BossRay.StartCoroutine("Spawn");
        }
    }

    IEnumerator CooldownsAttack()
    {
        yield return new WaitForSeconds(Cooldown);
        BossMana.canAttack = true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(this.transform.position + BossMana.LastDir * OriginDistOfAttackDetector, AttackRadius);
    }
}