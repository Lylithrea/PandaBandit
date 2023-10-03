using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    private GameObject head;
    private float shootSpeed = 5;
    private float chargeTime = 0;
    private float lifetime = 5;
    private float size = 1;

    [SerializeField] private EquipmentTypes[] damageTypes;
    private LayerMask layer;

    private float slopeTreshhold = 1f;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = gameObject.transform.position + gameObject.transform.forward * shootSpeed;
        Vector3 raycastPos = gameObject.transform.position + gameObject.transform.forward;
        raycastPos.y += slopeTreshhold;

        RaycastHit hit;
        //checks from a bit higher distance if there is ground nearby
        if (Physics.Raycast(raycastPos, -Vector3.up, out hit, 4, layer))
        {
            //we now have the hit point, now we need to add the distance from ground to projectile.
            float difference = targetPos.y - (hit.point.y + GameManager.projectileHeight);
            if (Mathf.Abs(difference) < 0.05f)
            {
                targetPos.y -= Mathf.Clamp(difference + 12 / difference, -1.5f, 1.5f);
            }

        }

        //this.gameObject.transform.position += gameObject.transform.forward * shootSpeed * Time.deltaTime;

        this.gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, Time.deltaTime);

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            lifetime -= Time.deltaTime;
        }
    }


    /// <summary>
    /// Sets up the projectile with the correct data and direction
    /// </summary>
    /// <param name="damageTypes"> What types of damage it will do when collding with enemies </param>
    /// <param name="projectileStats"> The stats of the projectile </param>
    /// <param name="playerPosition"> The position of the player </param>
    /// <param name="rotation"> The rotation of which the projectile should be in </param>
    public void SetupProjectile(EquipmentTypes[] damageTypes, S_Projectile projectileStats, Vector3 playerPosition, Quaternion rotation)
    {
        this.transform.rotation = rotation;
        //hardcoded the distance from player right now, could be nice to put it open for each projectile
        this.transform.position = playerPosition + this.transform.forward * 1;

        this.damageTypes = damageTypes;
        head = projectileStats.head;
        shootSpeed = projectileStats.shootSpeed;
        chargeTime = projectileStats.chargeTime;
        lifetime = projectileStats.lifetime;
        size = projectileStats.size;
        layer = projectileStats.groundLayer;

        this.gameObject.transform.localScale = new Vector3(size, size, size);

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().receiveDamage(damageTypes);
            Destroy(this.gameObject);
            //collision.gameObject.GetComponent<Dummy>().DoDamage(damageTypes);
            return;
        }
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Projectile")
        {
            Destroy(this.gameObject);
            return;
        }
        Debug.LogWarning("TODO: Implement functionality for dealing damage to damageable entities, hitting: " + other.gameObject.name);
    }


}
