using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_EnemyHealth : MonoBehaviour
{
    public ELC_EnemyAI enemyAI;

    public void GetHit(float timeToStun)
    {
        if (!enemyAI.isStunned)
        {
            enemyAI.isStunned = true;
            Invoke("CancelStun", timeToStun);
        }
    }

    private void CancelStun()
    {
        enemyAI.isStunned = false;
    }

    
}
