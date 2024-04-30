using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public WaveSpawner[] spawners; // wave spawners
    public float timeBetweenWaves; // intermission time between waves

    public int waveCurrent; // current wave

    void Awake()
    {
        instance = this;
        StartCoroutine(StartWave());
    }

    public IEnumerator StartWave()
    {
        waveCurrent++;

        if (waveCurrent <= spawners.Length)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            spawners[waveCurrent - 1].StartWave();
        }
    }
}
