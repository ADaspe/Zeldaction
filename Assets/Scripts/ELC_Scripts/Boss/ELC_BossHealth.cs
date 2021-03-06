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
    public Material StunMaterial;
    [HideInInspector]
    public Material BasicMat;

    public int CurrentHealth;

    

    void Awake()
    {
        CurrentHealth = FirstPhaseHealth;
        BossMana = this.GetComponent<ELC_BossManager>();
        BasicMat = this.GetComponent<SpriteRenderer>().material;
    }

    public void BossGetHit()
    {
        if(!BossMana.IsInSwitchPhase && !HaveShield)
        {
            Debug.Log("tap�");
            BossMana.music.SwitchMusicPart(0);
            if (CurrentHealth - 1 > 0) CurrentHealth--;
            else
            {
                SpriteRenderer SpriteRend = this.GetComponent<SpriteRenderer>();
                BossMana.BossAttacks.StopAllCoroutines();
                var mat = SpriteRend.material.color;
                mat.a = 1;
                SpriteRend.color = mat;
                BossMana.gameManager.audioManager.Play("Boss_Paralyzed");
                IsStunned = true;
                this.GetComponent<SpriteRenderer>().material = StunMaterial;
                this.GetComponent<Animator>().SetBool("isStun", true);
                BossMana.isStunned = IsStunned;
                BossMana.BossAttacks.EndAttack();
                
                
                //if(BossMana.CurrentPhase == 1) StartCoroutine(BossMana.BossAttacks.Fade(true));
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
        if (!BossMana.IsInSwitchPhase && BossMana.CurrentPhase == 2 && !IsStunned)
        {
            HaveShield = true;
            ShieldGO.GetComponent<ParticleSystem>().Play();
            ShieldGO.GetComponent<Animator>().SetBool("ActivateShield", true);
        }
        if(BossMana.CurrentPhase == 3 && !IsStunned && !BossMana.IsInSwitchPhase)
        {
            BossMana.BossAttacks.isTired = false;
            HaveShield = true;
            ShieldGO.GetComponent<ParticleSystem>().Play();
            ShieldGO.GetComponent<Animator>().SetBool("ActivateShield", true);
        }
    }

    public void Pacificate()
    {
        if(IsStunned)
        {
            IsStunned = false;
            BossMana.isStunned = IsStunned;
            this.GetComponent<SpriteRenderer>().material = BasicMat;
            this.GetComponent<Animator>().SetBool("isStun", false);
            if (BossMana.CurrentPhase < 3) BossMana.SwitchPhase();
            else
            {
                BossMana.End();
                Debug.Log("end");
            }
            
        }
    }
}
