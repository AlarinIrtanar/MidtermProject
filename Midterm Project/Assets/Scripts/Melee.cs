using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] int damage;

    bool hitHappened;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && !hitHappened)
        {
            dmg.TakeDamage(damage);
            hitHappened = true;
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        hitHappened = false;
    }
}


