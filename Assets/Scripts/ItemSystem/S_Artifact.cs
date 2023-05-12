using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Item/Artifact")]
public class S_Artifact : ScriptableObject, I_Item
{
    public string ItemName { get; set; }
    [HideIf("hasUpgrades")] public Sprite itemIcon;
    public bool hasUpgrades = false;
    
    [HideIf("hasUpgrades")] public Modifiers[] artifactModifiersList;
    [ShowIf("hasUpgrades")] public ArtifactUpgrades[] artifactUpgradesList;

    private int artifactUpgradeTier = 0;
    private S_Weapon weaponScript;

    public Modifiers[] GetModifiers()
    {
        if (hasUpgrades)
        {
            return artifactUpgradesList[artifactUpgradeTier].modifierList;
        }
        else
        {
            return artifactModifiersList;
        }
    }

    private void OnValidate()
    {
        Debug.Log("Validating artifact...");
        weaponScript.UpdateInternalStats();
    }

    public void SetWeaponScript(S_Weapon weapon)
    {
        weaponScript = weapon;
    }

    public void UpgradeArtifact()
    {
        artifactUpgradeTier++;
        if (artifactUpgradeTier >= artifactUpgradesList.Length)
        {
            artifactUpgradeTier = artifactUpgradesList.Length - 1;
        }
    }
    public void DowngradeArtifact()
    {
        artifactUpgradeTier--;
        if (artifactUpgradeTier < 0)
        {
            artifactUpgradeTier = 0;
        }
    }

    public void ResetArtifact()
    {
        artifactUpgradeTier = 0;
        Debug.LogWarning("Artifact reset is not fully implemented, and will only reset the upgrade tier.");
    }


}



[System.Serializable]
public class ArtifactUpgrades
{
    public Sprite itemIcon;
    public Modifiers[] modifierList;
}

[System.Serializable]
public class Modifiers
{
    public ArtifactModifiers modifier;
    [AllowNesting, ShowIf("modifier", ArtifactModifiers.AttackSpeed)] public float attackSpeedModifier;
    [AllowNesting, ShowIf("modifier", ArtifactModifiers.Damage)] public EquipmentDamage damageModifier;
    [AllowNesting, ShowIf("modifier", ArtifactModifiers.Defense)] public float defenseModifier;
    [AllowNesting, ShowIf("modifier", ArtifactModifiers.Size)] public float sizeModifier;
    [AllowNesting, ShowIf("modifier", ArtifactModifiers.ChargeTime)] public float chargeTimeModifier;
    [AllowNesting, ShowIf("modifier", ArtifactModifiers.Lifetime)] public float lifetimeModifier;
}

public enum ArtifactModifiers
{
    AttackSpeed,
    Damage,
    Defense,
    Size,
    ChargeTime,
    Lifetime
}