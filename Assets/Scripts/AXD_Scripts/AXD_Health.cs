using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AXD_Health : MonoBehaviour
{
    public ELC_CharacterManager CharacterManager;
    public AXD_UIHPDisplay hpDisplay;
    public bool Invincible;
    public float InvincibilityTime;

    [Button]
    public void GetHit()
    {
        if (this.CompareTag("Ryn"))
        {
            if (!Invincible && !CharacterManager.RynAttack.ShieldOn && !CharacterManager.invincibilityCheat)
            {
                Invincible = true;
                if (CharacterManager.TakeDamage(this.tag))
                {
                    hpDisplay.LoseLife();
                    CharacterManager.gameManager.audioManager.Play("Ryn_Hurt"+Random.Range(1,6));
                }
                Invoke("Vulnerable", InvincibilityTime);
            }else if (CharacterManager.RynAttack.ShieldOn)
            {
                CharacterManager.gameManager.audioManager.Play("Shield_Impact");
            }
        }else if (this.CompareTag("Spirit"))
        {
            //Appliquer feedback
            CharacterManager.gameManager.audioManager.Play("Spirit_Disappear");
            GetComponent<ELC_SpiritIdle>().Teleport();

        }
    }

    void Vulnerable()
    {
        Invincible = false;
    }

    public void FullHeal()
    {
        CharacterManager.currentHP = CharacterManager.maxHP;
        hpDisplay.HealFullLife();
    }
}
