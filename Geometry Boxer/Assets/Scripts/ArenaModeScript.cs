using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;
using PlayerUI;

public class ArenaModeScript : GameControllerScript {

    public GameObject[] Waves;

    private PlayerUserInterface playerInterface;

    private float timeBeforeWaveBegins;
    private float timeToStartWave = 3.0f;
    private bool waveActive = false;
    private int currentWaveNumber = 1;
    private int lastWaveNumber;
    private bool isCube;

    private GameObject currentHealthPickups;
    private GameObject currentPropPickups;


    protected override void Awake()
    {
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
                enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayerTransform(playerCharController.transform);

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
        Cursor.visible = false;
        playerInterface = GameObject.FindGameObjectWithTag("playerUI").GetComponent<PlayerUserInterface>();
        playerInterface.reinitializeUI(numEnemiesAlive);

        currentHealthPickups = Waves[0].transform.Find("HealthPickups").gameObject;
        currentPropPickups = Waves[0].transform.Find("Props").gameObject;
        Waves[0].gameObject.SetActive(false);


        //initAI();
    }

    

    protected override void Start()
    {
        base.Start();

        lastWaveNumber = Waves.Length;
    }

    protected override void Update()
    {
        if (!waveActive)
        {
            if (timeBeforeWaveBegins >= timeToStartWave)
            {
                Waves[currentWaveNumber - 1].gameObject.SetActive(true);
                enemyContainer.SetActive(true);
                currentHealthPickups.SetActive(true);
                currentPropPickups.SetActive(true);

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
            if (!levelWon && playerAlive && currentWaveNumber == lastWaveNumber)
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
                //set current wave inactive
                Waves[currentWaveNumber - 1].gameObject.SetActive(false);



                currentWaveNumber += 1;


                //set next wave active
                Waves[currentWaveNumber - 1].gameObject.SetActive(true);

                prepareForNextWave(currentWaveNumber);
            }

            //StartCoroutine(changeLevel(dominationMap));
        }
    }

    private void prepareForNextWave(int waveNumber)
    {
        //load the first wave enemies
        if (isCube)
        {
            enemyContainer = Waves[waveNumber - 1].transform.Find("EnemiesOctahedron").gameObject;
            Waves[waveNumber - 1].transform.Find("EnemiesCube").gameObject.SetActive(false);
        }
        else
        {
            enemyContainer = Waves[waveNumber - 1].transform.Find("EnemiesCube").gameObject;
            Waves[waveNumber - 1].transform.Find("EnemiesOctahedron").gameObject.SetActive(false);
        }

        //enemyContainer should refer to first wave enemy container.

        numEnemiesAlive = enemyContainer.transform.childCount;
        oldExpansionNumEnemies = numEnemiesAlive;
        
        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            Destroy(enemiesInWorld[i]);
        }
        enemiesInWorld = new GameObject[numEnemiesAlive];



        for (int i = 0; i < numEnemiesAlive; i++)
        {
            enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
            enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
            enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;
            enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);
            if (enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>() != null)
            {
                enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayerTransform(playerCharController.transform);

            }
            else if (enemyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
            {
                //nothing?
            }
            enemiesInWorld[i] = enemyContainer.transform.GetChild(i).gameObject;
        }



        currentHealthPickups = Waves[waveNumber - 1].transform.Find("HealthPickups").gameObject;
        currentPropPickups = Waves[waveNumber - 1].transform.Find("Props").gameObject;
        Waves[waveNumber - 1].gameObject.SetActive(false);

        waveActive = false;

        playerInterface.reinitializeUI(numEnemiesAlive);

    }


    /*public void initAI()
    {



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
                if (aContainer.transform.GetChild(j).GetComponentInChildren<Detect_Movement_AI>() != null)
                {
                    aContainer.transform.GetChild(j).GetComponentInChildren<Detect_Movement_AI>().SetPlayerTransform(playerCharController.transform);

                }
                else if (aContainer.transform.GetChild(j).GetComponentInChildren<NormalMovementAI>() != null)
                {
                    //nothing?
                }
            }


        }

        for (int i = 1; i < Waves.Length; i++)
        {
            Waves[i].gameObject.SetActive(false);
        }

    }*/

}
