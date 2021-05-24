using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossManager : MonoBehaviour
{
    public ELC_BossMoves BossMoves;
    public ELC_BossAttacks BossAttacks;
    public GameObject RynGO;
    public bool isAttacking;
    public Vector3 LastDir;

    private void Awake()
    {
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
