using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;
using PlayerUI;

public class ArenaModeScript : GameControllerScript
{
    public GameObject botCubePrefab;
    public GameObject botSpecialCubePrefab;
    public GameObject botOctahedronPrefab;
    public GameObject botSpecialOctahedronPrefab;
    public GameObject healthPrefab;
    [Tooltip("How much health the bots start with.")]
    public float startingEnemyHealth = 100f;
    [Tooltip("How much does the health go up by per round.")]
    public float healthGrowthAmount = 1.5f;
    [Tooltip("How much more health do the special bots have.")]
    public float specialBotHealthFactor = 2.0f;
    [Tooltip("Roughly how many seconds between each wave.")]
    public float timeBetweenWaves = 3.0f;
    [Tooltip("How many enemies are added each wave? Also how many enemies at the start")]
    public int waveGrowthAmount = 3;
    [Tooltip("What multiple of wave the special enemies will spawn on.")]
    public int specialSpawnWaveMultiple = 5;
    [Tooltip("On the first wave special enemies start in, this is how many there will be.")]
    public int specialEnemyStartingAmount = 2;
    [Tooltip("How many special enemies to spawn the next time around will be added on to by this factor.")]
    public int specialEnemySpawnFactor = 2;
    public int numberOfWavesPreloaded = 20;
    public int numberOfHealthpacks = 3;
    [Tooltip("Max number of enemies in the world at a time.")]
    public int enemyCap = 30;
    [Header("Framerate info")]
    public float refreshTime = 0.5f;


    private List<GameObject> enemyPool;
    private List<GameObject> specialEnemyPool;
    private List<Transform>[] healthPickupTransforms;
    private List<GameObject> healthPickups;
    private GameObject enemySpawnSetGameObject;
    private GameObject healthSpawnSetGameObject;
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
    private int enemyPoolCount;
    private int specialEnemyPoolCount;
    private int frameCounter;
    private int frameRefreshesSeen;
    private int leftoverEnemies;
    private int leftoverSpecialEnemies;

    private float timeBeforeWaveBegins;
    private float botHealth;
    private float timeCounter;
    private float lastFramerate;
    private float framerateTotal;
    private float highestFramerateSeen;

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
        enemySpawnSetGameObject = GameObject.FindGameObjectWithTag("RootSpawn");
        healthSpawnSetGameObject = GameObject.FindGameObjectWithTag("HealthSpawn");
        playerUIScript = playerUI.GetComponent<PlayerUserInterface>();
        player2UIScript = IsSplitScreen ? player2UI.GetComponent<PlayerUserInterface>() : null;
        enemySpawnSet = enemySpawnSetGameObject.GetComponentInChildren<SpawnSet>();
        healthSpawnSet = healthSpawnSetGameObject.GetComponentInChildren<SpawnSet>();
        healthContainer = GameObject.FindGameObjectWithTag("HealthContainer");

        waveActive = true;
        enemyPoolCount = numberOfWavesPreloaded * waveGrowthAmount;
        specialEnemyPoolCount = (numberOfWavesPreloaded * waveGrowthAmount) / specialSpawnWaveMultiple;
        enemyPool = new List<GameObject>(enemyPoolCount * 2);
        specialEnemyPool = new List<GameObject>(specialEnemyPoolCount * 2);
        int resetWaveNum = SaveAndLoadGame.saver.GetWaveOn();
        currentWaveNumber = resetWaveNum == -1 ? 0 : resetWaveNum;
        currentWaveIndex = 0;
        numberOfRegularEnemiesToSpawn = waveGrowthAmount * (currentWaveNumber == 0 ? 1 : currentWaveNumber);
        numberOfSpecialEnemiesToSpawn = specialEnemyStartingAmount * (currentWaveNumber == 0 ? 1 : currentWaveNumber);
        numberOfHealthToSpawn = numberOfHealthpacks;
        healthPickupTransforms = new List<Transform>[numberOfWavesPreloaded];
        //make a healthPickupList and set it to the i'th preloaded wave
        healthPickups = new List<GameObject>();
        GameObject currentHealthPickup;
        for (int j = 0; j < numberOfHealthToSpawn; j++)
        {
            currentHealthPickup = Instantiate(healthPrefab, Vector3.zero, Quaternion.identity);
            currentHealthPickup.SetActive(true);
            currentHealthPickup.transform.parent = healthContainer.transform;
            healthPickups.Add(currentHealthPickup);
        }
        botHealth = currentWaveNumber == 0 ? startingEnemyHealth : startingEnemyHealth * Mathf.Pow(healthGrowthAmount, currentWaveNumber - 1);

