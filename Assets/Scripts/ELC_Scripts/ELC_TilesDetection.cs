using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ELC_TilesDetection : MonoBehaviour
{
    public Tilemap TileMap;
    public Tile detectedTilecolor;
    

    public List<Vector3Int> OverridedTiles(Transform objectTransform)
    {
        List<Vector3Int> detectedTiles = new List<Vector3Int>();

        //On prend la position de l'objet
        Vector3 position = objectTransform.position;

        //On déduit la position des angles avec le scale
        float scaleX = objectTransform.localScale.x;
        float scaleY = objectTransform.localScale.y;
        Vector3 TopLeftAnglePos = new Vector3(position.x - scaleX / 2, position.y + scaleY / 2);
        Vector3 TopRightAnglePos = new Vector3(position.x + scaleX / 2, position.y + scaleY / 2);
        Vector3 BottomLeftAnglePos = new Vector3(position.x - scaleX / 2, position.y - scaleY / 2);
        Vector3 BottomRightAnglePos = new Vector3(position.x + scaleX / 2, position.y - scaleY / 2);

        //On détecte les tiles qui sont au niveau des angles
        Vector3Int TopLeftTile = TileMap.layoutGrid.WorldToCell(TopLeftAnglePos);
        Vector3Int TopRightTile = TileMap.layoutGrid.WorldToCell(TopRightAnglePos);
        Vector3Int BottomLeftTile = TileMap.layoutGrid.WorldToCell(BottomLeftAnglePos);
        Vector3Int BottomRightTile = TileMap.layoutGrid.WorldToCell(BottomRightAnglePos);

        int length = TopRightTile.x - TopLeftTile.x;
        int height = TopLeftTile.y - BottomLeftTile.y;

        for (int i = 0; i < length +1; i++)
        {
            for (int h = 0; h < height + 1; h++)
            {
                detectedTiles.Add(TileMap.layoutGrid.WorldToCell(new Vector3(BottomLeftTile.x + i * TileMap.layoutGrid.cellSize.x, BottomLeftTile.y + h * TileMap.layoutGrid.cellSize.y)));
            }
        }


        //On enregistre ces tiles + ceux qui sont entre et on les return
        Debug.Log("Nombre de tiles détectées sur le pont: " + detectedTiles.Count);
        return detectedTiles;
    }

}
