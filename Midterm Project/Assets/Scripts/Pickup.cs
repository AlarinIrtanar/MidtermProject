using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{


    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, 45.0f * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           //only trigger if the player has guns
           if(GameManager.instance.playerScript.RefillAmmo())
            {
                Destroy(gameObject);
            }
           //add functionality to only destroy if ammo wasn't already full
        }
    }
}
