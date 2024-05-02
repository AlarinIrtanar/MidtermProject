using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] int currentLevel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if ((GameManager.instance.enemyCount <= 0 && !WaveManager.instance) || (GameManager.instance.enemyCount <= 0 && WaveManager.instance && WaveManager.instance.waveCurrent >= WaveManager.instance.spawners.Length))
            {
                currentLevel++;
                SceneManager.LoadSceneAsync(currentLevel);
                if (currentLevel >= 2)
                {
                    GameManager.instance.statePaused();
                    GameManager.instance.menuActive = GameManager.instance.menuVictory;
                    GameManager.instance.menuActive.SetActive(GameManager.instance.isPaused);
                }
            }
        }
    }
}
