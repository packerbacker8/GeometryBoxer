﻿using UnityEngine;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SafetyNet : MonoBehaviour
{
    [Header("Player Character Options")]
    public GameObject[] playerOptions;

    private GameObject resetLocation;
    private GameObject mainPlayer;
    private GameObject activePlayer;
    private GameObject enemyContainer;
    private GameObject[] enemies;
    private bool sceneHasEnemies;


    // Use this for initialization
    void Start()
    {
        resetLocation = GameObject.FindGameObjectWithTag("Respawn");


        for (int i = 0; i < playerOptions.Length; i++)
        {
            if (playerOptions[i].name.Contains(SaveAndLoadGame.saver.GetCharacterType()))
            {
                playerOptions[i].SetActive(true);
                mainPlayer = playerOptions[i];
                activePlayer = playerOptions[i].transform.GetChild(2).gameObject;
            }
            else
            {
                playerOptions[i].SetActive(false);
            }
        }
        enemyContainer = GameObject.FindGameObjectWithTag("EnemyContainer");

        if (enemyContainer == null)
        {
            sceneHasEnemies = false;
        }
        else
        {
            enemies = new GameObject[enemyContainer.transform.childCount];
            sceneHasEnemies = true;
            for (int i = 0; i < enemyContainer.transform.childCount; i++)
            {
                enemies[i] = enemyContainer.transform.GetChild(i).gameObject;
            }
        }

        if (resetLocation == null)
        {
            resetLocation = new GameObject("ResetSpot");
            resetLocation.transform.position = activePlayer.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.transform.root.tag.Contains("Player"))
        {
            HandleSafteyNetCatch(other.transform.root.gameObject);
        }
        else if (other.gameObject.name.Contains("Projectile"))
        {
            return;
        }
        else if(other.transform.root.tag.Contains("Enemy"))
        {
            if(other.gameObject.name.Contains("Projectile"))
            {
                return; //do not want to destroy enemy from projectiles flying out of the world
            }
            GameObject findingRoot = other.gameObject;
            while (findingRoot.tag != "EnemyRoot")
            {
                findingRoot = findingRoot.transform.parent.gameObject;
            }
            HandleSafteyNetCatch(findingRoot);
        }
        else
        {
            Destroy(other.gameObject);
        }
        
    }


    private void HandleSafteyNetCatch(GameObject heWhoLeftTheWorld)
    {
        if(heWhoLeftTheWorld.tag.Contains("Player"))
        {
            SaveAndLoadGame.saver.SetLoadedFightScene(true);
            GameControllerScript gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
            HashSet<int> allyI = gameController.hasAllies ? gameController.AllyAliveIndicies() : new HashSet<int>();
            SaveAndLoadGame.saver.SetFightSceneSaveValues(gameController.EnemyAliveIndicies(), gameController.hasAllies, allyI);
            if (heWhoLeftTheWorld.name.Contains("Cube"))
            {
                SaveAndLoadGame.saver.SetPlayerCurrentHealth(heWhoLeftTheWorld.GetComponent<CubeSpecialStats>().GetPlayerHealth());
            }
            else
            {
                SaveAndLoadGame.saver.SetPlayerCurrentHealth(heWhoLeftTheWorld.GetComponent<OctahedronStats>().GetPlayerHealth());
            }
            SaveAndLoadGame.saver.SetCurrentScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            heWhoLeftTheWorld.SendMessage("PlayerBeingReset", resetLocation.transform, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            heWhoLeftTheWorld.SendMessage("ResetEnemy", SendMessageOptions.DontRequireReceiver);
        }
    }
}
