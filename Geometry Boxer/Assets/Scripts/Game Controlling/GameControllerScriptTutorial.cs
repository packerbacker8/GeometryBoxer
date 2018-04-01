﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;
using PlayerUI;

public class GameControllerScriptTutorial : MonoBehaviour
{

    [Tooltip("What string value of the level to load in the build order.")]
    public string mainMenu = "MainMenu";
    public float loadLevelTimeOut = 20f;
    public GameObject[] playerOptions;
    public List<GameObject> enemiesInWorld;
    public PlayerUserInterface ui;
    
    private string currentMapName;
    private GameObject activePlayer;
    private GameObject pauseMenu;
    private GameObject deathMenuObj;
    private GameObject winMenuObj;
    private int numEnemiesAlive;
    private bool playerAlive;

    // Use this for initialization
    void Awake()
    {
        numEnemiesAlive = enemiesInWorld.Count;

        for (int i = 0; i < playerOptions.Length; i++)
        {
            if (playerOptions[i].name.Contains(SaveAndLoadGame.saver.GetCharacterType()))
            {
                playerOptions[i].SetActive(true);
                activePlayer = playerOptions[i];
            }
            else
            {
                playerOptions[i].SetActive(false);
            }
        }

        numEnemiesAlive = enemiesInWorld.Count;
        for (int i = 0; i < numEnemiesAlive; i++)
        {
            enemiesInWorld[i].GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
            if(activePlayer == null)
            {
                Debug.Log("Player is null, problem");
                return;
            }
            enemiesInWorld[i].GetComponentInChildren<UserControlAI>().SetMoveTarget(activePlayer.transform.GetChild(2).gameObject);
        }
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        deathMenuObj = pauseMenu.GetComponentInChildren<DeathMenu>().gameObject;
        deathMenuObj.SetActive(false);
        winMenuObj = pauseMenu.GetComponentInChildren<WinMenu>().gameObject;
        winMenuObj.SetActive(false);

        playerAlive = true;
        currentMapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (numEnemiesAlive <= 0)
        {
            this.SendMessage("NewSceneIsLoading", SendMessageOptions.DontRequireReceiver);
            StartCoroutine(changeLevel(mainMenu));
        }
    }

    /// <summary>
    /// Updates how many enemies are alive after one is killed.
    /// </summary>
    /// <param name="index"></param>
    public void isKilled(int index)
    {
        numEnemiesAlive--;
        ui.EnemiesLeft(numEnemiesAlive);
    }

    /// <summary>
    /// Multi-threaded method to change level currently on with a fade 
    /// to black.
    /// </summary>
    /// <returns></returns>
    IEnumerator changeLevel(string levelToLoad)
    {
        float fadeTime = GetComponent<Fade>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        LoadLevel.loader.LoadALevel(levelToLoad);
    }

    /// <summary>
    /// Tells the game controller the player died.
    /// </summary>
    public void PlayerKilled()
    {
        playerAlive = false;
        //SaveAndLoadGame.saver.SetCityStatus(currentMapName, "notconquered");
        LoadLevel.loader.LoadALevel("Tutorial"); //index of the scene the player is currently on
    }

    public GameObject GetActivePlayer()
    {
        return activePlayer;
    }

    /// <summary>
    /// Function to set new target for bots to attack.
    /// </summary>
    /// <param name="index">The bot index in their respective array.</param>
    /// <param name="tag">Tag of their root object</param>
    public void SetNewTarget(int index, string tag)
    {
        enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(activePlayer.transform.GetChild(2).gameObject);
    }

    /// <summary>
    /// Function to give enemies alive indicies.
    /// </summary>
    /// <returns>Returns enemies that are being tracked as alive in enemiesInWorld container.</returns>
    public HashSet<int> EnemyAliveIndicies()
    {
        HashSet<int> enemiesAlive = new HashSet<int>();
        for (int i = 0; i < enemiesInWorld.Count; i++)
        {
            if (enemiesInWorld[i] != null)
            {
                enemiesAlive.Add(enemiesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex());
            }
        }
        return enemiesAlive;
    }
}
