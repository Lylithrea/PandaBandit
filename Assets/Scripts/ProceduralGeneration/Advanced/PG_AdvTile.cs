using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralGeneration/Advanced Tile")]
public class PG_AdvTile : ScriptableObject
{
    public string tileName;
    public GameObject tile;
    public TileTypes sideA;
    public TileTypes sideB;
    public TileTypes sideC;
    public TileTypes sideD;
}


public enum TileTypes
{
    None,
    Forest,
    Grass,
    Water
}