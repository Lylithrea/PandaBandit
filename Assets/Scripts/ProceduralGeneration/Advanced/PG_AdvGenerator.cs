using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows.Speech;

[ExecuteInEditMode]
public class PG_AdvGenerator : MonoBehaviour
{

    public List<PG_AdvTile> allTiles = new List<PG_AdvTile>();
    [ReadOnly] public int totalRarity = 0;
    public List<PG_AdvTile> startTiles = new List<PG_AdvTile>();
    [ReadOnly] public int totalStartRarity = 0;
    public List<PG_AdvTile> endTiles = new List<PG_AdvTile>();
    [ReadOnly] public int totalEndRarity = 0;
    public List<PG_AdvTile> backupTiles = new List<PG_AdvTile>();
    [Space]
    public GameObject baseTile;
    public bool newGeneration = false;
    public int columns = 0, rows = 0;



    public List<GameObject> generatedTiles = new List<GameObject>();

    public List<GameObject> lowestPossibilitiesTile = new List<GameObject>();
    public List<GameObject> connectedToStartTile = new List<GameObject>();
    public List<GameObject> uncompletedTiles = new List<GameObject>();
    public List<GameObject> neighbouringTiles = new List<GameObject>();
    [Space]
    public Dictionary<Vector3, GameObject> newGeneratedTiles = new Dictionary<Vector3, GameObject>();
    public List<GameObject> uncompletedNewTiles = new List<GameObject> ();
    public List<GameObject> lowestEntropyTiles = new List<GameObject> ();

    public static PG_AdvGenerator instance;

    public int minLength = 4;
    public int maxLength = 6;
    public AnimationCurve maxmaxConnectionCurve = new AnimationCurve();
    public AnimationCurve maxConnectionCurve = new AnimationCurve();
    public AnimationCurve medConnectionCurve = new AnimationCurve();
    public AnimationCurve minConnectionCurve = new AnimationCurve();
    [Foldout("Start Tile Settings")] public int minXPosition = 0;
    [Foldout("Start Tile Settings")] public int maxXPosition = 1;
    [Foldout("Start Tile Settings")] public int minYPosition = 0;
    [Foldout("Start Tile Settings")] public int maxYPosition = 1;

    [Foldout("End Tile Settings")] public int minAmount = 1;
    [Foldout("End Tile Settings")] public int maxAmount = 10;
    [Foldout("End Tile Settings")] public int minDistance = 5;

