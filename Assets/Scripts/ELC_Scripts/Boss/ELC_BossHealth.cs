using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossHealth : MonoBehaviour
{
    public int Alexandre;
    [HideInInspector]
    public ELC_BossManager BossMana;

    public int FirstPhaseHealth;
    public int SecondPhaseHealth;
    public int ThirdPhaseHealth;

    public int CurrentHealth;

    

    void Awake()
    {
        CurrentHealth = FirstPhaseHealth;
        BossMana = this.GetComponent<ELC_BossManager>();
    }

    public void BossGetHit()
    {
        Debug.Log("tapé");
        BossMana.music.SwitchMusicPart(0);
        if (CurrentHealth - 1 > 0) CurrentHealth--;
        else BossMana.SwitchPhase();
    }
}
