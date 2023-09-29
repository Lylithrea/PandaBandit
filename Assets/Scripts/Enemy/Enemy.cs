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

    protected virtual void move()
    {
        switch (movingState)
        {
            case MOVEMENTSTATE.idle:
                break;
            case MOVEMENTSTATE.patrol:
                break;
            case MOVEMENTSTATE.chasing:
                break;
            case MOVEMENTSTATE.running:
                break;
            default:
                Debug.LogWarning("Returned to default in switch statement of moving of enemy.");
                break;
        }
    }

    public virtual void receiveDamage(EquipmentTypes[] damageTypes)
    {
        foreach(EquipmentTypes damage in damageTypes)
        {
            Debug.LogWarning("TODO: Implement damage based on equipment");
            Debug.Log(_Name + " took " + damage.amount + " " + damage.damageType + " damage");
            currentHealth -= damage.amount;
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
