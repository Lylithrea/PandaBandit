using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeColliderHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit an enemy");
            this.gameObject.transform.parent.GetComponent<MeleeHandler>().DealDamage(other.gameObject);
        }
    }

}
