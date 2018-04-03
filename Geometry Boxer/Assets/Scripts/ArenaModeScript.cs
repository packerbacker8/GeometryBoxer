using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;
using PlayerUI;

public class ArenaModeScript : GameControllerScript
{
    public GameObject enemySpawnSetGameObject;
    public GameObject healthSpawnSetGameObject;
    public GameObject botCubePrefab;
    public GameObject botSpecialCubePrefab;
    public GameObject botOctahedronPrefab;
    public GameObject botSpecialOctahedronPrefab;
    public GameObject healthPrefab;
    [Tooltip("Roughly how many seconds between each wave.")]
    public float timeBetweenWaves = 3.0f;
    [Tooltip("How many enemies spawn at the start?")]
    public int startingWaveAmount = 2;
    [Tooltip("How many enemies are added each wave?")]
    public int waveGrowthAmount = 2;
    [Tooltip("What multiple of wave the special enemies will spawn on.")]
    public int specialSpawnWaveMultiple = 5;
    [Tooltip("On the first wave special enemies start in, this is how many there will be.")]
    public int specialEnemyStartingAmount = 2;
    [Tooltip("How many special enemies to spawn the next time around will be added on to by this factor.")]
    public int specialEnemySpawnFactor = 2;
    public int numberOfWavesPreloaded = 20;
    public int numberOfHealthpacks = 3;


    private List<GameObject>[] currentWavesAllocation;
    private List<GameObject>[] healthPickupPool;
    private GameObject healthContainer;
    private int currentWaveIndex;

    //just an idea to see what spawns are taken. The key is the position.x + position.y + position.z of the transform's position.
    //private Dictionary<int, Transform> spawnDictionary;

    private SpawnSet enemySpawnSet;
    private SpawnSet healthSpawnSet;
    private PlayerUserInterface playerUIScript;
    private PlayerUserInterface player2UIScript;

    private int numberOfRegularEnemiesToSpawn;
    private int numberOfSpecialEnemiesToSpawn;
    private int numberOfHealthToSpawn;
    private int currentWaveNumber;

    private float timeBeforeWaveBegins;

    private bool waveActive;
    private bool waveReady;
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
        enemySpawnSet = enemySpawnSetGameObject.GetComponentInChildren<SpawnSet>();
        healthSpawnSet = healthSpawnSetGameObject.GetComponentInChildren<SpawnSet>();
        healthContainer = GameObject.FindGameObjectWithTag("HealthContainer");

