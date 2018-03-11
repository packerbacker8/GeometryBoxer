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
        }
        else
        {
            enemyContainer = Waves[0].transform.Find("EnemiesCube").gameObject;
            Waves[0].transform.Find("EnemiesOctahedron").gameObject.SetActive(false);
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

        enemyContainer.SetActive(false);

    }

    

    protected override void Start()
    {
        base.Start();

        playerInterface = GameObject.FindGameObjectWithTag("playerUI").GetComponent<PlayerUserInterface>();

    }

    protected override void Update()
    {
        base.Update();
    }

}
