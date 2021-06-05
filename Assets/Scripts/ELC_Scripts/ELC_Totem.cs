using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Totem : MonoBehaviour
{
    public ELC_EnemyAI[] EnemiesToProtect;

    private void Start()
    {
        foreach (ELC_EnemyAI enemy in EnemiesToProtect)
        {
            enemy.isProtected = true;
            enemy.ShieldParticles.Play();
        }
    }

    public void Purify()
    {
        foreach (ELC_EnemyAI enemy in EnemiesToProtect)
        {
            enemy.ShieldParticles.Stop();
            enemy.isProtected = false;
        }
    }

}
