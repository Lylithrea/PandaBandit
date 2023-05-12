using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Melee")]
public class S_Melee : ScriptableObject, I_Attack
{
    public GameObject effect;
    public float attackSpeed;
    public float size;
}
