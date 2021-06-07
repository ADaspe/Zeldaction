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

    private void Start()
    {
        BasicTile = tilesScript.BasicTile;
        CloseBridge(true);
    }

    //private void Update()
    //{
    //    CheckActivations();
    //}



    public void OpenBridge()
    {
        isOpen = true;
        Tile detectedTilesColor = tilesScript.detectedTilecolor;
        detectedTiles = tilesScript.OverridedTiles(this.transform);
        foreach (Vector3Int tilePos in detectedTiles)
        {
            tilesScript.TileMap.SetTile(tilePos, detectedTilesColor);
        }
        ObjectAnimator.SetBool("Activated", true);
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
            ObjectAnimator.SetBool("Activated", false);
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
        if (!DialogActivation) CheckActivations();
        else OpenBridge();
    }
    
}
