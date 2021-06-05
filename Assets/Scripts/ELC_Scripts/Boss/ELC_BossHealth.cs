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
    public bool IsStunned;
    public bool HaveShield;
    public float shieldRecoveryTime;
    public GameObject ShieldGO;

    public int CurrentHealth;

    

    void Awake()
    {
        CurrentHealth = FirstPhaseHealth;
        BossMana = this.GetComponent<ELC_BossManager>();
    }

    public void BossGetHit()
    {
        if(!BossMana.IsInSwitchPhase || !HaveShield)
        {
            Debug.Log("tap�");
            BossMana.music.SwitchMusicPart(0);
            if (CurrentHealth - 1 > 0) CurrentHealth--;
            else
            {
                IsStunned = true;
                BossMana.isStunned = IsStunned;
                BossMana.BossAttacks.EndAttack();
            }
        }
    }

    public IEnumerator ShieldLostAndRecover()
    {
        HaveShield = false;
        ShieldGO.GetComponent<Animator>().SetBool("Dissolve", true);
        yield return new WaitForSeconds(1);
        ShieldGO.GetComponent<Animator>().SetBool("Dissolve", false);
        ShieldGO.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(shieldRecoveryTime);
        if (!BossMana.IsInSwitchPhase && BossMana.CurrentPhase == 2)
        {
            HaveShield = true;
            ShieldGO.GetComponent<ParticleSystem>().Play();
        }
    }

    public void Pacificate()
    {
        if(IsStunned)
        {
            IsStunned = false;
            BossMana.isStunned = IsStunned;
            if (BossMana.CurrentPhase < 3) BossMana.SwitchPhase();
            else BossMana.End();
        }
    }
}
