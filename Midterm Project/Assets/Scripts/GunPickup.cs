using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunStats gun;

    private void Start()
    {
        // Switch to corresponding gun model
        GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        // Make current ammo = max ammo
        gun.ammoCurrent = gun.ammoMax;
    }

    private void Update()
    {
        // Do rotation animation
        transform.Rotate(0, 45.0f * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Give the player the gun
            GameManager.instance.playerScript.GiveGun(gun);
            Destroy(gameObject);
        }
    }
}
