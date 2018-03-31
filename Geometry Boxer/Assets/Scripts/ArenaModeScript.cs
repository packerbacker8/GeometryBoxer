using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;
using PlayerUI;

public class ArenaModeScript : GameControllerScript {

    //public GameObject[] Waves;
    //public GameObject tempEnemyContainer;
    public GameObject spawnSetGameObject;
    private SpawnSet gameSpawnSet;
    private int numberOfRegularEnemiesToSpawn;
    private int numberOfHealthToSpawn;
    public GameObject botCubePrefab;
    public GameObject botOctahedronPrefab;
    public GameObject healthPrefab;
    List<GameObject>[] healthPickupPool;

    public static int numberOfWavesPreloaded;
    List<GameObject>[] currentWavesAllocation;
    private int currentWaveIndex;

    //just an idea to see what spawns are taken. The key is the position.x + position.y + position.z of the transform's position.
    //private Dictionary<int, Transform> spawnDictionary;

    private PlayerUserInterface playerInterface;

    private float timeBeforeWaveBegins;
    private float timeToStartWave = 3.0f;
    private bool waveActive = true;
    private int currentWaveNumber;
    //private int lastWaveNumber;
    private bool isCube;

    //private GameObject currentHealthPickups;
    //private GameObject currentPropPickups;
	//private Transform[] SpawnLocationTransforms; 

    protected override void Awake()
    {
        /*for (int i = 0; i < playerOptions.Length; i++)
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
        charControllerIndex = 2;
        playerCharController = activePlayer.transform.GetChild(charControllerIndex).gameObject;
        enemyContainer = activePlayer.name.Contains("Cube") ? enemyOctahedronContainer : enemyCubeContainer;
        //if we have allies the setup has to include their targets and health along with some tag changes





        //load the first wave enemies
        if (enemyContainer == enemyOctahedronContainer)
        {
            enemyContainer = Waves[0].transform.Find("EnemiesOctahedron").gameObject;
            Waves[0].transform.Find("EnemiesCube").gameObject.SetActive(false);
            isCube = true;
        }
        else
        {
            enemyContainer = Waves[0].transform.Find("EnemiesCube").gameObject;
            Waves[0].transform.Find("EnemiesOctahedron").gameObject.SetActive(false);
            isCube = false;
        }

        //enemyContainer should refer to first wave enemy container.

        numEnemiesAlive = enemyContainer.transform.childCount;
        oldExpansionNumEnemies = numEnemiesAlive;
        enemiesInWorld = new GameObject[numEnemiesAlive];



        for (int i = 0; i < numEnemiesAlive; i++)
        {
            enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
            enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
            enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;
            enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);
            if (enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>() != null)
            {
                enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, null);

            }
            else if (enemyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
            {
                //nothing?
            }
            enemiesInWorld[i] = enemyContainer.transform.GetChild(i).gameObject;
        }


        levelWon = false;
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        //Debug.Log(pauseMenu.GetComponentInChildren<DeathMenu>().ToString());
        deathMenuObj = pauseMenu.GetComponentInChildren<DeathMenu>().gameObject;
        deathMenuObj.SetActive(false);
        winMenuObj = pauseMenu.GetComponentInChildren<WinMenu>().gameObject;
        winMenuObj.SetActive(false);
        playerUI = GameObject.FindGameObjectWithTag("playerUI");

        playerAlive = true;
        currentMapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; */


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

        //lastWaveNumber = Waves.Length;
        //Destroy(tempEnemyContainer);
        playerInterface = GameObject.FindGameObjectWithTag("playerUI").GetComponent<PlayerUserInterface>();
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

                playerInterface.reinitializeUI(numEnemiesAlive);

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
            if (!playerAlive && !player2Alive /*&& currentWaveNumber == lastWaveNumber*/)
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
                if(currentWaveIndex - 1 >= 0)
                {
                    foreach (GameObject health in healthPickupPool[currentWaveIndex - 1])
                    {
                        Destroy(health);
                    }
                }
                

                currentWaveNumber += 1;


                //set next wave active
                //Waves[currentWaveNumber - 1].gameObject.SetActive(true);
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

            if(currentWaveNumber >=  1)
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
        if (isCube)
        {
           //enemyContainer = Waves[waveNumber - 1].transform.Find("EnemiesOctahedron").gameObject;
           // Waves[waveNumber - 1].transform.Find("EnemiesCube").gameObject.SetActive(false);
        }
        else
        {
            //enemyContainer = Waves[waveNumber - 1].transform.Find("EnemiesCube").gameObject;
            //Waves[waveNumber - 1].transform.Find("EnemiesOctahedron").gameObject.SetActive(false);
        }



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

        /*
        for (int i = 1; i < Waves.Length; i++)
        {
            GameObject aContainer = null;
            if(isCube)
            {
                 aContainer = Waves[i].transform.Find("EnemiesOctahedron").gameObject;
            }
            else
            {
                 aContainer = Waves[i].transform.Find("EnemiesCube").gameObject;
            }

            int numEnemies = aContainer.transform.childCount;
            for (int j = 0; j < numEnemies; j++)
            {
                aContainer.transform.GetChild(j).GetComponent<EnemyHealthScript>().SetEnemyIndex(j);
                aContainer.transform.GetChild(j).GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
                aContainer.transform.GetChild(j).GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;
                aContainer.transform.GetChild(j).GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);


                 switchPlayers = player2CharController == null ? false : !switchPlayers;
                 aContainer.transform.GetChild(j).GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);

            }


        }

        //for (int i = 1; i < Waves.Length; i++)
        //{
        //    Waves[i].gameObject.SetActive(false);
        //}

    */
        
    }

}
