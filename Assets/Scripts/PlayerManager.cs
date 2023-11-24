using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public int maxHealth { set { /*Update UI*/ maxHealth = value; } get { return maxHealth; } }
    public int currentHealth = 100;
    public int healthRegen = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
