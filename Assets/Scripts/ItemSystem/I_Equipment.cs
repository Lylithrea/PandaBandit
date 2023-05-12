using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public interface I_Equipment
{
    public EquipmentDamage[] EquipmentDamageList { get; set; }
    public EquipmentUpgrades[] EquipmentUpgradesList { get; set; }
    public bool hasUpgrades { get; set; }

    public Sprite itemIcon { get; set; }
}


[System.Serializable]
public class EquipmentDamage
{
    public DAMAGETYPES damageType;
    public int amount;
}


[System.Serializable]
public class EquipmentUpgrades
{
    public Sprite itemIcon;
    public EquipmentDamage[] EquipmentDamageList;
}








public enum DAMAGETYPES
{
        Physical,
        Fire,
        Water
}