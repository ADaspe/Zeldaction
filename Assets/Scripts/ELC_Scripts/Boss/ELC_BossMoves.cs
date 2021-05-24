using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ELC_BossMoves : MonoBehaviour
{
    public GameObject TargetGO;
    [HideInInspector]
    public Vector3 Target;
    ELC_BossManager BossMana;
    public float NextWaypointDist = 0.3f;
    public float distToStopNearTarget;
    public float speed;
    public bool CanMove;

    Vector2 direction;
    public Vector2 LastDirection;
    public LayerMask ObstaclesMask;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    private void Awake()
    {
        BossMana = this.GetComponent<ELC_BossManager>();
    }

    void Start()
    {
        currentWaypoint = 0;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f, 0.3f);
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
        Target = TargetGO.transform.position;
        if(seeker.IsDone())
        {
            seeker.StartPath(rb.position, Target, OnPathCalculated);
        }
    }

    private void FixedUpdate()
    {
        if (path == null || !CanMove) return;

        if(Vector2.Distance(rb.position, Target) < distToStopNearTarget)
        {
            if(!IsThereWallBetweenTarget(Target))
            {
                Debug.Log("Se prépare à attaquer");
                BossMana.Attack(LastDirection);
                return;
            }
            Debug.Log("est à distance mais ne peut attaquer");
            return;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; //On prends la position du prochain point du path et on calcule le vecteur pour aller dans sa direction
        Vector2 force = direction * speed * Time.deltaTime; //On augmente le vecteur pour avoir une plus grande force

        rb.AddForce(force); //On déplace l'objet dans la direction

        if (direction.magnitude > 0.05f)
        {
            LastDirection = direction;
            BossMana.LastDir = LastDirection;
        }

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]); //On vérifie à quelle distance on est du prochain waypoint
        if (distanceToWaypoint < NextWaypointDist) currentWaypoint++;//Si on est assez proche, on passe au prochain waypoint
    }

    private bool IsThereWallBetweenTarget(Vector3 target)
    {
        Vector2 dir = target - this.transform.position;

        RaycastHit2D wallCollides = Physics2D.Raycast(this.transform.position, dir, Vector2.Distance(target, this.transform.position), ObstaclesMask);
        if (wallCollides.collider == null) return false;
        else return true;
    }
}
