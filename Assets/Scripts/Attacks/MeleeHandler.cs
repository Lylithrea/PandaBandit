using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeHandler : MonoBehaviour
{

    EquipmentDamage[] damageTypes;

    public void Setup(EquipmentDamage[] damageTypes)
    {
        this.damageTypes = damageTypes;
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
