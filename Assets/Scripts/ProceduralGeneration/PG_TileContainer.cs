using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PG_TileContainer : MonoBehaviour
{

    public PG_Tile tile;
    public int possibilities;
    public int col;
    public int row;

    public List<PG_Tile> possibleTiles = new List<PG_Tile>();

    public void SetTile(PG_Tile tile)
    {
        this.tile = tile;
        this.GetComponent<MeshRenderer>().material = tile.tile;
        possibilities = 0;
        possibleTiles.Clear();
    }


    public void UpdatePossibilities(PG_Tile tile)
    {
        for (int i = 0; i < tile.illegalTiles.Length; i++)
        {
            for(int j = 0; j < possibleTiles.Count; j++)
            {
                if (tile.illegalTiles[i] == possibleTiles[j])
                {
                    Debug.Log("Found a illegal tile, removing it now...: " + tile.illegalTiles[i].tileName);
                    possibleTiles.Remove(possibleTiles[j]);
                    possibilities--;
                    j--;
                    continue;
                }
            }
        }

    }

}
