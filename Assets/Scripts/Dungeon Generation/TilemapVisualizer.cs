using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap;

    [SerializeField] private TileBase floorTile1, floorTile2, wallTop1, wallTop2,
                    wallSideRight1, wallSideRight2, wallSideLeft1, wallSideLeft2,
                    wallBottom1, wallBottom2,
                    wallInnerCornerDownLeft, wallInnerCornerDownRight, 
                    wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, 
                    wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft,
                    wallFull;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile1);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    public void PaintSingleBasicWall(Vector2Int wallPosition, String binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = Random.value > 0.5f ? wallTop1 : wallTop2;
        }
        else if(WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = Random.value > 0.5f ? wallSideRight1 : wallSideRight2;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = Random.value > 0.5f ? wallSideLeft1 : wallSideLeft2;
        }
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
        {
            tile = Random.value > 0.5f ? wallBottom1 : wallBottom2;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, wallPosition);
        }
    }

    internal void PaintSingleCornerWall(Vector2Int wallPosition, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, wallPosition);
        }
    }
}