        waveActive = true;
        currentWavesAllocation = new List<GameObject>[numberOfWavesPreloaded];
        currentWaveNumber = 0;
        currentWaveIndex = 0;
        numberOfRegularEnemiesToSpawn = startingWaveAmount;
        numberOfSpecialEnemiesToSpawn = specialEnemyStartingAmount;
        numberOfHealthToSpawn = 0;
        healthPickupPool = new List<GameObject>[numberOfWavesPreloaded];

    }

    protected override void Update()
    {
        //if the wave is not active, that means we are either waiting, or we are prepping for the next wave.
        if (!waveActive)
        {
            if (timeBeforeWaveBegins >= timeBetweenWaves)
            {
                //set every object in the current List<Object> that corresponds to the current wave to active
                foreach (GameObject obj in currentWavesAllocation[currentWaveIndex - 1])
                {
                    obj.SetActive(true);
                    switchPlayers = player2CharController == null ? false : !switchPlayers;
                    switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                    switchPlayers = player2Alive ? switchPlayers : false;
                    obj.GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                    obj.transform.parent = isCube ? enemyOctahedronContainer.transform : enemyCubeContainer.transform;
                }

                foreach (GameObject health in healthPickupPool[currentWaveIndex - 1])
                {
                    health.SetActive(true);
                    health.transform.parent = healthContainer.transform;
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

        //if the number of enemies is 0, then that means a wave is defeated and we need to get everything 
        //set to start the next wave
        if (numEnemiesAlive <= 0)
        {
            #region
            //if both players are dead
            //might be used if we have a win condition
            /*if (!playerAlive && !player2Alive)
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
            }*/
            #endregion

            //if there are any remaining health pickups, remove them.
            if (currentWaveIndex - 1 >= 0)
            {
                foreach (GameObject health in healthPickupPool[currentWaveIndex - 1])
                {
                    Destroy(health);
                }
            }
            currentWaveNumber++;
            //set next wave active
            currentWaveIndex = currentWaveIndex % numberOfWavesPreloaded;
            if (currentWaveIndex == 0)
            {
                waveReady = false;
                StartCoroutine(InstantiateNewWavePool());
            }
            else
            {
                waveReady = true;
            }
            if (waveReady)
            {
                PrepareForNextWave(currentWaveIndex);
                currentWaveIndex++;
            }
        }
    }

    /// <summary>
    /// Preinstantiates a number of enemy objects and waves into the current allocation.
    /// How may waves instatiated depend on the <c>numberOfWavesPreloaded</c> variable.
    /// </summary>
    private IEnumerator InstantiateNewWavePool()
    {
        for (int i = 0; i < numberOfWavesPreloaded; i++)
        {
            //make a spawnList and set it to the i'th preloaded wave
            List<GameObject> spawnList = new List<GameObject>();
            GameObject currentEnemy;
            for (int j = 0; j < numberOfRegularEnemiesToSpawn; j++)
            {
                //wave to spawn specials
                if(j < numberOfSpecialEnemiesToSpawn && (currentWaveNumber + i) % specialSpawnWaveMultiple == 0)
                {
                    //player is cube
                    if (isCube)
                    {
                        currentEnemy = Instantiate(botSpecialOctahedronPrefab, enemySpawnSet.getRandomSpawnTransform(), false);
                    }
                    else
                    {
                        Transform spawnTransform = enemySpawnSet.getRandomSpawnTransform();
                        currentEnemy = Instantiate(botSpecialCubePrefab, spawnTransform.position, spawnTransform.rotation);
                    }
                }
                else //otherwise spawn regular
                {
                    //player is cube
                    if (isCube)
                    {
                        currentEnemy = Instantiate(botOctahedronPrefab, enemySpawnSet.getRandomSpawnTransform(), false);
                    }
                    else
                    {
                        Transform spawnTransform = enemySpawnSet.getRandomSpawnTransform();
                        currentEnemy = Instantiate(botCubePrefab, spawnTransform.position, spawnTransform.rotation);
                    }
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
                Transform spawnTransform = healthSpawnSet.getRandomSpawnTransform();
                currentHealthPickup = Instantiate(healthPrefab, spawnTransform.position, spawnTransform.rotation);
                currentHealthPickup.SetActive(false);
                healthPickupList.Add(currentHealthPickup);
            }
            healthPickupPool[i] = healthPickupList;

            //set the parameters for the next wave
            numberOfRegularEnemiesToSpawn += waveGrowthAmount;
            if ((currentWaveNumber + i) % specialSpawnWaveMultiple == 0)
            {
                numberOfSpecialEnemiesToSpawn += specialEnemySpawnFactor;
            }
            if (currentWaveNumber >= 1)
            {
                numberOfHealthToSpawn = numberOfHealthpacks; //currently no change in the number of healthpacks, could grow each round or every couple rounds
            }
        }
        //initialize AI variables for every object in current pool just created
        InitAI();
        waveReady = true;
        yield return null;
    }

    /// <summary>
    /// Adjusts enemiesInWorld array to be proper for each wave, and fix which number of enemies to start expanding sightline on
    /// which won't matter in this case as all enemies know where the player is at all times. Also sets the wave as not active.
    /// </summary>
    /// <param name="waveIndex"></param>
    private void PrepareForNextWave(int waveIndex)
    {
        //load the first wave enemies
        numEnemiesAlive = currentWavesAllocation[waveIndex].Count;
        oldExpansionNumEnemies = numEnemiesAlive;

        //might be able to get away without having to do this
        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            Destroy(enemiesInWorld[i]);
        }
        enemiesInWorld = new GameObject[numEnemiesAlive];

        int index = 0;
        foreach (GameObject enemyObj in currentWavesAllocation[waveIndex])
        {
            enemiesInWorld[index] = enemyObj;
            index++;
        }

        waveActive = false;
    }

    /// <summary>
    /// Initializes AI object's variables for every object in the current wave pool.
    /// </summary>
    public void InitAI()
    {
        for (int i = 0; i < numberOfWavesPreloaded; i++)
        {
            int enemyIndex = 0;
            foreach (GameObject currentEnemyObject in currentWavesAllocation[i])
            {
                currentEnemyObject.GetComponent<EnemyHealthScript>().SetEnemyIndex(enemyIndex);
                currentEnemyObject.GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
                currentEnemyObject.GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;

                switchPlayers = player2CharController == null ? false : !switchPlayers;
                switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                switchPlayers = player2Alive ? switchPlayers : false;
                currentEnemyObject.GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);

                enemyIndex++;
            }
        }
    }

    /// <summary>
    /// Returns the 1 based wave number the player is on.
    /// </summary>
    /// <returns>The wave that is currently active, starts counting at 1.</returns>
    public int GetWaveNumber()
    {
        return currentWaveNumber + 1;
    }

}
