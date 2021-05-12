using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Totem : MonoBehaviour
{
    public ELC_EnemyAI[] EnemiesToProtect;

    rivate void Start()
    {
        foreach (ELC_EnemyAI enemy in EnemiesToProtect)
        {
            enemy.isProtected = true;
        }
    }

    public void Purify()
    {
        foreach (ELC_EnemyAI enemy in EnemiesToProtect)
        {
            enemy.isProtected = false;
        }
    }

}
