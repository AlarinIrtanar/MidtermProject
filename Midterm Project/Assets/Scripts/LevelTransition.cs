using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] int currentLevel;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && GameManager.instance.enemyCount <= 0)
        {
            currentLevel++;
            SceneManager.LoadSceneAsync(currentLevel);
        }
    }
}
