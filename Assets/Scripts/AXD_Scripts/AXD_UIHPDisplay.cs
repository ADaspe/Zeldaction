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
        allHPs[charaManager.currentHP - 1].EmptyHP();
    }

    public void HealLife()
    {
        allHPs[charaManager.currentHP - 1].FullfillHP();
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
            allHPs[charaManager.maxHP] = (Instantiate(HPPrefab, this.transform).GetComponent<AXD_HP>());
        }
    }

}
