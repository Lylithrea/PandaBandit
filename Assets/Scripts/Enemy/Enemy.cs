using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected string _Name;
    protected int maxHealth;
    protected int currentHealth;
    protected MOVEMENTSTATE movingState;

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

    protected virtual void receiveDamage(int damage)
    {
        Debug.Log(_Name + " took " + damage + " damage");
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            Debug.Log("Enemy died!");
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
