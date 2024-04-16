using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuVictory;
    [SerializeField] GameObject menuGameOver;
    public GameObject playerHPBar;
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
        if(Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
    }

    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }


    public void updateWinCondition(int updateVal)
    {
        //update the win condition and check if you have won
        enemyCount -= updateVal;

        if (enemyCount <= 0)
        {
            //win
            statePaused();
            menuActive = menuVictory;
            menuActive.SetActive(isPaused);
        }

    }

    public void gameOver()
    {
        statePaused();
        menuActive = menuGameOver;
        menuActive.SetActive(isPaused);
    }

}
