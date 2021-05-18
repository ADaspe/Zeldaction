using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//[ExecuteInEditMode]
public class ELC_TilemapSwitch : MonoBehaviour
{
    public Tilemap TilemapsToSwitch;
    public Tilemap TargetTilemap;
    [SerializeField]
    List<TileInfos> allTilesInfos = new List<TileInfos>();

    public void EngageSwitchProcess()
    {
        allTilesInfos.Clear();
        DetectTiles();
        Debug.Log("update tiles");
    }

    public void RevertSwitch()
    {
        foreach (var item in allTilesInfos)
        {
            Vector3Int ancientTargetTM = TargetTilemap.WorldToCell(item.WorldCoordinates);
            TargetTilemap.SetTile(ancientTargetTM, null);
            Vector3Int ancientCoord = TilemapsToSwitch.WorldToCell(item.WorldCoordinates);
            TilemapsToSwitch.SetTile(ancientCoord, item.tile);
        }
    }

    private void DetectTiles()
    {
        BoundsInt size = TilemapsToSwitch.cellBounds;

        foreach (var pos in TilemapsToSwitch.cellBounds.allPositionsWithin)
        {

            if (TilemapsToSwitch.HasTile(pos))
            {
                Vector3 cellLocalPos = TilemapsToSwitch.CellToWorld(pos);
                TileInfos infos = new TileInfos();
                infos.tile = TilemapsToSwitch.GetTile(pos);
                infos.WorldCoordinates = cellLocalPos;
                allTilesInfos.Add(infos);
            }
        }
        SwitchTilemap();
        TilemapsToSwitch.ClearAllTiles();
    }


    private void SwitchTilemap()
    {
        foreach (var item in allTilesInfos)
        {
            Vector3Int cellCoord = TargetTilemap.WorldToCell(item.WorldCoordinates);
            TargetTilemap.SetTile(cellCoord, item.tile);
        }
    }
}


[System.Serializable]
public class TileInfos
{
    public TileBase tile;
    public Vector3 WorldCoordinates;
}
