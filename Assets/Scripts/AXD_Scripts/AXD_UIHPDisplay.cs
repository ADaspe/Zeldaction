using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AXD_UIHPDisplay : MonoBehaviour
{
    public ELC_CharacterManager charaManager;
    public GameObject HPPrefab;
    public AXD_HP[] allHPs ;

    private void Start()
    {
        allHPs = new AXD_HP[6];
        for (int i = 0; i < charaManager.currentHP; i++)
        {
            allHPs[i] = (Instantiate(HPPrefab, this.transform).GetComponent<AXD_HP>());

        }
    }
    public void LoseLife()
    {
        if (charaManager.currentHP >= 0) allHPs[charaManager.currentHP].EmptyHP();
        else Debug.Log("HP à 0");
    }

    [Button]
    public void HealLife()
    {
        if(charaManager.currentHP < charaManager.maxHP)
        {
            allHPs[charaManager.currentHP - 1].FullfillHP();
            charaManager.currentHP++;
        }
    }

    public void HealFullLife()
    {
        for (int i = 0; i < allHPs.Length; i++)
        {
            if (allHPs[i] != null)
            {
                allHPs[i].FullfillHP();
            }
        }
    }

    [Button]
    public void AddLife()
    {
        if (charaManager.maxHP < 6)
        {
            
            charaManager.maxHP++;
            allHPs[charaManager.maxHP -1] = (Instantiate(HPPrefab, this.transform).GetComponent<AXD_HP>());
            HealLife();
            charaManager.gameManager.audioManager.Play("Jingle_Ryn");
        }
    }

}
