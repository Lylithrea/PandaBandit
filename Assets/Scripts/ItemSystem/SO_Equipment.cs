using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SO_Equipment : SO_Item
{
    public EquipmentTypes[] EquipmentDamageList;
    public EquipmentUpgrades[] EquipmentUpgradesList;
    public bool hasUpgrades;
    public List<S_Artifact> Artifacts;

    protected EquipmentTypes[] adj_equipmentDamage;
    protected EquipmentTypes[] currentEquipmentDamage;


    private int equipmentUpgradeTier = 0;

    //fix artifacts
    //add artifact
    //remove artifact
    //validate artifact list
    //

    public virtual void AddArtifact(S_Artifact artifact)
    {
        Artifacts.Add(artifact);
        UpdateInternalStats();
    }

    public virtual void RemoveArtifact(S_Artifact artifact)
    {
        Artifacts.Remove(artifact);
        UpdateInternalStats();
    }


    public void UpgradeWeapon()
    {
        equipmentUpgradeTier++;
        if (equipmentUpgradeTier >= EquipmentUpgradesList.Length)
        {
            equipmentUpgradeTier = EquipmentUpgradesList.Length - 1;
        }
        UpdateInternalStats();
    }

    public void DowngradeWeapon()
    {
        equipmentUpgradeTier--;
        if (equipmentUpgradeTier < 0)
        {
            equipmentUpgradeTier = 0;
        }
        UpdateInternalStats();
    }

    public void ResetWeapon()
    {
        equipmentUpgradeTier = 0;
        UpdateInternalStats();
    }

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
    }

    public virtual void UpdateInternalStats()
    {
        ResetInternalStats();

        foreach (S_Artifact artifact in Artifacts)
        {
            Modifiers[] mods = artifact.GetModifiers();
            if (mods == null) break;
            foreach (Modifiers mod in mods)
            {
                switch (mod.modifier)
                {
                    case ArtifactModifiers.Damage:

                        for (int i = 0; i < adj_equipmentDamage.Length; i++)
                        {
                            if (adj_equipmentDamage[i].damageType == mod.damageModifier.damageType)
                            {
                                Debug.LogWarning("Handling artifact damage modifications right now... might need improvement.");
                                adj_equipmentDamage[i].amount += (int)(currentEquipmentDamage[i].amount * ((float)mod.damageModifier.amount / 100));
                                break;
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning("The artifact modifier " + mod.modifier + " is not implemented in weapons.");
                        break;
                }
            }
        }

    }

    private void ResetInternalStats()
    {
        if (hasUpgrades)
        {
            adj_equipmentDamage = new EquipmentTypes[EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList.Length];
            currentEquipmentDamage = new EquipmentTypes[EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList.Length];
            for (int i = 0; i < adj_equipmentDamage.Length; i++)
            {
                adj_equipmentDamage[i] = new EquipmentTypes();
                adj_equipmentDamage[i].damageType = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].damageType;
                adj_equipmentDamage[i].amount = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].amount;

                currentEquipmentDamage[i] = new EquipmentTypes();
                currentEquipmentDamage[i].damageType = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].damageType;
                currentEquipmentDamage[i].amount = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].amount;
            }

        }
        else
        {
            adj_equipmentDamage = new EquipmentTypes[EquipmentDamageList.Length];
            currentEquipmentDamage = new EquipmentTypes[EquipmentDamageList.Length];
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
    public EquipmentTypes[] EquipmentDamageList;
}

public enum DAMAGETYPES
{
        Physical,
        Fire,
        Water
}