using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objects", menuName = "ScriptableObjects/EnemyStatsSO", order = 1)]
public class ELC_EnemySO : ScriptableObject
{
    public float prepareAttackTime;
    public float attackTime;
    public float attackDistance;
    public float detectionRadius;
    public float DetectionAngle;
    public bool haveInsensibilityZone;
    public float insensibilityRadius;
    public float insensibilityAngle;
    public LayerMask DetectionMask;
    public LayerMask ObstaclesMask;
    public float trackingPlayerSpeed;
    public float patrolSpeed;
}
