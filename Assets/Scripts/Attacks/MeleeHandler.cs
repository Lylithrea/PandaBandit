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
        AnimationClip clip = this.GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning("TODO: Implement functionality for dealing damage to damageable entities, hitting: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Dummy>().DoDamage(damageTypes);
        }
    }

    public void EndOfAttack()
    {
        Destroy(this.gameObject);
    }

}
