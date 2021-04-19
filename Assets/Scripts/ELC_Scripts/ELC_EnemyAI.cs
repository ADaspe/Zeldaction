using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ELC_EnemyAI : MonoBehaviour
{
    public Transform Target;
    public ELC_EnemySO EnemyStats;
    public bool EnableDebug;

    public float Speed = 200f;
    public float NextWaypointDistance = 3f; //à quelle distance il doit être d'un checkpoint pour se diriger vers le suivant (pour éviter que ce soit à 0 de distance qui serait impossible à atteindre pile)
    public float StopDistanceToPlayer = 5f; //à quelle distance il doit s'arrêter lorsqu'il est près du joueur

    public bool isPatrolling;
    public bool isFollowingPlayer;
    public bool isPreparingAttack;
    public bool isAttacking;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker; //Le calculateur de chemin
    Rigidbody2D rb;
    
    void Start()
    {
        currentWaypoint = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 1f);
        seeker.StartPath(rb.position, Target.position, OnPathCalculated);
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
            seeker.StartPath(rb.position, Target.position, OnPathCalculated);
        }
    }
    
    void FixedUpdate()
    {
        if (path == null || isPreparingAttack || isAttacking)
            return;

        if (isFollowingPlayer && Vector2.Distance(rb.position, Target.position) < StopDistanceToPlayer)
        {
            StartCoroutine(PrepareAttack(EnemyStats.prepareAttackTime));
            return;
        }
        
        if (path.vectorPath.Count <= currentWaypoint)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
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
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    private Transform Detection()
    {
        Collider2D[] mainRadius = Physics2D.OverlapCircleAll(rb.position, EnemyStats.detectionRadius, EnemyStats.DetectionMask);
        if (mainRadius.Length == 0) return this.transform;
        //ici mettre un else if (pas dans la zone angulaire) return;

        if (mainRadius.Length == 1) return mainRadius[0].transform;
        else
        {
            foreach (Collider2D col in mainRadius)
            {
                if (col.gameObject.layer.ToString().Equals("PlayerRyn")) return col.transform;
            }
        }

        return this.transform; //ça c'est vraiment si ça marche pas
    }
}
