using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossManager : MonoBehaviour
{
    [HideInInspector]
    public ELC_BossMoves BossMoves;
    [HideInInspector]
    public ELC_BossAttacks BossAttacks;
    public GameObject RynGO;
    public bool isAttacking;
    public bool canAttack;
    public Vector3 LastDir;
    public int CurrentPhase = 1;

    private void Awake()
    {
        canAttack = true;
        CurrentPhase = 1;
        BossMoves = this.GetComponent<ELC_BossMoves>();
        BossAttacks = this.GetComponent<ELC_BossAttacks>();
        BossMoves.Target = RynGO.transform.position;
        BossMoves.TargetGO = RynGO;
    }

    public void Attack(Vector3 TargetDir)
    {
        BossMoves.CanMove = false;
        BossAttacks.TargetDirection = TargetDir;
        BossAttacks.PrepareAttack();
    }
}
