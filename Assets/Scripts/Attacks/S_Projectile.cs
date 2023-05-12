using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Attacks/Projectile")]
public class S_Projectile : ScriptableObject, I_Attack
{
    public Sprite head;
    [HideInInspector] public float shootSpeed = 5;
    [HideInInspector] public float chargeTime = 0;
    [HideInInspector] public float damage = 1;
    [HideInInspector] public float lifetime = 5;
    [HideInInspector] public float size = 1;
    [HideInInspector] public Vector3 direction = new Vector3(0, 0, 0);


}
