using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ELC_EnemyAI : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Target;
    public ELC_EnemySO EnemyStats;
    public ELC_GameManager gameMana;
    public bool EnableDebug;
    public bool isStunned;
    public bool isProtected;
    public ParticleSystem ShieldParticles;
    public enum EnemyType { BASIC,SHIELD}
    public EnemyType type;
    public float Speed = 200f;
    public float NextWaypointDistance = 0.3f; //à quelle distance il doit être d'un checkpoint pour se diriger vers le suivant (pour éviter que ce soit à 0 de distance qui serait impossible à atteindre pile)
    

    public bool canPatrol;
    public bool isPatrolling;
    public bool isFollowingPlayer;
    public bool isPreparingAttack;
    public bool isAttacking;
    public List<Transform> PatrolPath;

    public Vector2 LastDirection;
    Vector2 direction;
    Path path;
    int currentWaypoint = 0;
    int PatrolPathIndex = 0;
    bool reachedEndOfPath = false;
    Animator anims;

    Seeker seeker; //Le calculateur de chemin
    Rigidbody2D rb;
    SpriteRenderer spriteRend;
    
    void Start()
    {
        spriteRend = this.GetComponent<SpriteRenderer>();
        anims = this.GetComponent<Animator>();
        currentWaypoint = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdateAnimations", 0.5f, 0.4f);
        foreach(Transform t in PatrolPath)
        {
            t.SetParent(null);
        }

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        
        Target = Detection();
        seeker.StartPath(rb.position, Target, OnPathCalculated);
    }

    void OnPathCalculated(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath()
    {
        Target = Detection();
        if(seeker.IsDone())
        {
            seeker.StartPath(rb.position, Target, OnPathCalculated);
        }
    }
    
    void FixedUpdate()
    {
        if(isStunned)
        {
            isAttacking = false;
            isPreparingAttack = false;
            StopAllCoroutines();
            return;
        }

        if (isProtected || EnemyStats.haveInsensibilityZone)
        {
            Shield();
        }

        if (path == null || isPreparingAttack || isAttacking)
            return;

        if (isFollowingPlayer && Vector2.Distance(rb.position, Target) < EnemyStats.distanceToStopNearTarget)
        {
            if (!IsThereWallBetweenTarget(Target))
            {
                StartCoroutine(PrepareAttack(EnemyStats.prepareAttackTime));
                return;
            }
        }
        
        if (path.vectorPath.Count <= currentWaypoint)
        {
            reachedEndOfPath = true;
            if(isPatrolling)
            {
                if (PatrolPathIndex >= PatrolPath.Count - 1) PatrolPathIndex = 0;
                else PatrolPathIndex++;
            }
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * Speed * Time.deltaTime;
        if (direction.magnitude >= 0.05f) LastDirection = direction;




        rb.AddForce(force);

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        
        if (distanceToWaypoint < NextWaypointDistance) currentWaypoint++;
    }

    private void UpdateAnimations()
    {
        if (!isAttacking || !isPreparingAttack)
        {
            anims.SetFloat("MoveX", direction.x);
            anims.SetFloat("MoveY", direction.y);

            anims.SetBool("isMoving", true);
        }
        else anims.SetBool("isMoving", false);

        if (direction.x < 0.05f)
        {
            spriteRend.flipX = true;
        }
        else spriteRend.flipX = false;
    }

    IEnumerator PrepareAttack(float time)
    {
        if(type == EnemyType.BASIC)
        {
            gameMana.audioManager.Play("Basic_Atk");
        }else if (type == EnemyType.SHIELD)
        {
            gameMana.audioManager.Play("DS_Atk");
        }
        
        anims.SetBool("PrepareAttack", true);
        isPreparingAttack = true;
        if(EnableDebug) Debug.Log("prepare");
        yield return new WaitForSeconds(time);
        StartCoroutine(Attack(EnemyStats.attackTime));
        isPreparingAttack = false;
    }

    IEnumerator Attack(float time)
    {
        if (EnableDebug) Debug.Log("attack");
        isAttacking = true;
        anims.SetBool("isAttacking", true);
        
        List<GameObject> col = DetectionZone(EnemyStats.attackAreaRadius, EnemyStats.attackAreaAngle, this.transform.position + (Vector3)(LastDirection.normalized * EnemyStats.attackAreaPositionFromEnemy));
        
        GameObject target = null;

        foreach (GameObject GO in col)
        {
            if(GO.CompareTag("Ryn"))
            {
                target = GO;
            }
            else if(GO.CompareTag("Spirit") && target == null)
            {
                target = GO;
            }
        }
        if (target != null)
        {
            gameMana.audioManager.Play("Basic_Impact");
            target.GetComponent<AXD_Health>().GetHit();
        }

        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    private Vector3 Detection()
    {
        Collider2D[] mainRadius = Physics2D.OverlapCircleAll(rb.position, EnemyStats.detectionRadius, EnemyStats.DetectionMask);
        
        if (mainRadius.Length == 0 )
        {
            isFollowingPlayer = false;
            if (reachedEndOfPath)
            {
                if (canPatrol) //Fait patrouiller l'ennemi si personne est détecté
                {
                    isPatrolling = true;
                    return PatrolPath[PatrolPathIndex].position;
                }
            }
            else return Target;
        }


        //ici mettre un else if (pas dans la zone angulaire) return;
        isPatrolling = false;
        if (mainRadius.Length == 1)
        {
            isFollowingPlayer = true;
            if(mainRadius[0].gameObject.CompareTag("Ryn") || !mainRadius[0].GetComponent<AXD_CharacterMove>().charaManager.Together) return mainRadius[0].transform.position;
        }
        else
        {
            isFollowingPlayer = true;
            foreach (Collider2D col in mainRadius)
            {
                if (col.gameObject.CompareTag("Ryn")) return col.transform.position;
            }
        }
        
        return path.vectorPath[path.vectorPath.Count - 1]; //ça c'est vraiment si ça marche pas
    }

    private bool IsThereWallBetweenTarget(Vector3 target)
    {
        Vector2 dir = target - this.transform.position;

        RaycastHit2D wallCollides = Physics2D.Raycast(this.transform.position, dir, Vector2.Distance(target, this.transform.position), EnemyStats.ObstaclesMask);
        if (wallCollides.collider == null) return false;
        else return true;
    }

    public List<GameObject> DetectionZone(float radius, float angle, Vector3 origin)
    {
        if (isProtected)
        {
            radius = 1.5f;
            angle = 180;
        }
        Collider2D[] col = Physics2D.OverlapCircleAll(origin, radius);

        List<GameObject> collidersInsideArea = new List<GameObject>();
        foreach (var collider in col)
        {
            float currentAngle = Vector2.Angle(LastDirection, collider.transform.position - origin);
            if (currentAngle <= angle && currentAngle >= -angle)
            {
                collidersInsideArea.Add(collider.gameObject);
                
            }
        }
        return collidersInsideArea;
    }

    private void Shield()
    {
        List<GameObject> col = DetectionZone(EnemyStats.insensibilityRadius, EnemyStats.insensibilityAngle, this.transform.position);
        
        foreach (GameObject GO in col)
        {
            
            if (GO.gameObject.CompareTag("Spirit"))
            {
                if (gameMana.CharacterManager.spiritMove.isDashing)
                {
                    gameMana.CharacterManager.SpiritGO.GetComponent<ELC_Attack>().StopDashCoroutine();
                    gameMana.audioManager.Play("DS_SpiritDefense");
                    Debug.Log("bloqué");
                }
                else if (gameMana.CharacterManager.RynGO.GetComponent<ELC_Attack>().spiritAttack)
                {
                    gameMana.CharacterManager.RynGO.GetComponent<ELC_Attack>().spiritAttack = false;
                    gameMana.audioManager.Play("DS_SpiritDefense");
                    Debug.Log("bloqué");
                }
            }
        }
    }

    public void PlayFootStepEnemy()
    {
        if (type == EnemyType.SHIELD)
        {
            gameMana.audioManager.Play("DS_Move");
        }else if (type == EnemyType.BASIC)
        {
            gameMana.audioManager.Play("Basic_Move");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, EnemyStats.detectionRadius);
        Gizmos.DrawRay(new Ray(this.transform.position, LastDirection));
        Gizmos.DrawWireSphere(this.transform.position + (Vector3)(LastDirection.normalized * EnemyStats.attackAreaPositionFromEnemy), EnemyStats.attackAreaRadius);
    }
}
