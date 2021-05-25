using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossHealth : MonoBehaviour
{
    public int Alexandre;
    public ELC_BossManager BossMana;

    public int FirstPhaseHealth;
    public int SecondPhaseHealth;
    public int ThirdPhaseHealth;

    public int CurrentHealth;

    void Awake()
    {
        CurrentHealth = FirstPhaseHealth;
    }

    public void BossGetHit()
    {
        if (CurrentHealth - 1 > 0) CurrentHealth--;
        else BossMana.SwitchPhase();
    }
}
