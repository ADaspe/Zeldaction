using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Totem : MonoBehaviour
{
    public bool isActive;
    public ELC_EnemyAI[] EnemiesToProtect;

    private void Start()
    {
        isActive = true;
        foreach (ELC_EnemyAI enemy in EnemiesToProtect)
        {
            enemy.isProtected = true;
            enemy.ShieldParticles.Play();
        }
    }

    public void Pacificate()
    {
        if (isActive)
        {
            if (EnemiesToProtect[0] != null) { 
                EnemiesToProtect[0].gameMana.audioManager.Play("Totem_Pacification"); 
            }
            isActive = false;
            foreach (ELC_EnemyAI enemy in EnemiesToProtect)
            {
                enemy.ShieldParticles.Stop();
                enemy.isProtected = false;
            }
        }
    }

}
