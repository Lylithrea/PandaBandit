using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public int maxStackSize = 1;
    public ItemVariant variant;
}
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