    public int currentEndTiles = 0;

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
        CalculateTotalRarity();
    }

    public void CalculateTotalRarity()
    {
        int startRarity = 0;
        foreach (PG_AdvTile tile in startTiles)
        {
            startRarity += tile.rarity;
        }
        totalStartRarity = startRarity;
        foreach (PG_AdvTile tile in startTiles)
        {
            tile.SetReadOnlys(totalStartRarity);
        }

        int endRarity = 0;
        foreach (PG_AdvTile tile in endTiles)
        {
            endRarity += tile.rarity;
        }
        totalEndRarity = endRarity;
        foreach (PG_AdvTile tile in endTiles)
        {
            tile.SetReadOnlys(totalEndRarity);
        }

        int totalAllRarity = 0;
        foreach (PG_AdvTile tile in allTiles)
        {
            totalAllRarity += tile.rarity;
        }
        totalRarity = totalAllRarity;
        foreach (PG_AdvTile tile in allTiles)
        {
            tile.SetReadOnlys(totalAllRarity);
        }
    }


    [Button]
    public void RemoveTiles()
    {
        foreach (GameObject tile in generatedTiles)
        {
            DestroyImmediate(tile);
        }
        foreach (KeyValuePair<Vector3, GameObject> tile in newGeneratedTiles)
        {
            DestroyImmediate(tile.Value);
        }
        foreach (Transform child in this.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        generatedTiles.Clear();
        uncompletedTiles.Clear();
        lowestPossibilitiesTile.Clear();
        startTilesManager = null;
        endTilesManagers.Clear();
        neighbouringTiles.Clear();

        newGeneratedTiles.Clear();
        uncompletedNewTiles.Clear();
        lowestEntropyTiles.Clear();
    }

    [Button]
    public void Generate()
    {
        RemoveTiles();

        if (newGeneration)
        {
            return;
        }

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


    private void SetupNextGenTile(GameObject tile, int col, int row)
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
        tile.GetComponent<PG_TileManager>().UpdateTile();
        newGeneratedTiles.Add(new Vector3(col, 0, row), tile);
    }

    private void GenerateNeighbouringTiles(GameObject tile)
    {
        PG_TileManager baseManager = tile.GetComponent<PG_TileManager> ();

        updateOrCreateTile(baseManager.col, baseManager.row + 1);
        updateOrCreateTile(baseManager.col + 1, baseManager.row);
        updateOrCreateTile(baseManager.col, baseManager.row - 1);
        updateOrCreateTile(baseManager.col - 1, baseManager.row );
    }

    private void updateOrCreateTile(int col, int row)
    {
        if (newGeneratedTiles.ContainsKey(new Vector3(col, 0, row)))
        {
            newGeneratedTiles[new Vector3(col, 0, row)].GetComponent<PG_TileManager>().UpdateTile();
        }
        else
        {
            GameObject tile = Instantiate(baseTile, this.transform);
            SetupNextGenTile(tile, col, row);
            uncompletedNewTiles.Add(tile);
        }
    }

    private void NewGenNextStep()
    {
        //newGeneratedTiles contains all generated tiles, so if its 0, none has spawned yet.
        if (newGeneratedTiles.Count <= 0)
        {
            //generate start tile; 
            //also spawn 4 surrounding tiles, so the entropy can be generated for those.
            Debug.Log("Creating start tile!");
            GameObject newTile = Instantiate(baseTile, this.transform);
            SetupNextGenTile(newTile, 0, 0);

            newTile.GetComponent<PG_TileManager>().SetStartTile(startTiles);
            newTile.GetComponent<PG_TileManager>().isConnectedToStart = true;
            newTile.GetComponent<PG_TileManager>().isStartTile = true;
            newTile.GetComponent<PG_TileManager>().distanceFromStart = 0;


            //spawn the 4 neighbouring tiles
            GenerateNeighbouringTiles(newTile);

            List<GameObject> result = SetConnectedToStart(newTile);
            newTile.GetComponent<PG_TileManager>().neighbours = result;
        }
        else
        {
            Debug.Log("Creating new tile!");

            int randomTile = Random.Range(0, lowestEntropyTiles.Count);

            lowestEntropyTiles[randomTile].GetComponent<PG_TileManager>().SetTile();

            GenerateNeighbouringTiles(lowestEntropyTiles[randomTile]);
            List<GameObject> result = SetConnectedToStart(lowestEntropyTiles[randomTile]);
            lowestEntropyTiles[randomTile].GetComponent<PG_TileManager>().neighbours = result;
            uncompletedNewTiles.Remove(lowestEntropyTiles[randomTile]);


        }
        //if we do already have generated tiles, we look at the lowest entropy.
        //Generate the lowest entropy tile, and then generate the 4 around it if they havent been generated yet, if they have, update those tiles instead.

        SortNewTiles();
    }


    public int steps = 1;

    [Button]
    public void NextStep()
    {

        for (int i = 0; i < steps; i++)
        {
            if (newGeneration)
            {
                NewGenNextStep();
                continue;
            }

            try
            {
                if (generatedTiles.Count == 0) return;

                //generate starttile
                if (startTilesManager == null)
                {
                    Debug.Log("Creating start tile!");
                    int randomStartTile = Random.Range(0, startTiles.Count);

                    lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().SetStartTile(startTiles);
                    lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().SetTile();
                    lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().isConnectedToStart = true;
                    SetConnectedToStart(lowestPossibilitiesTile[randomStartTile]);

                    //neighbouringTiles.AddRange(GetNeighbours(lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().row));
                    AddRange(GetNeighbours(lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>().row));
                    neighbouringTiles.Remove(lowestPossibilitiesTile[randomStartTile]);

                    updateSurroundingTiles(lowestPossibilitiesTile[randomStartTile]);

                    uncompletedTiles.Remove(lowestPossibilitiesTile[randomStartTile]);
                    startTilesManager = lowestPossibilitiesTile[randomStartTile].GetComponent<PG_TileManager>();
                    SortTiles();
                    return;
                }

                if (connectedToStartTile.Count > 0)
                {
                    Debug.Log("Creating connected tile!");
                    int randomTile = Random.Range(0, connectedToStartTile.Count);

                    connectedToStartTile[randomTile].GetComponent<PG_TileManager>().SetTile();

                    //neighbouringTiles.AddRange(GetNeighbours(lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().row));

                    AddRange(GetNeighbours(connectedToStartTile[randomTile].GetComponent<PG_TileManager>().col, connectedToStartTile[randomTile].GetComponent<PG_TileManager>().row));

                    neighbouringTiles.Remove(connectedToStartTile[randomTile]);

                    updateSurroundingTiles(connectedToStartTile[randomTile]);

                    uncompletedTiles.Remove(connectedToStartTile[randomTile]);
                    SetConnectedToStart(connectedToStartTile[randomTile]);
                    SortTiles();
                }
                else
                {
                    Debug.Log("Creating normal tile!");
                    int randomTile = Random.Range(0, lowestPossibilitiesTile.Count);

                    lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().SetTile();

                    //neighbouringTiles.AddRange(GetNeighbours(lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().row));

                    AddRange(GetNeighbours(lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().col, lowestPossibilitiesTile[randomTile].GetComponent<PG_TileManager>().row));

                    neighbouringTiles.Remove(lowestPossibilitiesTile[randomTile]);

                    updateSurroundingTiles(lowestPossibilitiesTile[randomTile]);

                    uncompletedTiles.Remove(lowestPossibilitiesTile[randomTile]);
                    SetConnectedToStart(lowestPossibilitiesTile[randomTile]);
                    SortTiles();
                }
            }
            catch
            {

            }


        }

    }

    public int maxDistance = 0;

    public List<GameObject> SetConnectedToStart(GameObject tile)
    {
        List<GameObject> result = new List<GameObject>();
        Debug.Log("Setting connected! Neighbouring tiles of: " + tile + " with distance: " + tile.GetComponent<PG_TileManager>().distanceFromStart);
        for (int i = 0; i < 4; i++)
        {
            TileTypes type = tile.GetComponent<PG_TileManager>().GetSideBasedOnRotation(i);
            if (type == TileTypes.Solid)
            {
                GameObject side = getNeighbourBasedOnSide(tile.GetComponent<PG_TileManager>().col, tile.GetComponent<PG_TileManager>().row, i);
                result.Add(side);
                if (side.GetComponent<PG_TileManager>().isStartTile) continue;
                side.GetComponent<PG_TileManager>().isConnectedToStart = true;
                side.GetComponent<PG_TileManager>().updateDistanceFromStart(tile.GetComponent<PG_TileManager>().distanceFromStart + 1);
            }
        }
        return result;

    }


    public void AddRange(List<GameObject> neighbours)
    {
        foreach (GameObject neighbour in neighbours)
        {
            if (neighbouringTiles.Contains(neighbour)) continue;
            if (uncompletedTiles.Contains(neighbour))
            {
                neighbouringTiles.Add(neighbour);
            }

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



    public void SortNewTiles()
    {
        lowestEntropyTiles.Clear();
        lowestValue = 100;

        foreach (GameObject neighbour in uncompletedNewTiles)
        {
            if (neighbour.GetComponent<PG_TileManager>().isConnectedToStart)
            {
                if (lowestEntropyTiles.Count == 0)
                {
                    lowestValue = neighbour.GetComponent<PG_TileManager>().entropy;
                    lowestEntropyTiles.Clear();
                    lowestEntropyTiles.Add(neighbour);
                    continue;
                }

                if (neighbour.GetComponent<PG_TileManager>().entropy == lowestValue)
                {
                    lowestEntropyTiles.Add(neighbour);
                }
                if (neighbour.GetComponent<PG_TileManager>().entropy < lowestValue)
                {
                    lowestEntropyTiles.Clear();
                    lowestEntropyTiles.Add(neighbour);
                    lowestValue = neighbour.GetComponent<PG_TileManager>().entropy;
                }


                continue;
            }
/*            if (neighbour.GetComponent<PG_TileManager>().entropy != 0)
            {
                if (lowestEntropyTiles.Count == 0)
                {
                    lowestEntropyTiles.Add(neighbour);
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
            }*/

        }
    }



    public float lowestValue = 100;
    public void SortTiles()
    {
        connectedToStartTile.Clear();
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
            if (neighbour.GetComponent<PG_TileManager>().isConnectedToStart)
            {
                if (connectedToStartTile.Count == 0)
                {
                    lowestValue = neighbour.GetComponent<PG_TileManager>().entropy;
                    connectedToStartTile.Clear();
                    connectedToStartTile.Add(neighbour);
                    continue;
                }

                if (neighbour.GetComponent<PG_TileManager>().entropy == lowestValue)
                {
                    connectedToStartTile.Add(neighbour);
                }
                if (neighbour.GetComponent<PG_TileManager>().entropy < lowestValue)
                {
                    connectedToStartTile.Clear();
                    connectedToStartTile.Add(neighbour);
                    lowestValue = neighbour.GetComponent<PG_TileManager>().entropy;
                }


                continue;
            }
            if (neighbour.GetComponent<PG_TileManager>().entropy != 0)
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

    public GameObject getNeighbourBasedOnSide(int col, int row, int side)
    {
        if (newGeneration)
        {
            return getNewNeighbourBasedOnSide(col, row, side);
        }
        switch (side)
        {
            case 0:
                if (row + 1 < rows)
                {
                    return generatedTiles[col * columns + (row + 1)];
                }
                return null;
            case 1:
                if (col + 1 < columns)
                {
                    return generatedTiles[(col + 1) * columns + row];
                }
                return null;
            case 2:
                if (row - 1 >= 0)
                {
                    return generatedTiles[col * columns + (row - 1)];
                }
                return null;
            case 3:
                if (col - 1 >= 0)
                {
                    return generatedTiles[(col - 1) * columns + row];
                }
                return null;
            default:
                Debug.LogWarning("Tiles do not have more than 4 sides, invalid side");
                return null;
        }
    }

    public GameObject getNewNeighbourBasedOnSide(int col, int row, int side)
    {
        switch (side)
        {
            case 0:
                if (newGeneratedTiles.ContainsKey(new Vector3(col, 0, row + 1)))
                {
                    return newGeneratedTiles[new Vector3(col, 0, row + 1)];
                }
                return null;
            case 1:
                if (newGeneratedTiles.ContainsKey(new Vector3(col + 1, 0, row)))
                {
                    return newGeneratedTiles[new Vector3(col + 1, 0, row)];
                }
                return null;
            case 2:
                if (newGeneratedTiles.ContainsKey(new Vector3(col, 0, row - 1)))
                {
                    return newGeneratedTiles[new Vector3(col, 0, row - 1)];
                }
                return null;
            case 3:
                if (newGeneratedTiles.ContainsKey(new Vector3(col - 1, 0, row)))
                {
                    return newGeneratedTiles[new Vector3(col - 1, 0, row)];
                }
                return null;
            default:
                Debug.LogWarning("Tiles do not have more than 4 sides, invalid side");
                return null;
        }
    }


    public List<TileTypes> getNewTypes(int col, int row)
    {
        List<TileTypes> neighbours = new List<TileTypes>();

        if (newGeneratedTiles.ContainsKey(new Vector3(col, 0, row + 1)))
        {
            neighbours.Add(newGeneratedTiles[new Vector3(col, 0, row + 1)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.None);
        }
        if (newGeneratedTiles.ContainsKey(new Vector3(col + 1, 0, row)))
        {
            neighbours.Add(newGeneratedTiles[new Vector3(col + 1, 0, row)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.None);
        }
        if (newGeneratedTiles.ContainsKey(new Vector3(col, 0, row - 1)))
        {
            neighbours.Add(newGeneratedTiles[new Vector3(col, 0, row - 1)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.None);
        }
        if (newGeneratedTiles.ContainsKey(new Vector3(col - 1, 0, row)))
        {
            neighbours.Add(newGeneratedTiles[new Vector3(col -1 , 0, row)].GetComponent<PG_TileManager>().GetSide(col, row));
        }
        else
        {
            neighbours.Add(TileTypes.None);
        }

        return neighbours;
    }

    public List<TileTypes> GetTypesOfNeighbours(int col, int row)
    {
        List<TileTypes> neighbours = new List<TileTypes>();

        if (newGeneration)
        {
            return getNewTypes(col, row);
        }

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
