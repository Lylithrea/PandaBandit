using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralGeneration/Single Tile")]
public class PG_AdvSingleTile : PG_AdvTile
{
    public GameObject tile;
    public TileTypes sideA;
    public TileTypes sideB;
    public TileTypes sideC;
    public TileTypes sideD;

    public override string TileName()
    {
        return tileName;
    }

    public override TileTypes GetSide(int side)
    {
        switch (side)
        {
            case 0:
                return sideA;
            case 1:
                return sideB;
            case 2:
                return sideC;
            case 3:
                return sideD;
            default:
                Debug.LogWarning("Side not supported.");
                return TileTypes.None;

        }
    }


    public override GameObject GetTile()
    {
        return tile;
    }

}
