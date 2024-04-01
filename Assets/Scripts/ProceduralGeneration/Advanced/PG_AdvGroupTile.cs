using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, CreateAssetMenu(menuName = "ProceduralGeneration/Group Tile")]
public class PG_AdvGroupTile : PG_AdvTile
{
    public List<GameObject> tiles = new List<GameObject>();
    public int columns = 0, rows = 0;

    public Dictionary<Vector2, GameObject> tilePositions = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, PG_AdvTile> tilePositionsNormalized = new Dictionary<Vector2, PG_AdvTile>();

    [Button]
    public void GenerateTileLayout()
    {
        tiles = new List<GameObject>(columns * rows);
        for (int i = 0; i < columns * rows; i++)
        {
            tiles.Add(new GameObject());
        }
        Debug.Log("Blep");

        return;

        float smallestX = 10;
        float smallestY = 10;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].transform.position.x < smallestX)
            {
                smallestX = tiles[i].transform.position.x;
            }
            if (tiles[i].transform.position.y < smallestY)
            {
                smallestX = tiles[i].transform.position.x;
            }
        }

        for (int i = 0;i < tiles.Count; i++)
        {
            // Get position of the GameObject
            Vector2 position = new Vector2(tiles[i].transform.position.x, tiles[i].transform.position.y);

            // Calculate index based on reference position and grid size
            int xIndex = Mathf.FloorToInt((position.x - smallestX) / columns);
            int yIndex = Mathf.FloorToInt((position.y - smallestY) / rows);

            // Store the calculated index
            Vector2Int index = new Vector2Int(xIndex, yIndex);
            tilePositions.Add(index, tiles[i]);
            Debug.Log("Tile: " + tiles[i].GetComponent<PG_AdvTile>().TileName() + " placed on: " + index);
        }
    }





}
