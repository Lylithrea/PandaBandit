using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon")]
public class S_Weapon : SO_Equipment
{

    [SerializeField] private WeaponType weaponType;


    [SerializeField, ShowIf("weaponType", WeaponType.Melee), Expandable] private S_Melee meleeAttack;
    [SerializeField, ShowIf("weaponType", WeaponType.Ranged), Expandable] private S_Projectile rangedAttack;


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




    private Vector3 playerPosition;
    private GameObject player;





    /// <summary>
    /// Updates values of weapons when someone changes the values
    /// </summary>
    private void OnValidate()
    {
        UpdateInternalStats();
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

    }

    /// <summary>
    /// Updates the melee values to the adjusted values
    /// </summary>
    private void updateMeleeStats()
    {
        if (meleeAttack != null)
        {
            meleeAttack.attackSpeed = adj_attackSpeed;
            meleeAttack.size = adj_attackSize;
        }
    }

    /// <summary>
    /// Updates the projectile values to the adjusted values
    /// </summary>
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


    /// <summary>
    /// Calls the attack method of currently equipped weapon
    /// </summary>
    /// <param name="player">The player who uses the weapon</param>
    public void Attack(GameObject player)
    {
        playerPosition = player.gameObject.transform.position;
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
        GameObject newCollider = Instantiate(meleeAttack.effect, player.transform);
        newCollider.transform.position = playerPosition;
        newCollider.transform.rotation = GetMouseDirection();
        MeleeHandler handler = newCollider.GetComponent<MeleeHandler>();
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
        //hard coded, might need to change, the distance of which it gets initialized from player, so it can be changed per attack
        newProjectile.transform.position += newProjectile.transform.forward * 1;
        newProjectile.AddComponent<ProjectileHandler>();
        newProjectile.GetComponent<ProjectileHandler>().SetupProjectile(adj_equipmentDamage, rangedAttack);
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
            return Quaternion.Euler(new Vector3(0, 0, 0));
        }


        //set the rotation
        Vector3 mousePos = worldPosition;
        mousePos.y = 0;
        Vector3 playerPos = playerPosition;
        playerPos.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(mousePos - playerPos);

        return lookRot;
    }


    /// <summary>
    /// Resets the adjusted values to the original values and then changes them accordingly to the artifacts
    /// </summary>
    public override void UpdateInternalStats()
    {
        base.UpdateInternalStats();

        //reset them to normal values, then make adjustments based on artifacts
        resetAdjustedValues();

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
                    default:
                        Debug.LogWarning("The artifact modifier " + mod.modifier + " is not implemented in weapons.");
                        break;
                }
            }
        }

    }

    /// <summary>
    /// Reset adjusted variables back to the original values
    /// </summary>
    void resetAdjustedValues()
    {
        adj_attackSize = attackSize;
        adj_chargeTime = chargeTime;
        adj_attackSpeed = attackSpeed;
        adj_lifetime = lifetime;
        adj_projectileSize = projectileSize;
        adj_ShootSpeed = shootSpeed;
    }
}



public enum WeaponType
{
    None,
    Melee,
    Ranged
}