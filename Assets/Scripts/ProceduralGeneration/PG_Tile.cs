using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ProceduralGeneration/Tile")]
public class PG_Tile : ScriptableObject
{
    public string tileName;
    public PG_Tile[] illegalTiles;
    public Material tile;

}
