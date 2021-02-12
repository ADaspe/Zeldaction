using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Attack : MonoBehaviour
{
    [SerializeField]
    ELC_CharacterManager CharManager;

    public void Attack()
    {
        if (CharManager.Together) AttackTogether();
    }

    private void MiaShield()
    {

    }

    private void SpiritDashAttack()
    {

    }

    private void AttackTogether()
    {
        Debug.Log("Attack Together");
    }
}
