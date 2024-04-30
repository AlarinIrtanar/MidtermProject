using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField][Range(0, 100)] int amountToSpawn;
    [SerializeField][Range(0.0f, 20.0f)] float spawnRate;
    [SerializeField] Transform[] positions;

    int numSpawned;
    bool isSpawning;
    bool startSpawning;
    int numKilled;

    void Update()
    {
        if (startSpawning && !isSpawning && numSpawned < amountToSpawn)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;

        int objIndex = Random.Range(0, objects.Length);
        int posIndex = Random.Range(0, positions.Length);

        GameObject obj = Instantiate(objects[objIndex], positions[posIndex].position, positions[posIndex].rotation);

        EnemyAI ai = obj.GetComponent<EnemyAI>();
        
        if (ai)
        {
            ai.mySpawner = this;
        }

        numSpawned++;
        yield return new WaitForSeconds(spawnRate);
        isSpawning = false;
    }

    public void UpdateEnemyNumber()
    {
        numKilled++;

        if (numKilled >= amountToSpawn)
        {
            // This wave has been wiped!
            startSpawning = false;
            StartCoroutine(WaveManager.instance.StartWave());
        }
    }

    public void StartWave()
    {
        startSpawning = true;
        GameManager.instance.updateWinCondition(amountToSpawn);
    }
}
