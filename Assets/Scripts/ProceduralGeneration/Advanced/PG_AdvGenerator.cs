using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[ExecuteInEditMode]
public class PG_AdvGenerator : MonoBehaviour
{

    public List<PG_AdvTile> allTiles = new List<PG_AdvTile>();
    public GameObject baseTile;

    public int columns = 0, rows = 0;



    public List<GameObject> generatedTiles = new List<GameObject>();

    public List<GameObject> lowestPossibilitiesTile = new List<GameObject>();
    public List<GameObject> uncompletedTiles = new List<GameObject>();
    public List<GameObject> neighbouringTiles = new List<GameObject>();

    public static PG_AdvGenerator instance;

    public List<PG_AdvTile> startTiles = new List<PG_AdvTile>();
    public List<PG_AdvTile> endTiles = new List<PG_AdvTile>();
    public int minLength = 4;
    public int maxLength = 6;
    [Foldout("Start Tile Settings")] public int minXPosition = 0;
    [Foldout("Start Tile Settings")] public int maxXPosition = 1;
    [Foldout("Start Tile Settings")] public int minYPosition = 0;
    [Foldout("Start Tile Settings")] public int maxYPosition = 1;

    private PG_TileManager startTilesManager;
    private List<PG_TileManager> endTilesManagers = new List<PG_TileManager>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OnValidate()
    {
        Awake();
    }



    [Button]
    public void RemoveTiles()
    {
        foreach (GameObject tile in generatedTiles)
        {
            DestroyImmediate(tile);
        }
        generatedTiles.Clear();
        uncompletedTiles.Clear();
        lowestPossibilitiesTile.Clear();
        startTilesManager = null;
        endTilesManagers.Clear();
        neighbouringTiles.Clear();
    }

