using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string _Name;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected MOVEMENTSTATE movingState;
    [SerializeField] protected EquipmentTypes[] resitances;
    [SerializeField] protected float idleSpeed = 1;
    [SerializeField] protected float idleCooldownMin = 5;
    [SerializeField] protected float idleCooldownMax = 5;
    [SerializeField] protected float idleRange = 1;
    [SerializeField] protected float movementSpeed = 3;
    [SerializeField] protected float aggressionRange = 3;
    //Gotta change this to knock back resistence later instead, and knock back strength on the weapon
    [SerializeField] protected float knockBackStrength = 0.25f;

    // Eventually get the player from gamemanager
    public GameObject player;

    private float currentTime = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        movingState = MOVEMENTSTATE.idle;
        player = GameObject.FindGameObjectWithTag("Player");
        Vector2 randomPos = Random.insideUnitCircle;
        randomPos *= idleRange;
        newPos = this.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
    }

    protected virtual void move()
    {
        switch (movingState)
        {
            case MOVEMENTSTATE.idle:
                idleMovement();
                break;
            case MOVEMENTSTATE.patrol:
                break;
            case MOVEMENTSTATE.chasing:
                chasePlayer();
                break;
            case MOVEMENTSTATE.running:
                break;
            default:
                Debug.LogWarning("Returned to default in switch statement of moving of enemy.");
                break;
        }
    }

    private float movementTime = 0;
    Vector3 newPos = new Vector3(0, 0, 0);

    private void idleMovement()
    {
        if (currentTime <= 0)
        {
            
            
            movementTime += Time.deltaTime * idleSpeed;
            this.transform.position = Vector3.Slerp(this.transform.position, newPos, movementTime);

            if(Vector3.Distance(this.transform.position, newPos) < 0.25f)
            {
                currentTime = Random.Range(idleCooldownMin, idleCooldownMax);
                movementTime = 0;
                Vector2 randomPos = Random.insideUnitCircle;
                randomPos *= idleRange;
                newPos = this.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
            }
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }

    private void chasePlayer()
    {
        Vector3 direction = player.transform.position - this.transform.position;
        direction.Normalize();
        this.transform.position += direction * movementSpeed * Time.deltaTime;
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, this.gameObject.transform.position) < aggressionRange)
        {
            //Debug.Log("Enemy spotted you!");
            movingState = MOVEMENTSTATE.chasing;
        }
        move();
    }

    public virtual void receiveDamage(EquipmentTypes[] damageTypes)
    {
        //check health every damage instance or afterwards?
        foreach(EquipmentTypes damage in damageTypes)
        {
            Debug.LogWarning("TODO: Implement damage based on equipment");
            Debug.Log(_Name + " took " + damage.amount + " " + damage.damageType + " damage");
            currentHealth -= damage.amount;
        }
        knockBack(knockBackStrength);

        checkHealth();
    }

    void knockBack(float strength)
    {
        //gotta change this to attack direction and not player :3
        Vector3 knockbackDirection = player.transform.position - this.transform.position;
        knockbackDirection.Normalize();
        //gotta do this over time later instead
        Vector3 knockPos = this.transform.position;
        knockPos.x -= knockbackDirection.x * strength * Time.deltaTime;
        knockPos.z -= knockbackDirection.z * strength * Time.deltaTime;
        this.transform.position = knockPos;
    }

    void checkHealth()
    {
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject); 
            return;
        }
    }

    protected virtual void attack()
    {

    }
    
    

}

public enum MOVEMENTSTATE{
    idle,
    patrol,
    chasing,
    running
}
