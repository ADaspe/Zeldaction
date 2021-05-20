using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objects", menuName = "ScriptableObjects/EnemyStatsSO", order = 1)]
public class ELC_EnemySO : ScriptableObject
{
    public float prepareAttackTime;
    public float attackTime;
    public float attackAreaPositionFromEnemy; //EN gros si on veut faire partir le cercle de collision plus loin que la position de l'entité
    public float attackAreaRadius;
    public float attackAreaAngle;
    public float detectionRadius;
    public float DetectionAngle;
    public bool haveInsensibilityZone;
    public float insensibilityRadius;
    public float insensibilityAngle;
    public LayerMask DetectionMask;
    public LayerMask ObstaclesMask;
    public float trackingPlayerSpeed;
    public float patrolSpeed;
    public float distanceToStopNearTarget;
}