    [Button]
    public void Generate()
    {
        RemoveTiles();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject newTile = Instantiate(baseTile, this.transform);
                SetupTile(newTile, i, j);

                generatedTiles.Add(newTile);
                uncompletedTiles.Add(newTile);
            }
        }
        for(int j =  0; j < generatedTiles.Count; j++)
        {
            generatedTiles[j].GetComponent<PG_TileManager>().UpdateTile();
        }

        SortTiles();
    }

    private void SetupTile(GameObject tile, int col, int row)
    {
        PG_TileManager manager = tile.GetComponent<PG_TileManager>();
        if (manager == null) return;

        manager.col = col;
        manager.row = row;

        foreach (PG_AdvTile tile2 in allTiles)
        {
            tile.GetComponent<PG_TileManager>().possibleTiles.Add(tile2);
        }

        tile.transform.position = new Vector3(col, 0, row);
    }



    public int steps = 1;

    [Button]
    public void NextStep()
    {
        for (int i = 0; i < steps; i++)
        {
            if (generatedTiles.Count == 0) return;

            if (startTilesManager == null)
            {
                int randomStartTile = Random.Range(0, startTiles.Count);

                lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().SetTile();

                neighbouringTiles.AddRange(GetNeighbours(lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().row));
                neighbouringTiles.Remove(lowestPossibilitiesTile[randomStartTile]);

                updateSurroundingTiles(lowestPossibilitiesTile[randomStartTile]);

                uncompletedTiles.Remove(lowestPossibilitiesTile[randomStartTile]);
                return;
            }

            int randomTile = Random.Range(0, lowestPossibilitiesTile.Count);

            lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().SetTile();

            neighbouringTiles.AddRange(GetNeighbours(lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().row));
            neighbouringTiles.Remove(lowestPossibilitiesTile[randomTile]);

            updateSurroundingTiles(lowestPossibilitiesTile[randomTile]);

            uncompletedTiles.Remove(lowestPossibilitiesTile[randomTile]);

            SortTiles();
        }

    }

    public void updateSurroundingTiles(GameObject tile)
    {
        int col = tile.GetComponent<PG_TileManager>().col;
        int row = tile.GetComponent<PG_TileManager>().row;


        if (col - 1 >= 0)
        {
            if (uncompletedTiles.Contains(generatedTiles[(col - 1) * columns + row]))
            {
                generatedTiles[(col - 1) * columns + row].GetComponent<PG_TileManager>().UpdateTile();
            }

        }
        if (col + 1 < columns)
        {
            if (uncompletedTiles.Contains(generatedTiles[(col + 1) * columns + row]))
            {
                generatedTiles[(col + 1) * columns + row].GetComponent<PG_TileManager>().UpdateTile();
            }

        }
        if (row - 1 >= 0)
        {
            if (uncompletedTiles.Contains(generatedTiles[col * columns + (row - 1)]))
            {
                generatedTiles[col * columns + (row - 1)].GetComponent<PG_TileManager>().UpdateTile();
            }

        }
        if (row + 1 < rows)
        {
            if (uncompletedTiles.Contains(generatedTiles[col * columns + (row + 1)]))
            {
                generatedTiles[col * columns + (row + 1)].GetComponent<PG_TileManager>().UpdateTile();
            }

        }

    }


    public float lowestValue = 100;
    public void SortTiles()
    {
        lowestPossibilitiesTile.Clear();
        lowestValue = 100;
        if (startTilesManager == null)
        {
            int startX = Random.Range(minXPosition, maxXPosition);
            int startY = Random.Range(minYPosition, maxYPosition);
            GameObject startTile = generatedTiles[(startX - 1) * columns + startY];
            lowestPossibilitiesTile.Add(startTile);
            neighbouringTiles.Add(startTile);
            return;
        }

        foreach (GameObject neighbour in neighbouringTiles)
        {
            if (lowestPossibilitiesTile.Count == 0)
            {
                lowestPossibilitiesTile.Add(neighbour);
                lowestValue = neighbour.GetComponent<PG_TileManager>().entropy;
            }
            if (neighbour.GetComponent<PG_TileManager>().entropy == lowestValue)
            {
                lowestPossibilitiesTile.Add(neighbour);
            }
            if (neighbour.GetComponent<PG_TileManager>().entropy < lowestValue)
            {
                lowestPossibilitiesTile.Clear();
                lowestPossibilitiesTile.Add(neighbour);
                lowestValue = neighbour.GetComponent<PG_TileManager>().entropy;
            }
        }
        return;

        foreach (GameObject uncompletedTile in uncompletedTiles)
        {
            if (lowestPossibilitiesTile.Count == 0)
            {
                lowestPossibilitiesTile.Add(uncompletedTile);
                lowestValue = uncompletedTile.GetComponent<PG_TileManager>().entropy;
            }
            if (uncompletedTile.GetComponent<PG_TileManager>().entropy == lowestValue)
            {
                lowestPossibilitiesTile.Add(uncompletedTile);
            }
            if (uncompletedTile.GetComponent<PG_TileManager>().entropy < lowestValue)
            {
                lowestPossibilitiesTile.Clear();
                lowestPossibilitiesTile.Add(uncompletedTile);
                lowestValue = uncompletedTile.GetComponent<PG_TileManager>().entropy;
            }
        }
    }


    public List<GameObject> GetNeighbours(int col, int row)
    {
        List<GameObject> neighbours = new List<GameObject>();

        if (col - 1 >= 0)
        {
            neighbours.Add(generatedTiles[(col - 1) * columns + row]);
        }
        if (col + 1 < columns)
        {
            neighbours.Add(generatedTiles[(col + 1) * columns + row]);
        }
        if (row - 1 >= 0)
        {
            neighbours.Add(generatedTiles[col * columns + (row - 1)]);
        }
        if (row + 1 < rows)
        {
            neighbours.Add(generatedTiles[col * columns + (row + 1)]);
        }


        return neighbours;
    }

    public List<TileTypes> GetTypesOfNeighbours(int col, int row)
    {
        List<TileTypes> neighbours = new List<TileTypes>();


        if (row + 1 < rows)
        {
            neighbours.Add(generatedTiles[col * columns + (row + 1)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.Empty);
        }
        if (col + 1 < columns)
        {
            neighbours.Add(generatedTiles[(col + 1) * columns + row].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.Empty);
        }
        if (row - 1 >= 0)
        {
            neighbours.Add(generatedTiles[col * columns + (row - 1)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else 
        {
            neighbours.Add(TileTypes.Empty);
        }
        if (col - 1 >= 0)
        {
            neighbours.Add(generatedTiles[(col - 1) * columns + row].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.Empty);
        }

        return neighbours;
    }


}
