using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AXD_HP : MonoBehaviour
{
    public Sprite HPEmpty;
    public Sprite HPFull;
    private Image HPToDisplay;

    private void Awake()
    {
        HPToDisplay = GetComponent<Image>();
    }

    public void FullfillHP()
    {
        HPToDisplay.sprite = HPFull;
    }

    public void EmptyHP()
    {
        HPToDisplay.sprite = HPEmpty;
    }

}
