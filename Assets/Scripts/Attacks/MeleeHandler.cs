using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeHandler : MonoBehaviour
{

    EquipmentTypes[] damageTypes;

    /// <summary>
    /// Sets up the melee attack with the values
    /// TODO: Scale and attack speed based on percentages or something
    /// </summary>
    /// <param name="damageTypes"> The types of damage it will do when colliding with enemies </param>
    /// <param name="playerPosition"> The player position, to according place the weapon </param>
    /// <param name="rotation"> The rotation of which direction it should play at </param>
    public void Setup(EquipmentTypes[] damageTypes, S_Melee meleeStats, Vector3 playerPosition, Quaternion rotation)
    {
        this.transform.position = playerPosition;
        this.transform.rotation = rotation;

        this.damageTypes = damageTypes;
        Debug.Log("damage type: " + damageTypes[0].damageType);
        Animator anim = this.GetComponent<Animator>();
        if (anim == null)
        {
            anim = this.GetComponentInChildren<Animator>();
        }
        AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];

        //remove previous created events
        RemoveEvents(clip);
        //add new event
        AnimationEvent evt = new AnimationEvent();
        //at the end of the clip length
        evt.time = clip.length;
        evt.functionName = "EndOfAttack";
        clip.AddEvent(evt);

        anim.speed = anim.speed * meleeStats.attackSpeed;
        this.gameObject.transform.localScale = new Vector3(meleeStats.size, meleeStats.size, meleeStats.size);
    }

    public void RemoveEvents(AnimationClip clip)
    {
        AnimationEvent[] myEvents = clip.events;
        if (myEvents.Length > 0)
        {
            var list = myEvents.ToList();
            list.RemoveAt(0);
            clip.events = list.ToArray();
        }
    }

    public void DealDamage(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().receiveDamage(damageTypes);
    }

    public void EndOfAttack()
    {
        Destroy(this.gameObject);
    }

}
