using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IDamage
{
    [SerializeField] int hp;
    [SerializeField] AudioSource hitSound;
    [SerializeField] float volumePerDamage;

    public void TakeDamage(int amount)
    {
        hitSound.volume = volumePerDamage * amount;
        hitSound.pitch = (Random.value - 0.5f) * 0.5f + 1.0f;
        hitSound.Play();
        hp -= amount;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
