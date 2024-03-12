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

    public static PG_AdvGenerator instance;

    
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
    }

    [Button]
    public void Generate()
    {
        foreach (GameObject tile in generatedTiles)
        {
            DestroyImmediate(tile);
        }
        generatedTiles.Clear();
        uncompletedTiles.Clear();
        lowestPossibilitiesTile.Clear();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject newTile = Instantiate(baseTile, this.transform);
                SetupTile(newTile, i, j);

                generatedTiles.Add(newTile);
                lowestPossibilitiesTile.Add(newTile);
                uncompletedTiles.Add(newTile);
            }
        }
        for(int j =  0; j < generatedTiles.Count; j++)
        {
            generatedTiles[j].GetComponent<PG_TileManager>().UpdateTile();
        }
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

            int randomTile = Random.Range(0, lowestPossibilitiesTile.Count);

            int random = Random.Range(0, lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().possibleTiles.Count);

            Debug.Log("Random : " + random);
            Debug.Log("Random Count: " + lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().possibleTiles.Count);

            PG_AdvTile tile = lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().possibleTiles[random];
            lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().SetTile(tile);

            updateSurroundingTiles(lowestPossibilitiesTile[randomTile]);

            uncompletedTiles.Remove(lowestPossibilitiesTile[randomTile]);

            SortTiles();
        }

    }

    public void updateSurroundingTiles(GameObject tile)
    {
        int col = tile.GetComponent<PG_TileManager>().col;
        int row = tile.GetComponent<PG_TileManager>().row;

        Debug.Log("updating tile position: " + col + ", " + row);


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

        Debug.Log("Out position: " + col + ", " + row);

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

        Debug.Log("Our position: " + col + ", " + row);


        if (row + 1 < rows)
        {
            Debug.Log("Inside boundaries, under row");
            neighbours.Add(generatedTiles[col * columns + (row + 1)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            Debug.Log("Outside boundaries, over row");
            neighbours.Add(TileTypes.None);
        }
        if (col + 1 < columns)
        {
            Debug.Log("Inside boundaries, under col");
            neighbours.Add(generatedTiles[(col + 1) * columns + row].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            Debug.Log("Outside boundaries, over col");
            neighbours.Add(TileTypes.None);
        }
        if (row - 1 >= 0)
        {
            Debug.Log("Inside boundaries, over row");
            neighbours.Add(generatedTiles[col * columns + (row - 1)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else 
        {
            Debug.Log("Outside boundaries, under row");
            neighbours.Add(TileTypes.None);
        }
        if (col - 1 >= 0)
        {
            Debug.Log("Inside boundaries, over col");
            neighbours.Add(generatedTiles[(col - 1) * columns + row].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            Debug.Log("Outside boundaries, under col");
            neighbours.Add(TileTypes.None);
        }

        return neighbours;
    }


}
