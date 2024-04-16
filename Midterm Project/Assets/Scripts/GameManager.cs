using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuVictory;
    [SerializeField] GameObject menuGameOver;
    public GameObject playerHPBer;

    public GameObject player;

    public bool isPaused;
    int enemyCount;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        
    }
}
