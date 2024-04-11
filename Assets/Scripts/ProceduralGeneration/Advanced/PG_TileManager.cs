using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class TileRotations
{
    public PG_AdvTile tile;
    public int rotation;
    public TileRotations(PG_AdvTile tile, int rotation)
    {
        this.tile = tile;
        this.rotation = rotation;
    }
}

public class PG_TileManager : MonoBehaviour
{
    public float entropy = 0;
    public int col = 0, row = 0;
    public int rotation = 0;
    public PG_AdvTile tile = null;
    public bool isConnectedToStart = false;

    public List<PG_AdvTile> possibleTiles = new List<PG_AdvTile>();
    public Dictionary<PG_AdvTile, int> possibleRotTiles = new Dictionary<PG_AdvTile, int>();
    public Dictionary<int, PG_AdvTile> randomTiles = new Dictionary<int, PG_AdvTile>();

    public List<TileRotations> tileRotations = new List<TileRotations>();

    public TileTypes sideA;
    public TileTypes sideB;
    public TileTypes sideC;
    public TileTypes sideD;


    //if we arent a tile yet
    //calculate based on surrounding tiles how much entropy we have
    public void UpdateTile()
    {
        if (tile != null) { entropy = 0; return; }

        List<TileTypes> tileTypes = GetNeightbouringTileTypes();

        possibleRotTiles.Clear();
        randomTiles.Clear();
        tileRotations.Clear();

        UpdatePossibleTiles(tileTypes);
        CalculateEntropy();

    }

    public void CalculateEntropy()
    {
        entropy = tileRotations.Count;
    }

    public virtual void UpdatePossibleTiles(List<TileTypes> tileTypes)
    {
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            //find all possible starting points
            //starting points are points where tiletypea is the same as somewhere in the possible tile
            //then check if the following tiles are also correct

            List<int> startingPoints = new List<int>();

            //loop through all 4 sides of the cube
            for (int j = 0; j < tileTypes.Count; j++)
            {
                TileTypes tileSide = TileTypes.None;
                switch (j)
                {
                    case 0: tileSide = possibleTiles[i].GetSide(0); break;
                    case 1: tileSide = possibleTiles[i].GetSide(1); break;
                    case 2: tileSide = possibleTiles[i].GetSide(2); break;
                    case 3: tileSide = possibleTiles[i].GetSide(3); break;
                    default: break;
                }
                if (tileSide == tileTypes[0] || tileTypes[0] == TileTypes.None || tileSide == TileTypes.None)
                {

                    startingPoints.Add(j);
                }
            }

            //for each starting point see if we can fully match it
            //starting points contain which side of the current block we checking
            for (int s = 0; s < startingPoints.Count; s++)
            {
                int combo = 1;
                int start = startingPoints[s];

                //we know the first one is correct so thats why we start on 1
                for (int k = 1; k < tileTypes.Count; k++)
                {
                    //we know the first side is correct
                    //so we go immediatly to the next one
                    start++;
                    if (start > 3) start = 0;

                    TileTypes tileSide = TileTypes.None;
                    switch (start)
                    {
                        case 0: tileSide = possibleTiles[i].GetSide(0); break;
                        case 1: tileSide = possibleTiles[i].GetSide(1); break;
                        case 2: tileSide = possibleTiles[i].GetSide(2); break;
                        case 3: tileSide = possibleTiles[i].GetSide(3); break;
                        default: break;
                    }
                    if (tileTypes[k] == tileSide || tileTypes[k] == TileTypes.None || tileSide == TileTypes.None)
                    {
                        combo++;
                    }
                    else
                    {
                        combo = 0;
                        break;
                    }
                }
                if (combo == tileTypes.Count)
                {
                    int tileRot = startingPoints[s];
                    tileRotations.Add(new TileRotations(possibleTiles[i], tileRot));
                }
            }
        }
    }

    public List<TileTypes> GetNeightbouringTileTypes()
    {
        List<TileTypes> tileTypes = PG_AdvGenerator.instance.GetTypesOfNeighbours(col, row);
        int index = 0;
        foreach (TileTypes tileType in tileTypes)
        {
            switch (index)
            {
                case 0:
                    sideA = tileType;
                    break;
                case 1:
                    sideB = tileType;
                    break;
                case 2:
                    sideC = tileType;
                    break;
                case 3:
                    sideD = tileType;
                    break;
                default:
                    break;
            }

            index++;
        }
        return tileTypes;
    }

    public TileTypes GetSide(int col, int row)
    {
        if (tile == null) return TileTypes.None;
        if (this.col < col)
        {
            return GetSideBasedOnRotation(1);
        }
        else if (this.col > col)
        {
            return GetSideBasedOnRotation(3);
        }
        else if (this.row > row)
        {
            return GetSideBasedOnRotation(2);
        }
        else if (this.row < row)
        {
            return GetSideBasedOnRotation(0);
        }
        return GetSideBasedOnRotation(0);
    }


    public TileTypes GetSideBasedOnRotation(int side)
    {
        int actualSide = side + rotation;
        if (actualSide > 3) actualSide -= 4;
        //actualSide = Mathf.Abs(actualSide);

        switch (actualSide)
        {
            case 0:
                return tile.GetSide(0);
            case 1:
                return tile.GetSide(1);
            case 2:
                return tile.GetSide(2);
            case 3:
                return tile.GetSide(3);
            default:
                Debug.Log("Something went wrong");
                return TileTypes.None;
        }
    }




    public void SetTile()
    {
        entropy = 0;
        int randomTile = Random.Range(0, tileRotations.Count);

        //PG_AdvTile newTile = randomTiles[randomTile];
        var newTile = tileRotations[randomTile].tile;
        this.tile = newTile;
        this.rotation = tileRotations[randomTile].rotation;

        foreach (Transform child in this.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        GameObject tileObj = Instantiate(newTile.GetTile(), this.transform);

        tileObj.transform.Rotate(new Vector3(0, tileRotations[randomTile].rotation * -90, 0));
    }

    public void SetStartTile(List<PG_AdvTile> startTiles)
    {
        entropy = 0;

        possibleTiles.Clear();
        possibleTiles.AddRange(startTiles);
        UpdateTile();
        int randomTile = Random.Range(0, tileRotations.Count);

        //PG_AdvTile newTile = randomTiles[randomTile];
        var newTile = tileRotations[randomTile].tile;
        this.tile = newTile;
        this.rotation = tileRotations[randomTile].rotation;

        foreach (Transform child in this.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        GameObject tileObj = Instantiate(newTile.GetTile(), this.transform);

        tileObj.transform.Rotate(new Vector3(0, tileRotations[randomTile].rotation * -90, 0));
    }

    public void SetEndTile(List<PG_AdvTile> endTiles)
    {
        entropy = 0;

        possibleTiles.Clear();
        possibleTiles.AddRange(endTiles);
        UpdateTile();
        int randomTile = Random.Range(0, tileRotations.Count);

        //PG_AdvTile newTile = randomTiles[randomTile];
        var newTile = tileRotations[randomTile].tile;
        this.tile = newTile;
        this.rotation = tileRotations[randomTile].rotation;

        foreach (Transform child in this.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        GameObject tileObj = Instantiate(newTile.GetTile(), this.transform);

        tileObj.transform.Rotate(new Vector3(0, tileRotations[randomTile].rotation * -90, 0));
    }

}
