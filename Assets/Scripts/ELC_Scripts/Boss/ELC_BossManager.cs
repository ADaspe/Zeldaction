using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossManager : MonoBehaviour
{
    [HideInInspector]
    public ELC_BossMoves BossMoves;
    [HideInInspector]
    public ELC_BossAttacks BossAttacks;
    public ELC_BossHealth BossHealth;
    public GameObject RynGO;
    public Vector3 Spawn;
    public bool isAttacking;
    public bool canAttack;
    public Vector3 LastDir = Vector3.zero;
    public int CurrentPhase = 1;
    public LayerMask ObstaclesMask;
    public ELC_BossRay[] RaysList;

    private void Awake()
    {
        BossMoves = this.GetComponent<ELC_BossMoves>();
        BossAttacks = this.GetComponent<ELC_BossAttacks>();
        BossMoves.Target = RynGO.transform.position;
        BossMoves.TargetGO = RynGO;
        BossAttacks.Rays = RaysList;
        canAttack = true;
        CurrentPhase = 1;
    }

    public void Attack(Vector3 TargetDir)
    {
        BossMoves.CanMove = false;
        BossAttacks.TargetDirection = TargetDir;
        BossAttacks.PrepareAttack();
    }

    public void SwitchPhase()
    {
        CurrentPhase++;
        switch (CurrentPhase)
        {
            case 2:
                BossHealth.CurrentHealth = BossHealth.SecondPhaseHealth;
                break;
            case 3:
                BossHealth.CurrentHealth = BossHealth.ThirdPhaseHealth;
                break;
            default:
                break;
        }
    }
}
