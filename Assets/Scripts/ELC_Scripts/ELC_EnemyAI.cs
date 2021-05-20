using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ELC_EnemyAI : MonoBehaviour
{
    public Vector3 Target;
    public ELC_EnemySO EnemyStats;
    public ELC_GameManager gameMana;
    public bool EnableDebug;
    public bool isStunned;
    public bool isProtected;

    public float Speed = 200f;
    public float NextWaypointDistance = 0.3f; //à quelle distance il doit être d'un checkpoint pour se diriger vers le suivant (pour éviter que ce soit à 0 de distance qui serait impossible à atteindre pile)
    

    public bool canPatrol;
    public bool isPatrolling;
    public bool isFollowingPlayer;
    public bool isPreparingAttack;
    public bool isAttacking;
    public List<Transform> PatrolPath;


    Vector2 direction;
    Path path;
    int currentWaypoint = 0;
    int PatrolPathIndex = 0;
    bool reachedEndOfPath = false;

    Seeker seeker; //Le calculateur de chemin
    Rigidbody2D rb;
    
    void Start()
    {
        currentWaypoint = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        foreach(Transform t in PatrolPath)
        {
            t.SetParent(null);
        }

        InvokeRepeating("UpdatePath", 0f, 0.5f);
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


        rb.AddForce(force);

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        
        if (distanceToWaypoint < NextWaypointDistance) currentWaypoint++;
    }

    IEnumerator PrepareAttack(float time)
    {
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
        
        List<Collider2D> col = DetectionZone(EnemyStats.attackAreaRadius, EnemyStats.attackAreaAngle, this.transform.position + (Vector3)(direction.normalized * EnemyStats.attackAreaPositionFromEnemy));
        
        Collider2D target = null;

        foreach (Collider2D collider in col)
        {
            if(collider.CompareTag("Ryn"))
            {
                target = collider;
                Debug.Log("oui");
            }
            else if(collider.CompareTag("Spirit") && target == null)
            {
                target = collider;
            }
        }
        if(target != null) target.GetComponent<AXD_Health>().GetHit();

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

    private List<Collider2D> DetectionZone(float radius, float angle, Vector3 origin)
    {
        if (isProtected)
        {
            radius = 0.8f;
            angle = 180;
        }
        Collider2D[] col = Physics2D.OverlapCircleAll(this.transform.position, radius);

        List<Collider2D> collidersInsideArea = new List<Collider2D>();
        foreach (var collider in col)
        {
            float currentAngle = Vector2.Angle(direction, collider.transform.position - origin);
            if (currentAngle <= angle && currentAngle >= -angle)
            {
                collidersInsideArea.Add(collider);
                
            }
        }
        return collidersInsideArea;
    }

    private void Shield()
    {
        List<Collider2D> col = DetectionZone(EnemyStats.insensibilityRadius, EnemyStats.insensibilityAngle, this.transform.position);
        foreach (Collider2D collider in col)
        {
            if (collider.gameObject.CompareTag("Spirit") && gameMana.CharacterManager.spiritMove.isDashing)
            {
                gameMana.CharacterManager.SpiritGO.GetComponent<ELC_Attack>().StopDashCoroutine();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, EnemyStats.detectionRadius);
        Gizmos.DrawRay(new Ray(this.transform.position, direction));
        Gizmos.DrawWireSphere(this.transform.position + (Vector3)(direction.normalized * EnemyStats.attackAreaPositionFromEnemy), EnemyStats.attackAreaRadius);
    }
}
