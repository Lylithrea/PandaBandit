using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SO_Equipment : SO_Item
{
    public bool hasUpgrades;
    [SerializeField, HideIf("hasUpgrades")] private EquipmentTypes[] EquipmentDamageList;
    [SerializeField, ShowIf("hasUpgrades")] private EquipmentUpgrades[] EquipmentUpgradesList;
    [SerializeField] protected List<S_Artifact> Artifacts;

    protected EquipmentTypes[] adj_equipmentDamage;
    protected EquipmentTypes[] currentEquipmentDamage;


    private int equipmentUpgradeTier = 0;


    /// <summary>
    /// Add arifact to item and update the internal adjusted stats
    /// </summary>
    /// <param name="artifact"> The artifact to add</param>
    public virtual void AddArtifact(S_Artifact artifact)
    {
        Artifacts.Add(artifact);
        UpdateInternalStats();
    }

    /// <summary>
    /// Remove an artifact of the item and update the internal adjusted stats
    /// </summary>
    /// <param name="artifact"> The artifact to remove </param>
    public virtual void RemoveArtifact(S_Artifact artifact)
    {
        Artifacts.Remove(artifact);
        UpdateInternalStats();
    }

    /// <summary>
    /// Upgrade the weapon with 1 tier and update the stats
    /// </summary>
    public virtual void UpgradeEquipement()
    {
        equipmentUpgradeTier++;
        if (equipmentUpgradeTier >= EquipmentUpgradesList.Length)
        {
            equipmentUpgradeTier = EquipmentUpgradesList.Length - 1;
        }
        UpdateInternalStats();
    }

    /// <summary>
    /// Degrade the weapon with 1 tier and update the stats
    /// </summary>
    public virtual void DowngradeEquipment()
    {
        equipmentUpgradeTier--;
        if (equipmentUpgradeTier < 0)
        {
            equipmentUpgradeTier = 0;
        }
        UpdateInternalStats();
    }

    /// <summary>
    /// Reset the weapon to the first tier of the weapon and update the stats
    /// </summary>
    public virtual void ResetEquipment()
    {
        equipmentUpgradeTier = 0;
        UpdateInternalStats();
    }

    /// <summary>
    /// Validate all artifacts equipped to item, and delete invalid artifacts and update the internal stats
    /// </summary>
    private void validateArtifacts()
    {
        for (int i = Artifacts.Count - 1; i >= 0; i--)
        {
            if (Artifacts[i] == null)
            {
                Artifacts.RemoveAt(i);
                i++;
            }
        }
        UpdateInternalStats();
    }

    /// <summary>
    /// Update the internal damage type stats of the item
    /// </summary>
    public virtual void UpdateInternalStats()
    {
        ResetInternalStats();

        //loop through all artifacts
        foreach (S_Artifact artifact in Artifacts)
        {
            Modifiers[] mods = artifact.GetModifiers();
            //break if it cannot get any values
            if (mods == null) break;
            //now loop through all the values that can be changed due to the artifact
            foreach (Modifiers mod in mods)
            {
                //only update damage, as other stats are dependend on type of equipment
                if(mod.modifier == ArtifactModifiers.Damage)
                {
                    //go through all types of damage
                    for (int i = 0; i < adj_equipmentDamage.Length; i++)
                    {
                        if (adj_equipmentDamage[i].damageType == mod.damageModifier.damageType)
                        {
                            Debug.LogWarning("Handling artifact damage modifications right now... might need improvement.");
                            adj_equipmentDamage[i].amount += (int)(currentEquipmentDamage[i].amount * ((float)mod.damageModifier.amount / 100));
                        }
                    }
                }
            }
        }

    }


    /// <summary>
    /// Resets all internal stats to base value of the item
    /// </summary>
    private void ResetInternalStats()
    {
        if (hasUpgrades)
        {
            //if no tier are avalaible we default back to default value of physical damage of 0
            if (EquipmentUpgradesList == null || EquipmentUpgradesList.Length <= 0)
            {
                Debug.LogWarning("There are no upgrades for the equipment, will return to default value (physical: 0)");
                adj_equipmentDamage = new EquipmentTypes[1];
                currentEquipmentDamage = new EquipmentTypes[1];

                adj_equipmentDamage[0] = new EquipmentTypes();
                adj_equipmentDamage[0].damageType = DAMAGETYPES.Physical;
                adj_equipmentDamage[0].amount = 0;

                currentEquipmentDamage[0] = new EquipmentTypes();
                currentEquipmentDamage[0].damageType = DAMAGETYPES.Physical;
                currentEquipmentDamage[0].amount = 0;
                return;
            }


            //make new array based on the amount of damage types at said upgrade tier
            adj_equipmentDamage = new EquipmentTypes[EquipmentUpgradesList[equipmentUpgradeTier].EquipmentTypeList.Length];
            currentEquipmentDamage = new EquipmentTypes[EquipmentUpgradesList[equipmentUpgradeTier].EquipmentTypeList.Length];
            //loop through all the types and update the values of both adjusted and current values
            for (int i = 0; i < adj_equipmentDamage.Length; i++)
            {
                adj_equipmentDamage[i] = new EquipmentTypes();
                adj_equipmentDamage[i].damageType = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentTypeList[i].damageType;
                adj_equipmentDamage[i].amount = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentTypeList[i].amount;

                currentEquipmentDamage[i] = new EquipmentTypes();
                currentEquipmentDamage[i].damageType = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentTypeList[i].damageType;
                currentEquipmentDamage[i].amount = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentTypeList[i].amount;
            }

        }
        else
        {
            //if no tier are avalaible we default back to default value of physical damage of 0
            if (EquipmentDamageList == null || EquipmentDamageList.Length <= 0)
            {
                Debug.LogWarning("There are no damage types set for equipment, will return to default value (physical: 0)");
                adj_equipmentDamage = new EquipmentTypes[1];
                currentEquipmentDamage = new EquipmentTypes[1];

                adj_equipmentDamage[0] = new EquipmentTypes();
                adj_equipmentDamage[0].damageType = DAMAGETYPES.Physical;
                adj_equipmentDamage[0].amount = 0;

                currentEquipmentDamage[0] = new EquipmentTypes();
                currentEquipmentDamage[0].damageType = DAMAGETYPES.Physical;
                currentEquipmentDamage[0].amount = 0;
                return;
            }
            //make new array based on the amount of damage types
            adj_equipmentDamage = new EquipmentTypes[EquipmentDamageList.Length];
            currentEquipmentDamage = new EquipmentTypes[EquipmentDamageList.Length];
            //loop through all the types and update the values of both adjusted and current values
            for (int i = 0; i < adj_equipmentDamage.Length; i++)
            {
                adj_equipmentDamage[i] = new EquipmentTypes();
                adj_equipmentDamage[i].damageType = EquipmentDamageList[i].damageType;
                adj_equipmentDamage[i].amount = EquipmentDamageList[i].amount;

                currentEquipmentDamage[i] = new EquipmentTypes();
                currentEquipmentDamage[i].damageType = EquipmentDamageList[i].damageType;
                currentEquipmentDamage[i].amount = EquipmentDamageList[i].amount;
            }
        }

    }
}


    [System.Serializable]
public class EquipmentTypes
{
    public DAMAGETYPES damageType;
    public int amount;
}


[System.Serializable]
public class EquipmentUpgrades
{
    public Sprite itemIcon;
    public EquipmentTypes[] EquipmentTypeList;
}

public enum DAMAGETYPES
{
    Physical,
    Fire,
    Water
}