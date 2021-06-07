using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ELC_Bridge : AXD_Activable
{
    public ELC_TilesDetection tilesScript;
    private List<Vector3Int> detectedTiles;
    [SerializeField]
    private bool isOpen;
    private Tile BasicTile;
    public bool ActivateOnDisable;
    public bool DialogActivation;
    public bool persistentActivation;
    public Animator[] animators;

    private void Start()
    {
        ObjectAnimator = this.GetComponent<Animator>();
        animators = GetComponentsInChildren<Animator>();
        BasicTile = tilesScript.BasicTile;
        if (!isActivated) CloseBridge(true);
        else OpenBridge();
        InvokeRepeating("Check", 1f, 1f);
    }

    //private void Update()
    //{
    //    CheckActivations();
    //}

    private void Check()
    {
        if(this.gameObject.activeInHierarchy)
        {
            if (isOpen) AnimatorChange("Activated", true);
            else AnimatorChange("Activated", false);
        }

    }

    public void OpenBridge()
    {
        isOpen = true;
        Tile detectedTilesColor = tilesScript.detectedTilecolor;
        detectedTiles = tilesScript.OverridedTiles(this.transform);
        foreach (Vector3Int tilePos in detectedTiles)
        {
            tilesScript.TileMap.SetTile(tilePos, detectedTilesColor);
        }
        AnimatorChange("Activated", true);
    }

    public void CloseBridge(bool start = false)
    {
        if(!DialogActivation || start)
        {
            detectedTiles = tilesScript.OverridedTiles(this.transform);
            isOpen = false;
            foreach (Vector3Int tilePos in detectedTiles)
            {
                tilesScript.TileMap.SetTile(tilePos, BasicTile);
            }
            AnimatorChange("Activated",false);
        }
        
    }

    public void CheckActivations()
    {
        if(!DialogActivation)
        {
            int currentNumberOfActivation = 0;

            foreach (ELC_Activation active in ActivationsNeeded)
            {
                if ((!ActivateOnDisable && active.isActivated) || (ActivateOnDisable && !active.isActivated)) currentNumberOfActivation++;
            }
            //Debug.Log(currentNumberOfActivation);

            if (currentNumberOfActivation == ActivationsNeeded.Count)
            {
                if (!isOpen)
                {
                    OpenBridge();
                }
            }
            else if (isOpen)
            {
                CloseBridge();
            }
        }
    }

    public override void Activate()
    {
        if (!DialogActivation && !persistentActivation) CheckActivations();
        else OpenBridge();
    }
    
    void AnimatorChange(string boolToChange, bool activate)
    {
        for(int i = 0; i < animators.Length; i++)
        {
            animators[i].SetBool(boolToChange,activate);
        }
    }

}
