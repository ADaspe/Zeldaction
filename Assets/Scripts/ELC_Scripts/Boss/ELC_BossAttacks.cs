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
    private SpriteRenderer SpriteRend;

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
    public int NumberOfRaysPerPhase;
    public float TiredTime;
    int currentNumberOfRays;

    public bool isAttacking;
    Animator anims;
    bool isFading;
    public bool isTired;

    private void Awake()
    {
        anims = this.GetComponent<Animator>();
        BossMana = this.GetComponent<ELC_BossManager>();
        SpriteRend = this.GetComponent<SpriteRenderer>();
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
        if(isAttacking && BossMana.CurrentPhase == 2 && !BossMana.isStunned) //Attaque Dash
        {
            Dash();
        }
    }

    public void PrepareAttack()
    {
        
        BossMana.isAttacking = true;
        BossMana.gameManager.audioManager.Play("Boss_Growl");
        switch (BossMana.CurrentPhase)
        {
            case 1:
                PrepareAttackDuration = BasicAttackPreparationTime;
                AttackDuration = BasicAttackDuration;
                Cooldown = BasicAttackCooldown;
                AttackRadius = BasicAttackRadius;
                AttackAngle = BasicAttackAngle;
                //SpriteRend.enabled = true;
                StartCoroutine(Fade(true));
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
        anims.SetBool("Growl", false);
        BossMana.gameManager.audioManager.Play("Boss_Atk");
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
        anims.SetBool("Dash", true);
        
        RaycastHit2D WallDetector = Physics2D.Raycast(this.gameObject.GetComponent<Rigidbody2D>().position, BossMana.LastDir.normalized, 1.5f, BossMana.ObstaclesMask);
        if (WallDetector.collider != null)
        {
            DashCrashOnWall();
        }
        
        Vector3 origin = this.transform.position + BossMana.LastDir * OriginDistOfAttackDetector;
        this.GetComponent<Rigidbody2D>().velocity = BossMana.LastDir * (DashDistance / DashDuration);
        List<GameObject> detected = DetectionZone(AttackRadius, AttackAngle, origin);

        foreach (GameObject DetectedGO in detected)
        {
            if (DetectedGO.CompareTag("Ryn"))
            {
                DetectedGO.GetComponent<AXD_Health>().GetHit();
            }
            //else if (DetectedGO.CompareTag("Obstacle"))
            //{
            //    DashCrashOnWall();
            //}
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        anims.SetBool("Bite", false);
        anims.SetBool("AttackPhase", false);
        anims.SetBool("isGrowling", false);
        StartCoroutine("CooldownsAttack");
        if (BossMana.CurrentPhase == 1) StartCoroutine(Fade());


        anims.SetBool("Dash", false);
        BossMana.isAttacking = false;
        //if (BossMana.CurrentPhase == 1) SpriteRend.enabled = false;
        if(BossMana.CurrentPhase != 3) BossMana.BossMoves.CanMove = true;
    }

    void BasicAttack()
    {
        anims.SetBool("Bite", true);
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
        currentNumberOfRays++;
        if (!isTired) PrepareAttack();

        if (currentNumberOfRays == NumberOfRaysPerPhase)
        {
            currentNumberOfRays = 0;
            Tired();
        }

        yield return new WaitWhile(() => isTired);
        yield return new WaitForSeconds(RayAttackPreparationTime + RaySpawnTime + RayDespawnTime);

        StartCoroutine(RayPhase());
    }

    void DashCrashOnWall()
    {
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        CancelInvoke("EndAttack");

        EndAttack();

        if(BossMana.BossHealth.HaveShield) StartCoroutine(BossMana.BossHealth.ShieldLostAndRecover());
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

    private void Tired()
    {
        isTired = true;
        BossMana.BossHealth.ShieldLostAndRecover();
    }

    public IEnumerator Fade(bool FadeIn = false)
    {
        yield return new WaitWhile(() => isFading);
        isFading = true;
        float alphaValue = 0;
        if(!FadeIn)
        {
            alphaValue = 1;
            while (alphaValue > 0)
            {
                yield return new WaitForSeconds(0.05f);
                alphaValue -= 0.1f;
                var mat = SpriteRend.material.color;
                mat.a = alphaValue;
                SpriteRend.color = mat;
            }
        }
        else
        {
            alphaValue = 0;
            while (alphaValue < 1)
            {
                yield return new WaitForSeconds(0.05f);
                alphaValue += 0.1f;
                var mat = SpriteRend.material.color;
                mat.a = alphaValue;
                SpriteRend.color = mat;
            }
        }
        isFading = false;
    }

    public IEnumerator CooldownsAttack()
    {
        yield return new WaitForSeconds(Cooldown);
        BossMana.canAttack = true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(this.transform.position + BossMana.LastDir * OriginDistOfAttackDetector, AttackRadius);
    }
}
