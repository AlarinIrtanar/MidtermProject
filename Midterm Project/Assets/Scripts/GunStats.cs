using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject gunModel;

    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public int ammoCurrent;
    public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0,1)] public float shootSoundVolume;
}