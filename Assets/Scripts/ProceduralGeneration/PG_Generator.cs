using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PG_Generator : MonoBehaviour
{

    public PG_Tile[] allTiles;
    public GameObject baseTile;

    public int columns;
    public int rows;


    public List<GameObject> generatedTiles = new List<GameObject>();

    public List<GameObject> lowestPossibilitiesTile = new List<GameObject>();
    public List<GameObject> uncompletedTiles = new List<GameObject>();

    [Button]
    public void Generate()
    {
        foreach(GameObject tile in generatedTiles)
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

    }

    private void SetupTile(GameObject tile, int col, int row)
    {
        tile.GetComponent<PG_TileContainer>().possibilities = allTiles.Length;
        tile.GetComponent<PG_TileContainer>().col = col;
        tile.GetComponent<PG_TileContainer>().row = row;
        foreach (PG_Tile tile2 in allTiles)
        {
            tile.GetComponent<PG_TileContainer>().possibleTiles.Add(tile2);
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

            int random = Random.Range(0, lowestPossibilitiesTile[randomTile].GetComponent<PG_TileContainer>().possibleTiles.Count);

            Debug.Log("Random : " + random);
            Debug.Log("Random Count: " + lowestPossibilitiesTile[randomTile].GetComponent<PG_TileContainer>().possibleTiles.Count);

            PG_Tile tile = lowestPossibilitiesTile[randomTile].GetComponent<PG_TileContainer>().possibleTiles[random];
            lowestPossibilitiesTile[randomTile].GetComponent<PG_TileContainer>().SetTile(tile);

            updateSurroundingTiles(lowestPossibilitiesTile[randomTile], tile);

            uncompletedTiles.Remove(lowestPossibilitiesTile[randomTile]);

            SortTiles();
        }

    }

    public void updateSurroundingTiles(GameObject tile, PG_Tile setTile)
    {
        int col = tile.GetComponent<PG_TileContainer>().col;
        int row = tile.GetComponent<PG_TileContainer>().row;

        int minCol = Mathf.Clamp(col - 1, 0, columns - 1);
        int maxCol = Mathf.Clamp(col + 1, 0, columns - 1);
        int minRow = Mathf.Clamp(row - 1, 0, rows - 1);
        int maxRow = Mathf.Clamp(row + 1, 0, rows - 1);

        Debug.Log("Out position: " + col + ", " + row);

        for(int i = minCol; i <= maxCol; i++)
        {
            for (int j = minRow; j <= maxRow; j++)
            {
                Debug.Log("Checking position: " + i + ", " + j);
                Debug.Log("Index: " + (i * columns + j));
                generatedTiles[i * columns + j].GetComponent<PG_TileContainer>().UpdatePossibilities(setTile);
            }
        }

    }

    public int lowestValue = 100;
    public void SortTiles()
    {
        lowestPossibilitiesTile.Clear();
        lowestValue = 100;
        foreach (GameObject uncompletedTile in uncompletedTiles)
        {
            if (lowestPossibilitiesTile.Count == 0)
            {
                lowestPossibilitiesTile.Add(uncompletedTile);
                lowestValue = uncompletedTile.GetComponent<PG_TileContainer>().possibilities;
            }
            if (uncompletedTile.GetComponent<PG_TileContainer>().possibilities == lowestValue)
            {
                lowestPossibilitiesTile.Add(uncompletedTile);
            }
            if (uncompletedTile.GetComponent<PG_TileContainer>().possibilities < lowestValue)
            {
                lowestPossibilitiesTile.Clear();
                lowestPossibilitiesTile.Add(uncompletedTile);
                lowestValue = uncompletedTile.GetComponent<PG_TileContainer>().possibilities;
            }
        }
    }


}
