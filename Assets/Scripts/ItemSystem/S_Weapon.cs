using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon")]
public class S_Weapon : ScriptableObject, I_Item, I_Equipment
{
    [field:SerializeField] public string ItemName { get; set; }

    [SerializeField] private WeaponType weaponType;
    [field: SerializeField, HideIf("hasUpgrades")] public Sprite itemIcon { get; set; }



    [SerializeField, ShowIf("weaponType", WeaponType.Melee), Expandable] private S_Melee meleeAttack;
    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), Expandable] private S_Projectile rangedAttack;

    [field: SerializeField] public bool hasUpgrades { get; set; }
    [field: SerializeField, HideIf("hasUpgrades")] public EquipmentDamage[] EquipmentDamageList { get; set; }
    [field: SerializeField, ShowIf("hasUpgrades")] public EquipmentUpgrades[] EquipmentUpgradesList { get; set; }


    [Space]

    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), BoxGroup("Stats")] private float shootSpeed = 5;
    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), BoxGroup("Stats")] private float chargeTime = 0;
    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), BoxGroup("Stats")] private float lifetime = 5;
    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), BoxGroup("Stats")] private float projectileSize = 1;
    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), BoxGroup("Stats")] private LayerMask groundLayer = 1;

    [SerializeField, ShowIf("weaponType", WeaponType.Melee), BoxGroup("Stats")] private float attackSpeed = 1;
    [SerializeField, ShowIf("weaponType", WeaponType.Melee), BoxGroup("Stats")] private float attackSize = 1;

    private float adj_ShootSpeed;
    private float adj_chargeTime;
    private float adj_lifetime;
    private float adj_projectileSize;
    private float adj_attackSpeed;
    private float adj_attackSize;

    private EquipmentDamage[] adj_equipmentDamage;
    private EquipmentDamage[] currentEquipmentDamage;


    private int equipmentUpgradeTier = 0;


    [OnValueChanged("updateArtifacts")] public List<S_Artifact> Artifacts;

    private Vector3 playerPosition;
    private GameObject player;



    private void updateArtifacts()
    {
        //this specifically for testing, and making sure that if the artifact scripts update, this gets updated
        foreach (S_Artifact artifact in Artifacts)
        {
            artifact.SetWeaponScript(this);
        }
    }

    private void OnValidate()
    {


        Debug.Log("On validate!");
        switch (weaponType)
        {
            case WeaponType.Melee:
                updateMeleeStats();
                break;
            case WeaponType.Ranged:
                updateProjectileStats();
                break;
            default:
                Debug.Log("No weapon stats updater implemented for this weapon type: " + weaponType);
                break;
        }

        UpdateInternalStats();
    }
    private void updateMeleeStats()
    {
        if (meleeAttack != null)
        {
            meleeAttack.attackSpeed = adj_attackSpeed;
            meleeAttack.size = adj_attackSize;
        }
    }

    private void updateProjectileStats()
    {
        if (rangedAttack != null)
        {
            rangedAttack.shootSpeed = adj_ShootSpeed;
            rangedAttack.chargeTime = adj_chargeTime;
            rangedAttack.lifetime = adj_lifetime;
            rangedAttack.size = adj_projectileSize;
            rangedAttack.groundLayer = groundLayer;
        }
    }


    public void Attack(GameObject player, Vector3 position)
    {
        playerPosition = position;
        this.player = player;
        switch (weaponType)
        {
            case WeaponType.Melee:
                handleMeleeAttack();
                break;
            case WeaponType.Ranged:
                handleRangedAttack();
                break;
            default:
                Debug.Log("No weapon attack implemented for this weapon type: " + weaponType);
                break;
        }
    }

    private void handleMeleeAttack()
    {
        Debug.Log("Handling melee attack");
        GameObject newCollider = Instantiate(meleeAttack.effect);
        newCollider.transform.position = playerPosition;
        newCollider.transform.rotation = GetMouseDirection();
        MeleeHandler handler =  newCollider.GetComponent<MeleeHandler>();
        if (handler == null)
        {
            newCollider.AddComponent<MeleeHandler>();
            handler = newCollider.GetComponent<MeleeHandler>();
        }

        handler.Setup(adj_equipmentDamage);
    }

    private void handleRangedAttack() 
    {
        Debug.Log("Handling ranged attack");

        //if we get past the check if we hit a targetable object, we spawn the object and set the settings
        GameObject newProjectile = Instantiate(rangedAttack.head);

        //to which direction to we need to shoot the projectile?
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        Vector3 worldPosition = new Vector3(0, 0, 0);
        if (Physics.Raycast(ray, out hitData))
        {
            worldPosition = hitData.point;
        }
        else
        {
            Debug.LogWarning("Mouse click was not on a valid target.");
            return;
        }

        newProjectile.transform.rotation = GetMouseDirection();
        newProjectile.transform.position = new Vector3(playerPosition.x, playerPosition.y + GameManager.projectileHeight, playerPosition.z);
        //hard coded, might need to change, the distance of which it gets initialized from player (to avoid overlap collisions)
        //might need to disable collisions of projectile for x ms, or not respond to player at all, or not respond to player for x ms
        newProjectile.transform.position += newProjectile.transform.forward * 1;
        newProjectile.AddComponent<ProjectileHandler>();

        newProjectile.GetComponent<ProjectileHandler>().SetupProjectile(adj_equipmentDamage, rangedAttack, worldPosition.normalized);
    }


    private Quaternion GetMouseDirection()
    {
        //to which direction to we need to shoot the projectile?
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        Vector3 worldPosition = new Vector3(0, 0, 0);
        if (Physics.Raycast(ray, out hitData))
        {
            worldPosition = hitData.point;
        }
        else
        {
            Debug.LogWarning("Mouse click was not on a valid target.");
            return Quaternion.Euler(new Vector3(0,0,0));
        }


        //set the rotation
        Vector3 mousePos = worldPosition;
        mousePos.y = 0;
        Vector3 playerPos = playerPosition;
        playerPos.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(mousePos - playerPos);

        return lookRot;
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

    public void AddArtifact(S_Artifact artifact)
    {
        Artifacts.Add(artifact);
        UpdateInternalStats();
    }

    public void RemoveArtifact(S_Artifact artifact)
    {
        Artifacts.Remove(artifact);
        UpdateInternalStats();
    }

    public void UpdateInternalStats()
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
                    case ArtifactModifiers.AttackSpeed:
                        adj_ShootSpeed += shootSpeed * (mod.attackSpeedModifier / 100);
                        adj_attackSpeed += attackSpeed * (mod.attackSpeedModifier / 100);
                        break;
                    case ArtifactModifiers.ChargeTime:
                        adj_chargeTime += chargeTime * (mod.chargeTimeModifier / 100);
                        break;
                    case ArtifactModifiers.Lifetime:
                        adj_lifetime += lifetime * (mod.lifetimeModifier / 100);
                        break;
                    case ArtifactModifiers.Size:
                        adj_projectileSize += projectileSize * (mod.sizeModifier / 100);
                        adj_attackSize += attackSize * (mod.sizeModifier / 100);
                        break;
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

        updateMeleeStats();
        updateProjectileStats();
    }


    private void ResetInternalStats()
    {
        if (hasUpgrades)
        {
            adj_equipmentDamage = new EquipmentDamage[EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList.Length];
            currentEquipmentDamage = new EquipmentDamage[EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList.Length];
            for (int i = 0; i < adj_equipmentDamage.Length; i++)
            {
                adj_equipmentDamage[i] = new EquipmentDamage();
                adj_equipmentDamage[i].damageType = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].damageType;
                adj_equipmentDamage[i].amount = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].amount;

                currentEquipmentDamage[i] = new EquipmentDamage();
                currentEquipmentDamage[i].damageType = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].damageType;
                currentEquipmentDamage[i].amount = EquipmentUpgradesList[equipmentUpgradeTier].EquipmentDamageList[i].amount;
            }
            
        }
        else
        {
            adj_equipmentDamage = new EquipmentDamage[EquipmentDamageList.Length];
            currentEquipmentDamage = new EquipmentDamage[EquipmentDamageList.Length];
            for (int i = 0; i < adj_equipmentDamage.Length; i++)
            {
                adj_equipmentDamage[i] = new EquipmentDamage();
                adj_equipmentDamage[i].damageType = EquipmentDamageList[i].damageType;
                adj_equipmentDamage[i].amount = EquipmentDamageList[i].amount;

                currentEquipmentDamage[i] = new EquipmentDamage();
                currentEquipmentDamage[i].damageType = EquipmentDamageList[i].damageType;
                currentEquipmentDamage[i].amount = EquipmentDamageList[i].amount;
            }
        }

        adj_ShootSpeed = shootSpeed;
        adj_chargeTime = chargeTime;
        adj_lifetime = lifetime;
        adj_projectileSize = projectileSize;
        adj_attackSpeed = attackSpeed;
        adj_attackSize = attackSize;
}


}



public enum WeaponType
{
    None,
    Melee,
    Ranged
}