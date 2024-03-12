using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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


        //get neighbouring tiles
        //get their side
        //based on their side calculate entropy
        Debug.Log(PG_AdvGenerator.instance);
        /*
        List<GameObject> neighbours = PG_AdvGenerator.instance.GetNeighbours(col, row);

        Debug.Log("Neighbours: " + neighbours.Count);


        List<TileTypes> tileTypes = new List<TileTypes>();
        foreach (GameObject neighbour in neighbours)
        {
            TileTypes type = neighbour.GetComponent<PG_TileManager>().GetSide(col, row);
            tileTypes.Add(type);
        }
        */
        List<TileTypes> tileTypes = PG_AdvGenerator.instance.GetTypesOfNeighbours(col, row);
        int index = 0;
        foreach (TileTypes tileType in tileTypes)
        {
            Debug.Log(tileType);

            switch(index)
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


        Debug.Log("Actual neighbours: " + tileTypes.Count);
        //always a, b ,c ,d


        possibleRotTiles.Clear();
        randomTiles.Clear();
        tileRotations.Clear();
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            //find all possible starting points
            //starting points are points where tiletypea is the same as somewhere in the possible tile
            //then check if the following tiles are also correct

            List<int> startingPoints = new List<int>();

            //loop through all 4 sides of the cube
            for(int j = 0; j < tileTypes.Count; j++)
            {
                TileTypes tileSide = TileTypes.None;
                switch (j)
                {
                    case 0: tileSide = possibleTiles[i].sideA; break;
                    case 1: tileSide = possibleTiles[i].sideB; break;
                    case 2: tileSide = possibleTiles[i].sideC; break;
                    case 3: tileSide = possibleTiles[i].sideD; break;
                    default: break;
                }
                if (tileSide == tileTypes[0] || tileTypes[0] == TileTypes.None || tileSide == TileTypes.None)
                {
                    //Debug.Log("Found a tile that could match: " + tileSide + " tiletype: " + tileTypes[0] + " with starting point: " + j);
                    startingPoints.Add(j);
                }
            }
            //Debug.Log("StartingPoints: " + startingPoints.Count);
            //for each starting point see if we can fully match it
            //starting points contain which side of the current block we checking
            for (int s = 0; s < startingPoints.Count; s++)
            {
                int combo = 1;
                int start = startingPoints[s];
                
                //we know the first one is correct so thats why we start on 1
                for(int k = 1; k < tileTypes.Count; k++)
                {
                    //we know the first side is correct
                    //so we go immediatly to the next one
                    start++;
                    if (start > 3) start = 0;

                    TileTypes tileSide = TileTypes.None;
                    switch (start)
                    {
                        case 0: tileSide = possibleTiles[i].sideA; break;
                        case 1: tileSide = possibleTiles[i].sideB; break;
                        case 2: tileSide = possibleTiles[i].sideC; break;
                        case 3: tileSide = possibleTiles[i].sideD; break;
                        default: break;
                    }
                    //Debug.Log("Check side from start (so filling in cube): " + tileSide + " against side of surrounding tiles (starts with B): " + tileTypes[k]);
                    //tileside == tiletypes.none should never be the case, as it should always have a side
                    if (tileTypes[k] == tileSide || tileTypes[k] == TileTypes.None || tileSide == TileTypes.None)
                    {
                        combo++;
                    }
                    else
                    {
                        combo = 0;
                        //Debug.Log("We yeeted this tile");
                        break;
                    }
                }
                if (combo == tileTypes.Count)
                {
                    //it is possible! :D
                    //Debug.Log("We found a possible tile!");
                    //int tileRot = (startingPoints[s] - rotation + 4) % 4;
                    int tileRot = startingPoints[s];
                    //possibleRotTiles.Add(possibleTiles[i], tileRot);
                    //randomTiles.Add(randomTiles.Count, possibleTiles[i]);
                    //Debug.Log("possible tile: " + possibleTiles[i] + " with rot: " + tileRot);
                    tileRotations.Add(new TileRotations(possibleTiles[i], tileRot));
                }
            }
        }
        //Debug.Log("Tile rotations count: " + tileRotations.Count);
        entropy = tileRotations.Count;


    }

    public TileTypes GetSide(int col, int row)
    {
        Debug.Log("Trying to get side of: " + tile);
        if (tile == null) return TileTypes.None;
        if (this.col < col)
        {
            Debug.Log("TileB is to the right of TileA.");
            return GetSideBasedOnRotation(1);
        }
        else if (this.col > col)
        {
            Debug.Log("TileB is to the left of TileA.");
            return GetSideBasedOnRotation(3);
        }
        else if (this.row > row)
        {
            Debug.Log("TileB is below TileA.");
            return GetSideBasedOnRotation(2);
        }
        else if (this.row < row)
        {
            
            Debug.Log("TileB is above TileA.");
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
                Debug.Log(tile.sideA);
                return tile.sideA;
            case 1:
                Debug.Log(tile.sideB);
                return tile.sideB;
            case 2:
                Debug.Log(tile.sideC);
                return tile.sideC;
            case 3:
                Debug.Log(tile.sideD);
                return tile.sideD;
            default:
                
                Debug.Log("Something went wrong");
                return tile.sideA;
        }
    }




    public void SetTile(PG_AdvTile tile)
    {
        //this.tile = tile;
        /*
        int randomTile = Random.Range(0,randomTiles.Count);
        if (!possibleRotTiles.ContainsKey(randomTiles[randomTile]))
        {
            Debug.Log("Somehow tile key does not exist");
            return;
        }*/

        int randomTile = Random.Range(0, tileRotations.Count);

        //PG_AdvTile newTile = randomTiles[randomTile];
        PG_AdvTile newTile = tileRotations[randomTile].tile;
        this.tile = newTile;
        this.rotation = tileRotations[randomTile].rotation;

        foreach (Transform child in this.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        GameObject tileObj = Instantiate(newTile.tile, this.transform);

        tileObj.transform.Rotate(new Vector3(0, tileRotations[randomTile].rotation * -90, 0));

        Debug.Log("Tile spawned with rotation: " + tileRotations[randomTile].rotation);
        Debug.Log("Tile spawned with rotation: " + tileRotations[randomTile].rotation * -90);
        Debug.Log("Side A: " + GetSideBasedOnRotation(0));
        Debug.Log("Side B: " + GetSideBasedOnRotation(1));
        Debug.Log("Side C: " + GetSideBasedOnRotation(2));
        Debug.Log("Side D: " + GetSideBasedOnRotation(3));
        
        
    }



}
