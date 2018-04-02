﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;
using PlayerUI;

public class ArenaModeScript : GameControllerScript
{
    public GameObject spawnSetGameObject;
    public GameObject botCubePrefab;
    public GameObject botSpecialCubePrefab;
    public GameObject botOctahedronPrefab;
    public GameObject botSpecialOctahedronPrefab;
    public GameObject healthPrefab;
    public int specialSpawnWaveMultiple = 5;
    public static int numberOfWavesPreloaded;


    private List<GameObject>[] currentWavesAllocation;
    private List<GameObject>[] healthPickupPool;
    private int currentWaveIndex;

    //just an idea to see what spawns are taken. The key is the position.x + position.y + position.z of the transform's position.
    //private Dictionary<int, Transform> spawnDictionary;

    private SpawnSet gameSpawnSet;
    private PlayerUserInterface playerUIScript;
    private PlayerUserInterface player2UIScript;

    private int numberOfRegularEnemiesToSpawn;
    private int numberOfHealthToSpawn;
    private int currentWaveNumber;

    private float timeBeforeWaveBegins;
    private float timeToStartWave = 3.0f;

    private bool waveActive = true;
    private bool isCube;

    protected override void Awake()
    {
        base.Awake();
        if (enemyContainer == enemyOctahedronContainer)
        {
            isCube = true;
        }
        else
        {
            isCube = false;
        }
        numEnemiesAlive = 0;
    }



    protected override void Start()
    {
        base.Start();
        playerUIScript = playerUI.GetComponent<PlayerUserInterface>();
        player2UIScript = IsSplitScreen ? player2UI.GetComponent<PlayerUserInterface>() : null;
        gameSpawnSet = spawnSetGameObject.GetComponentInChildren<SpawnSet>();

        numberOfWavesPreloaded = 5;
        currentWavesAllocation = new List<GameObject>[numberOfWavesPreloaded];
        currentWaveNumber = 0;
        currentWaveIndex = 0;
        numberOfRegularEnemiesToSpawn = 2;
        numberOfHealthToSpawn = 0;
        healthPickupPool = new List<GameObject>[numberOfWavesPreloaded];

    }

    protected override void Update()
    {
        if (!waveActive)
        {
            if (timeBeforeWaveBegins >= timeToStartWave)
            {
                //set every object in the current List<Object> that corresponds to the current wave to active
                foreach (GameObject obj in currentWavesAllocation[currentWaveIndex - 1])
                {
                    obj.SetActive(true);
                }

                foreach (GameObject health in healthPickupPool[currentWaveIndex - 1])
                {
                    health.SetActive(true);
                }

                playerUIScript.reinitializeUI(numEnemiesAlive);
                if (IsSplitScreen)
                {
                    player2UIScript.reinitializeUI(numEnemiesAlive);
                }

                timeBeforeWaveBegins = 0.0f;
                waveActive = true;
            }
            else
            {
                timeBeforeWaveBegins += Time.deltaTime;
            }
        }

        if (numEnemiesAlive <= 0)
        {
            //if both players are dead
            //might be used if we have a win condition
            if (!playerAlive && !player2Alive) /*&& currentWaveNumber == lastWaveNumber*/
            {
                SaveAndLoadGame.saver.SetCityStatus(currentMapName, "conquered");

                //disable any pause menu at this point
                pauseMenu.GetComponent<PauseMenu>().pauseMenuCanvas.SetActive(false);

                //display win menu
                winMenuObj.SetActive(true);
                WinMenu winMenu = winMenuObj.GetComponent<WinMenu>();
                winMenu.setButtonActive();
                winMenu.setMouse();

                levelWon = true;
            }
            else if (!levelWon)
            {
                //if there are any remaining health pickups, remove them.
                if (currentWaveIndex - 1 >= 0)
                {
                    foreach (GameObject health in healthPickupPool[currentWaveIndex - 1])
                    {
                        Destroy(health);
                    }
                }
                currentWaveNumber += 1;
                //set next wave active
                currentWaveIndex = currentWaveIndex % numberOfWavesPreloaded;
                if (currentWaveIndex == 0)
                {
                    InstantiateNewWavePool();
                }
                prepareForNextWave(currentWaveIndex);
                currentWaveIndex++;
            }


        }
    }

    /* Preinstantiates a number of enemy objects and waves into the current allocation*/
    private void InstantiateNewWavePool()
    {
        for (int i = 0; i < numberOfWavesPreloaded; i++)
        {
            //make a spawnList and set it to the i'th preloaded wave
            List<GameObject> spawnList = new List<GameObject>();
            GameObject currentEnemy;
            for (int j = 0; j < numberOfRegularEnemiesToSpawn; j++)
            {
                if (isCube)
                {
                    currentEnemy = Instantiate(botOctahedronPrefab, gameSpawnSet.getRandomSpawnTransform(), false);
                }
                else
                {
                    Transform spawnTransform = gameSpawnSet.getRandomSpawnTransform();
                    currentEnemy = Instantiate(botCubePrefab, spawnTransform.position, spawnTransform.rotation);
                }

                currentEnemy.SetActive(false);
                spawnList.Add(currentEnemy);
            }
            currentWavesAllocation[i] = spawnList;

            //make a healthPickupList and set it to the i'th preloaded wave
            List<GameObject> healthPickupList = new List<GameObject>();
            GameObject currentHealthPickup;
            for (int j = 0; j < numberOfHealthToSpawn; j++)
            {
                Transform spawnTransform = gameSpawnSet.getRandomSpawnTransform();
                currentHealthPickup = Instantiate(healthPrefab, spawnTransform.position, spawnTransform.rotation);
                currentHealthPickup.SetActive(false);
                healthPickupList.Add(currentHealthPickup);
            }
            healthPickupPool[i] = healthPickupList;

            //set the parameters for the next wave
            numberOfRegularEnemiesToSpawn = numberOfRegularEnemiesToSpawn + 2;
            if (currentWaveNumber >= 1)
            {
                numberOfHealthToSpawn = 1;
            }
        }
        //initialize AI variables for every object in current pool just created
        initAI();
    }

    /*this method basically just takes care of the enemiesInWorld and oldExpansionNumEnemies variables from GameController script*/
    private void prepareForNextWave(int waveIndex)
    {
        //load the first wave enemies
        numEnemiesAlive = currentWavesAllocation[waveIndex].Count;
        oldExpansionNumEnemies = numEnemiesAlive;

        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            Destroy(enemiesInWorld[i]);
        }
        enemiesInWorld = new GameObject[numEnemiesAlive];

        int index = 0;
        foreach (GameObject enemyObj in currentWavesAllocation[waveIndex])
        {
            enemiesInWorld[index] = enemyObj;
        }

        waveActive = false;
    }


    //Initializes AI object's variables for every object in the current wave pool.
    public void initAI()
    {
        for (int i = 0; i < numberOfWavesPreloaded; i++)
        {
            int enemyIndex = 0;
            foreach (GameObject currentEnemyObject in currentWavesAllocation[i])
            {
                currentEnemyObject.GetComponent<EnemyHealthScript>().SetEnemyIndex(enemyIndex);
                currentEnemyObject.GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
                currentEnemyObject.GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;
                currentEnemyObject.GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);

                switchPlayers = player2CharController == null ? false : !switchPlayers;
                currentEnemyObject.GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);

                enemyIndex++;
            }
        }
    }

}
