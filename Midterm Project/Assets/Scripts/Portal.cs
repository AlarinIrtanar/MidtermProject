using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Transform teleportTarget;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.controller.enabled = false;
            GameManager.instance.player.transform.position = teleportTarget.transform.position;
            GameManager.instance.playerScript.controller.enabled = true;
        }
    }

}
