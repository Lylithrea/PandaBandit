using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    private GameObject head;
    private float shootSpeed = 5;
    private float chargeTime = 0;
    private float damage = 1;
    private float lifetime = 5;
    private float size = 1;
    private Vector3 direction = new Vector3(0, 0, 0);
    private EquipmentDamage[] damageTypes;
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

    public void SetupProjectile(EquipmentDamage[] damageTypes, S_Projectile projectileStats, Vector3 projectileDirection)
    {
        this.damageTypes = damageTypes;
        head = projectileStats.head;
        shootSpeed = projectileStats.shootSpeed;
        chargeTime = projectileStats.chargeTime;
        damage = projectileStats.damage;
        lifetime = projectileStats.lifetime;
        size = projectileStats.size;
        direction = projectileDirection;
        layer = projectileStats.groundLayer;

        this.gameObject.transform.localScale = new Vector3(size, size, size);

    }

    public void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Dummy>().DoDamage(damageTypes);
        }
        if (collision.gameObject.tag != "Player")
        {
            Destroy(this.gameObject);
            return;
        }
        Debug.LogWarning("TODO: Implement functionality for dealing damage to damageable entities, hitting: " + collision.gameObject.name);

    }

}
