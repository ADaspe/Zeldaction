using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ELC_Bridge : AXD_Activable
{
    public ELC_TilesDetection tilesScript;
    private List<Vector3Int> detectedTiles;
    private bool isOpen;


    public void OpenBridge()
    {
        isOpen = true;
        Tile detectedTilesColor = tilesScript.detectedTilecolor;
        detectedTiles = tilesScript.OverridedTiles(this.transform);

        foreach (Vector3Int tilePos in detectedTiles)
        {
            tilesScript.TileMap.SetTile(tilePos, detectedTilesColor);
        }
    }

    public void CheckActivations()
    {
        int currentNumberOfActivation = 0;

        foreach (ELC_Activation active in ActivationsNeeded)
        {
            if (active.isActivated) currentNumberOfActivation++;
        }

        if (currentNumberOfActivation == ActivationsNeeded.Count)
        {
            OpenBridge();
            return;
        }
    }

    public override void Activate()
    {
        if(!isOpen) CheckActivations();
    }
    
}
