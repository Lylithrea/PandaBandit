using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PG_AdvTile : ScriptableObject
{
    protected string tileName;
    public GameObject visualObject;

    public virtual TileTypes GetSide(int side)
    {
        return TileTypes.None;
    }

    public virtual GameObject GetTile()
    {
        return null;
    }

    public virtual string TileName()
    {
        return tileName;
    }
}


public enum TileTypes
{
    None,
    Forest,
    Grass,
    Water,
    Solid,
    Empty
}