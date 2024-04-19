using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PG_AdvTile : ScriptableObject
{
    protected string tileName;


    [InfoBox("The total of all rarities of this type will be shown here. The spawn percentage of this tile will be : rarity / totalRarity * 100.\nThe percentage is automatically calculated.\n\nExample: Rarity = 5, totalRarity = 25. The spawn perctage will be: 5/25*100, thus 20%", EInfoBoxType.Normal)]

    [ReadOnly] public int totalRarity = 0;
    [ReadOnly] public double percentage = 0;

    public int rarity = 1;


    public void OnValidate()
    {
        PG_AdvGenerator.instance.CalculateTotalRarity();
    }

    public void SetReadOnlys(int total)
    {
        totalRarity = total;
        percentage = System.Math.Round((double)((float)rarity / (float)totalRarity) * 100, 2);
    }

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