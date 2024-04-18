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
        
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
            //pickup item

            Destroy(gameObject);
        }
    }
}
