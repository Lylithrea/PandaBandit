using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    private Sprite head;
    private float shootSpeed = 5;
    private float chargeTime = 0;
    private float damage = 1;
    private float lifetime = 5;
    private float size = 1;
    private Vector3 direction = new Vector3(0, 0, 0);
    private EquipmentDamage[] damageTypes;



    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += direction.normalized * shootSpeed * Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            lifetime -= Time.deltaTime;
        }
    }

    public void SetupProject(EquipmentDamage[] damageTypes, S_Projectile projectileStats, Vector3 projectileDirection)
    {
        this.damageTypes = damageTypes;
        head = projectileStats.head;
        shootSpeed = projectileStats.shootSpeed;
        chargeTime = projectileStats.chargeTime;
        damage = projectileStats.damage;
        lifetime = projectileStats.lifetime;
        size = projectileStats.size;
        direction = projectileDirection;

        this.gameObject.AddComponent<SpriteRenderer>();
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = head;
        this.gameObject.AddComponent<CircleCollider2D>();
        this.gameObject.transform.localScale = new Vector3(size, size, size);

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning("TODO: Implement functionality for dealing damage to damageable entities, hitting: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Dummy>().DoDamage(damageTypes);
        }
        Destroy(this.gameObject);
    }

}
