using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armour")]
public class S_Armour : ScriptableObject, I_Item, I_Equipment
{
    [field: SerializeField] public string ItemName { get; set; }

    [field:SerializeField] public bool hasUpgrades { get; set; }

    [field: SerializeField, HideIf("hasUpgrades")] public EquipmentDamage[] EquipmentDamageList { get; set; }

    [field: SerializeField, ShowIf("hasUpgrades")] public EquipmentUpgrades[] EquipmentUpgradesList { get; set; }

    [field: SerializeField, HideIf("hasUpgrades")] public Sprite itemIcon { get; set; }

}
