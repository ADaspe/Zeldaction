using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_EnemyMove : MonoBehaviour
{
    public bool isStuned;
    private float timeToMoveAgain;

    public void UnStun()
    {
        Debug.Log("Je ne suis plus stun héhé");
        isStuned = false;
    }
}
