using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SO_Item : ScriptableObject
{
    public string ItemName;
    public int itemID;
    public Sprite ItemIcon;
    public int maxStackSize = 1;
    public ItemVariant variant;
    public bool hasDiscovered = false;
}

[Serializable]
public enum ItemVariant
{
    None,
    Helmet,
    Chestplate,
    Leggings,
    Weapon,
    Artifact,
    Ring
}