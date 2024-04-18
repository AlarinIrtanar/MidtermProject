using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Transform teleportTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /*GameManager.instance.player.transform.position = teleportTarget;*/
            GameManager.instance.playerScript.controller.enabled = false;
            GameManager.instance.player.transform.position = teleportTarget.transform.position;
            GameManager.instance.playerScript.controller.enabled = true;

        }
    }

}
