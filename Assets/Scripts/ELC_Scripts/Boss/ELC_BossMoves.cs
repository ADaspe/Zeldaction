using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ELC_BossMoves : MonoBehaviour
{
    [HideInInspector]
    public GameObject TargetGO;
    [HideInInspector]
    public Vector3 Target;
    [HideInInspector]
    ELC_BossManager BossMana;
    public float NextWaypointDist = 0.3f;
    public float BasicDistToStopNearPlayer;
    public float DashDistToStopNearPlayer;
    [HideInInspector]
    public float distToStopNearTarget;
    public float speed;
    public bool CanMove;
    public bool FollowPlayer;
    public bool isGoingToPreciseLocation;
    public bool ReachedThePreciseLocation;
    public GameObject shadow;

    Vector2 direction;
    public Vector2 LastDirection;
    private LayerMask ObstaclesMask;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;
    Animator anims;
    SpriteRenderer SpriteRend;

    private void Awake()
    {
        SpriteRend = this.GetComponent<SpriteRenderer>();
        anims = this.GetComponent<Animator>();
        BossMana = this.GetComponent<ELC_BossManager>();
        ObstaclesMask = BossMana.ObstaclesMask;
    }

    void Start()
    {
        currentWaypoint = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f, 0.3f);
        seeker.StartPath(rb.position, Target, OnPathCalculated);
        FollowPlayer = true;
        InvokeRepeating("UpdateAnimator", 0.5f, 0.3f);
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
        if (FollowPlayer) Target = TargetGO.transform.position;
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, Target, OnPathCalculated);
        }
    }

    private void FixedUpdate()
    {
        if (path == null || !CanMove || BossMana.isStunned)
        {
            anims.SetBool("isMoving", false);
            return;
        }

        if(Vector2.Distance(rb.position, Target) < distToStopNearTarget && !BossMana.IsInSwitchPhase)
        {
            if(!IsThereWallBetweenTarget(Target) && BossMana.canAttack)
            {
                anims.SetBool("isMoving", false);
                BossMana.Attack(LastDirection);
                return;
            }
            //Debug.Log("est � distance mais ne peut attaquer");
            return;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; //On prends la position du prochain point du path et on calcule le vecteur pour aller dans sa direction
        Vector2 force = direction * speed * Time.deltaTime; //On augmente le vecteur pour avoir une plus grande force

        rb.AddForce(force); //On d�place l'objet dans la direction

        if (direction.magnitude > 0.4f)
        {
            LastDirection = direction;
            BossMana.LastDir = LastDirection;

            anims.SetBool("isMoving", true);
        }

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]); //On v�rifie � quelle distance on est du prochain waypoint
        if (distanceToWaypoint < NextWaypointDist)
        {
            if (isGoingToPreciseLocation && currentWaypoint == path.vectorPath.Count - 1) //Si on est en train de suivre un chemin pr�cis et qu'on a atteint le bout
            {
                BossMana.ReachedLocation();
                isGoingToPreciseLocation = false;
            }

            if(currentWaypoint != path.vectorPath.Count - 1) currentWaypoint++;//Si on est assez proche, on passe au prochain waypoint
        }
    }

    private bool IsThereWallBetweenTarget(Vector3 target)
    {
        Vector2 dir = target - this.transform.position;

        RaycastHit2D wallCollides = Physics2D.Raycast(this.transform.position, dir, Vector2.Distance(target, this.transform.position), ObstaclesMask);
        if (wallCollides.collider == null) return false;
        else return true;
    }

    private void UpdateAnimator()
    {
        if(!BossMana.isAttacking && !BossMana.BossAttacks.isAttacking)
        {
            anims.SetFloat("MovesX", LastDirection.x);
            if(BossMana.CurrentPhase != 3) anims.SetFloat("MovesY", LastDirection.y);
            else anims.SetFloat("MovesY", -1);
            if (LastDirection.x > 0 && LastDirection.x != 0) SpriteRend.flipX = true;
            else SpriteRend.flipX = false;
        }
    }

    public void RotateShadow0()
    {
        shadow.transform.SetPositionAndRotation(shadow.transform.position, new Quaternion(0,0,0,0));
    }

    public void RotateShadow90()
    {
        shadow.transform.SetPositionAndRotation(shadow.transform.position, new Quaternion(0, 0, 90, 0));
    }
}