        lastFramerate = 0.0f;
        timeCounter = 0.0f;
        framerateTotal = 0.0f;
        highestFramerateSeen = 0.0f;
        frameCounter = 0;
        frameRefreshesSeen = 0;

        leftoverEnemies = 0;
        leftoverSpecialEnemies = 0;
    }

    protected override void Update()
    {
        /*
        //Info for framerate stuff
        if (timeCounter < refreshTime)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            lastFramerate = frameCounter / timeCounter;
            if(lastFramerate > highestFramerateSeen)
            {
                highestFramerateSeen = lastFramerate;
            }
            framerateTotal += lastFramerate;
            frameRefreshesSeen++;
            frameCounter = 0;
            timeCounter = 0.0f;
        }*/
        //if the wave is not active, that means we are either waiting, or we are prepping for the next wave.
        if (!waveActive)
        {
            if (timeBeforeWaveBegins >= timeBetweenWaves)
            {
                /*
                float avgFramerate = framerateTotal / frameRefreshesSeen;
                if(avgFramerate < highestFramerateSeen/3.0f)
                {
                    enemyCap = enemiesInWorld.Length;
                }
                frameRefreshesSeen = 0;
                framerateTotal = 0f;*/
                //set every object in the current List<Object> that corresponds to the current wave to active
                foreach (GameObject obj in enemiesInWorld)
                {
                    obj.SetActive(true);
                    float tempHealth = botHealth;
                    if(obj.GetComponent<SpecialCubeAttackAI>() != null || obj.GetComponent<SpecialOctahedronAttackAI>() != null)
                    {
                        tempHealth *= specialBotHealthFactor;
                    }
                    obj.GetComponent<ArenaEnemyHealthScript>().SetOriginalHealth(tempHealth);
                    obj.GetComponent<ArenaEnemyHealthScript>().ResetValues(enemySpawnSet.getRandomSpawnTransform3D());
                    obj.GetComponent<ArenaEnemyHealthScript>().UpdateEnemyMaterial();
                    switchPlayers = player2CharController == null ? false : !switchPlayers;
                    switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                    switchPlayers = player2Alive ? switchPlayers : false;
                    obj.GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                    obj.transform.parent = isCube ? enemyOctahedronContainer.transform : enemyCubeContainer.transform;
                }
                botHealth *= healthGrowthAmount; //grow the enemy health each wave
                for (int i = 0; i < healthPickupTransforms[currentWaveIndex - 1].Count; i++)
                {
                    healthPickups[i % healthPickups.Count].transform.position = healthPickupTransforms[currentWaveIndex - 1][i].position;
                    healthPickups[i % healthPickups.Count].GetComponent<HealthPickup>().ChangeStartingPosition(healthPickupTransforms[currentWaveIndex - 1][i].position);
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
            //set next wave active
            currentWaveIndex = currentWaveIndex % numberOfWavesPreloaded;
            if (currentWaveNumber == 0 || SaveAndLoadGame.saver.GetWaveOn() != -1)
            {
                waveReady = false;
                int startEnemy = enemyPoolCount - numberOfWavesPreloaded * waveGrowthAmount;
                int startSpecial = specialEnemyPoolCount - numberOfWavesPreloaded * waveGrowthAmount / 5;
                InstantiateNewWavePool(startEnemy, startSpecial);
                enemyPoolCount += numberOfWavesPreloaded * waveGrowthAmount;
                specialEnemyPoolCount += numberOfWavesPreloaded * waveGrowthAmount / 5;
            }
            else
            {
                waveReady = true;
            }
            if (waveReady)
            {
                currentWaveNumber++;
                PrepareForNextWave(currentWaveIndex);
                currentWaveIndex++;
                numberOfRegularEnemiesToSpawn += waveGrowthAmount;
                if(currentWaveNumber % specialSpawnWaveMultiple == 0)
                {
                    numberOfSpecialEnemiesToSpawn += specialEnemySpawnFactor;
                }
            }
        }
    }

    /// <summary>
    /// Preinstantiates a number of enemy objects and waves into the current allocation.
    /// How may waves instatiated depend on the <c>numberOfWavesPreloaded</c> variable.
    /// </summary>
    private void InstantiateNewWavePool(int startEnemyPool, int startSpecialEnemyPool)
    {
        //generate enemies for enemy pool
        //no need to generate more enemies that will ever be on the screen 
        for (int i = startEnemyPool; i < enemyCap; i++)
        {
            GameObject currentEnemy;
            //player is cube
            if (isCube)
            {
                currentEnemy = Instantiate(botOctahedronPrefab, enemySpawnSet.getRandomSpawnTransform3D(), false);
            }
            else
            {
                Transform spawnTransform = enemySpawnSet.getRandomSpawnTransform3D();
                currentEnemy = Instantiate(botCubePrefab, spawnTransform.position, spawnTransform.rotation);
            }

            currentEnemy.SetActive(false);
            enemyPool.Add(currentEnemy);

            //set the parameters for the next wave
            //numberOfRegularEnemiesToSpawn += waveGrowthAmount;
        }
        //generate health pickup locations
        for (int i = 0; i < numberOfWavesPreloaded; i++)
        {
            //make a healthPickupList and set it to the i'th preloaded wave
            List<Transform> healthPickupList = new List<Transform>();
            Transform healthSpawnTransform;
            for (int j = 0; j < numberOfHealthToSpawn; j++)
            {
                healthSpawnTransform = healthSpawnSet.getRandomSpawnTransform2D();
                while (healthPickupList.Contains(healthSpawnTransform))
                {
                    healthSpawnTransform = healthSpawnSet.getRandomSpawnTransform2D();
                }
                healthPickupList.Add(healthSpawnTransform);
            }
            healthPickupTransforms[i] = healthPickupList;
        }
        //generate special enemy pool 
        //no need to generate more enemies than there ever will be
        for (int i = startSpecialEnemyPool; i < enemyCap; i++)
        {
            GameObject currentEnemy;

            //player is cube
            if (isCube)
            {
                currentEnemy = Instantiate(botSpecialOctahedronPrefab, enemySpawnSet.getRandomSpawnTransform3D(), false);
            }
            else
            {
                Transform spawnTransform = enemySpawnSet.getRandomSpawnTransform3D();
                currentEnemy = Instantiate(botSpecialCubePrefab, spawnTransform.position, spawnTransform.rotation);
            }
            currentEnemy.SetActive(false);
            specialEnemyPool.Add(currentEnemy);
        }
        //initialize AI variables for every object in current pool just created
        InitAI();
        waveReady = true;
    }

    /// <summary>
    /// Adjusts enemiesInWorld array to be proper for each wave, and fix which number of enemies to start expanding sightline on
    /// which won't matter in this case as all enemies know where the player is at all times. Also sets the wave as not active.
    /// </summary>
    /// <param name="waveIndex"></param>
    private void PrepareForNextWave(int waveIndex)
    {
        //load the first wave enemies
        numEnemiesAlive = numberOfRegularEnemiesToSpawn;
        int numEnemiesToSpawn = numEnemiesAlive > enemyCap ? enemyCap : numEnemiesAlive;
        leftoverEnemies = numEnemiesAlive - numEnemiesToSpawn;
        oldExpansionNumEnemies = numEnemiesAlive;
        int numSpecialToSpawn = currentWaveNumber % specialSpawnWaveMultiple == 0 ? numberOfSpecialEnemiesToSpawn : 0;
        int numSpecialToActuallySpawn = numSpecialToSpawn > enemyCap ? enemyCap : numSpecialToSpawn;
        leftoverSpecialEnemies = numSpecialToSpawn - numSpecialToActuallySpawn;
        leftoverEnemies -= leftoverSpecialEnemies;

        enemiesInWorld = new GameObject[numEnemiesToSpawn];
        for (int i = 0; i < numEnemiesToSpawn; i++)
        {
            enemiesInWorld[i] = enemyPool[i];
        }
        for (int i = 0 ; i < numSpecialToActuallySpawn; i++)
        {
            enemiesInWorld[i] = specialEnemyPool[i];
        }

        waveActive = false;
    }

    /// <summary>
    /// Initializes AI object's variables for every object in the current wave pool.
    /// </summary>
    public void InitAI()
    {
        int enemyIndex = 0;
        foreach (GameObject currentEnemyObject in enemyPool)
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
        enemyIndex = 0;
        foreach(GameObject currentSpecialEnemy in specialEnemyPool)
        {
            currentSpecialEnemy.GetComponent<EnemyHealthScript>().SetEnemyIndex(enemyIndex);
            currentSpecialEnemy.GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
            currentSpecialEnemy.GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;

            switchPlayers = player2CharController == null ? false : !switchPlayers;
            switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
            switchPlayers = player2Alive ? switchPlayers : false;
            currentSpecialEnemy.GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);

            enemyIndex++;
        }
    }

    /// <summary>
    /// Returns the 1 based wave number the player is on.
    /// </summary>
    /// <returns>The wave that is currently active, starts counting at 1.</returns>
    public int GetWaveNumber()
    {
        return currentWaveNumber;
    }


    /// <summary>
    /// Updates how many enemies are alive after one is killed.
    /// </summary>
    /// <param name="index">Index in the respective array.</param>
    /// <param name="tag">Tag of the object sent.</param>
    public override bool IsKilled(int index, string tag)
    {
        bool actuallyKill = true;
        if (hasAllies)
        {
            if (tag.Contains("Enemy"))
            {
                numEnemiesAlive--;
                if(leftoverSpecialEnemies > 0)
                {
                    leftoverSpecialEnemies--;
                    enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().SetOriginalHealth(botHealth * specialBotHealthFactor);
                    enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().ResetValues(enemySpawnSet.getRandomSpawnTransform3D());
                    switchPlayers = player2CharController == null ? false : !switchPlayers;
                    switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                    switchPlayers = player2Alive ? switchPlayers : false;
                    enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                    enemiesInWorld[index].transform.parent = isCube ? enemyOctahedronContainer.transform : enemyCubeContainer.transform;
                    actuallyKill = false;
                }
                else if(leftoverEnemies > 0)
                {
                    leftoverEnemies--;
                    enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().SetOriginalHealth(botHealth);
                    enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().ResetValues(enemySpawnSet.getRandomSpawnTransform3D());
                    switchPlayers = player2CharController == null ? false : !switchPlayers;
                    switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                    switchPlayers = player2Alive ? switchPlayers : false;
                    enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                    enemiesInWorld[index].transform.parent = isCube ? enemyOctahedronContainer.transform : enemyCubeContainer.transform;
                    actuallyKill = false;
                }
                else
                {
                    enemiesInWorld[index] = null;
                }
            }
            else
            {
                alliesInWorld[index] = null;
            }
        }
        else
        {
            numEnemiesAlive--;
            if (leftoverSpecialEnemies > 0)
            {
                leftoverSpecialEnemies--;
                enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().SetOriginalHealth(botHealth * specialBotHealthFactor);
                enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().ResetValues(enemySpawnSet.getRandomSpawnTransform3D());
                switchPlayers = player2CharController == null ? false : !switchPlayers;
                switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                switchPlayers = player2Alive ? switchPlayers : false;
                enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                enemiesInWorld[index].transform.parent = isCube ? enemyOctahedronContainer.transform : enemyCubeContainer.transform;
                actuallyKill = false;
            }
            else if (leftoverEnemies > 0)
            {
                leftoverEnemies--;
                enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().SetOriginalHealth(botHealth);
                enemiesInWorld[index].GetComponent<ArenaEnemyHealthScript>().ResetValues(enemySpawnSet.getRandomSpawnTransform3D());
                switchPlayers = player2CharController == null ? false : !switchPlayers;
                switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
                switchPlayers = player2Alive ? switchPlayers : false;
                enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                enemiesInWorld[index].transform.parent = isCube ? enemyOctahedronContainer.transform : enemyCubeContainer.transform;
                actuallyKill = false;
            }
            else
            {
                enemiesInWorld[index] = null;
            }
        }
        playerUI.GetComponent<PlayerUI.PlayerUserInterface>().EnemiesLeft(numEnemiesAlive);
        if (IsSplitScreen) player2UI.GetComponent<PlayerUI.PlayerUserInterface>().EnemiesLeft(numEnemiesAlive);
        return actuallyKill;
    }

    /*
/// <summary>
/// Tells the game controller the player died.
/// </summary>
/// <param name="p2">If this is true, the player that died is player 2.</param>
public override void PlayerKilled(bool p2)
{
    if (p2)
    {
        player2Alive = false;
    }
    else
    {
        playerAlive = false;
    }
    if (!playerAlive && !player2Alive)
    {
        SaveAndLoadGame.saver.SetCityStatus(currentMapName, "notconquered");

        //disable any pause menu at this point
        pauseMenu.GetComponent<PauseMenu>().pauseMenuCanvas.SetActive(false);

        //display death menu
        deathMenuObj.SetActive(true);
        DeathMenu deathMenu = deathMenuObj.GetComponent<DeathMenu>();
        deathMenu.SetReloadString(deathReloadMap);
        deathMenu.setButtonActive();
        deathMenu.setMouse();

        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            if (enemiesInWorld[i].activeInHierarchy && enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>() != null)
            {
                enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(0, true);
                enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(1, IsSplitScreen);
            }
        }
    }
    else
    {
        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            if (enemiesInWorld[i].activeInHierarchy && enemiesInWorld[i].GetComponentInChildren<NormalMovementAI>() != null)
            {
                ChangeTarget(enemiesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex(), !p2);
            }
            else if (enemiesInWorld[i].activeInHierarchy && enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>() != null)
            {
                enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(p2 ? 1 : 0, false);
            }
        }
    }

    //LoadLevel.loader.LoadALevel(deathReloadMap); //index of the scene the player is currently on
}

/// <summary>
/// Function to set new target for bots to attack.
/// </summary>
/// <param name="index">The bot index in their respective array.</param>
/// <param name="tag">Tag of their root object</param>
public override void SetNewTarget(int index, string tag)
{
    if (hasAllies)
    {
        if (tag.Contains("Enemy"))
        {
            while (enemyTargetQueue.Count != 0)
            {
                GameObject target = enemyTargetQueue.Dequeue();
                if (target != null && target.activeInHierarchy)
                {
                    enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(target.transform.GetChild(charControllerIndex).gameObject);
                    enemyTargetQueue.Enqueue(target);
                    return;
                }
            }
            switchPlayers = IsSplitScreen ? !switchPlayers : false;
            enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
        }
        else
        {
            while (allyTargetQueue.Count != 0)
            {
                GameObject target = allyTargetQueue.Dequeue();
                if (target != null && target.activeInHierarchy)
                {
                    alliesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(target.transform.GetChild(charControllerIndex).gameObject);
                    allyTargetQueue.Enqueue(target);
                    return;
                }
            }
        }
    }
    else
    {
        switchPlayers = IsSplitScreen ? !switchPlayers : false;
        switchPlayers = playerAlive ? switchPlayers : true; //could cause a crash if no player 2 and player 1 is dead and enemies are trying to target still
        switchPlayers = IsSplitScreen && player2Alive ? switchPlayers : false;
        enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
    }
}

public override void SetTargetHealthPack(int index, GameObject objOfHealth, string tag)
{
    if (tag.Contains("Ally"))
    {
        if (alliesInWorld[index] != null && alliesInWorld[index].activeInHierarchy)
        {
            alliesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(objOfHealth);
        }
    }
    else
    {
        if (enemiesInWorld[index] != null && enemiesInWorld[index].activeInHierarchy)
        {
            enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(objOfHealth);
        }
    }
}

/// <summary>
/// Coroutine to expand enemies' (currently alive in map) sight radius to be larger by the given amount.
/// </summary>
/// <returns>Nothing is returned.</returns>
/// <param name="amount">This amount will be multiplied to the enemies current sight radius.</param>
protected override IEnumerator IncreaseEnemySight(float amount)
{
    yield return null;
    foreach (GameObject enemy in enemiesInWorld)
    {
        if (enemy != null && enemy.activeInHierarchy && enemy.GetComponentInChildren<Detect_Movement_AI>() != null)
        {
            enemy.GetComponentInChildren<Detect_Movement_AI>().IncreaseSight(amount);
        }
    }
    yield return null;
}

/// <summary>
/// Function to give enemies alive indicies.
/// </summary>
/// <returns>Returns enemies that are being tracked as alive in enemiesInWorld container.</returns>
public override HashSet<int> EnemyAliveIndicies()
{
    HashSet<int> enemiesAlive = new HashSet<int>();
    for (int i = 0; i < enemiesInWorld.Length; i++)
    {
        if (enemiesInWorld[i] != null && enemiesInWorld[i].activeInHierarchy)
        {
            enemiesAlive.Add(enemiesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex());
        }
    }
    return enemiesAlive;
}

/// <summary>
/// Function to give allies alive indicies.
/// </summary>
/// <returns>Returns allies that are being tracked as alive in alliesInWorld container.</returns>
public override HashSet<int> AllyAliveIndicies()
{
    HashSet<int> alliesAlive = new HashSet<int>();
    for (int i = 0; i < alliesInWorld.Length; i++)
    {
        if (alliesInWorld[i] != null && alliesInWorld[i].activeInHierarchy)
        {
            alliesAlive.Add(alliesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex());
        }
    }
    return alliesAlive;
}*/
}
