using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{

    public S_Weapon[] weapon;
    public int weaponCount;
    public float switchTimer = 0.25f;
    private float currentSwitchTimer = 0;


    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            if (currentSwitchTimer <= 0)
            {
                weaponCount++;
                currentSwitchTimer = switchTimer;
                if (weaponCount >= weapon.Length)
                {
                    weaponCount = 0;
                }
            }
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            if (currentSwitchTimer <= 0)
            {
                weaponCount--;
                currentSwitchTimer = switchTimer;
                if (weaponCount < 0)
                {
                    weaponCount = weapon.Length -1;
                }
            }
        }
        currentSwitchTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            weapon[weaponCount].Attack(this.gameObject);

        }
    }

}
