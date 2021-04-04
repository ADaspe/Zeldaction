using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_EnemyHealth : MonoBehaviour
{
    public AXD_EnemyMove enemyMove;

    public void GetHit(float timeToStun)
    {
        if (!enemyMove.isStuned)
        {
            Debug.Log("Aïe, je suis stun");
            enemyMove.isStuned = true;
            enemyMove.Invoke("UnStun", timeToStun);
        }
    }
}
