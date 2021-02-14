using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_SpiritIdle : MonoBehaviour
{
    [SerializeField]
    ELC_CharacterManager CharaManager;
    private bool PlayerIsImmobile;
    private float LastPlayerMove;
    public float TimeToWaitForIdleState;
    

    
    void Update()
    {
        if (CharaManager.miaMove.rawInputMovement.magnitude > 0.01f) LastPlayerMove = Time.deltaTime;

        if (Time.deltaTime - LastPlayerMove > TimeToWaitForIdleState) PlayerIsImmobile = true;
        else PlayerIsImmobile = false;
    }
}
