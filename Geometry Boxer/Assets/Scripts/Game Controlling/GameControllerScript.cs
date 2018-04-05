﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;

public class GameControllerScript : MonoBehaviour
{
    [Tooltip("What string value of the level to load in the build order.")]
    public string dominationMap = "CitySelectMap";
    public string deathReloadMap;
    public float loadLevelTimeOut = 20f;
    public GameObject[] playerOptions;
    public GameObject enemyCubeContainer;
    public GameObject enemyOctahedronContainer;
    public GameObject SafeSpot;
    [Header("Ally Information")]
    [Tooltip("If set to true, two of the player characters will be active with the camera being half and half.")]
    public bool IsSplitScreen = false;
    [Tooltip("If set to true, the enemy container that matches the type of the player will be used as a group of allies in the fight.")]
    public bool hasAllies = false;
    [Tooltip("This integer will divide the number of bots in the other container that will be the allies. The higher the number, the lower the number of allies.")]
    [Range(1, 100)]
    public int fractionalAllies = 3;
    [Tooltip("Percentage of number of enemies left to make the sight line increase by.")]
    [Range(0, 0.99f)]
    public float sightLineExpansionThreshold = 0.5f;
    [Tooltip("How much to expand sight bubble's radius by.")]
    public float sightExpansionAmount = 2f;
    

    protected string currentMapName;
    protected int numEnemiesAlive;
    protected int oldExpansionNumEnemies;
    protected int charControllerIndex;
    protected GameObject activePlayer;
    protected GameObject player2;
    protected GameObject enemyContainer;
    protected GameObject[] enemiesInWorld;
    protected GameObject allyContainer;
    protected GameObject[] alliesInWorld;
    protected GameObject playerCharController;
    protected GameObject player2CharController;
    protected Queue<GameObject> enemyTargetQueue;
    protected Queue<GameObject> allyTargetQueue;
    protected GameObject pauseMenu;
    protected GameObject deathMenuObj;
    protected GameObject winMenuObj;
    protected GameObject playerUI;
    protected GameObject player2UI;
    protected bool playerAlive;
    protected bool player2Alive;
    protected bool levelWon;
    protected bool switchPlayers;


    // Use this for initialization
    protected virtual void Awake()
    {
        switchPlayers = false;
        for (int i = 0; i < playerOptions.Length; i++)
        {
            if(playerOptions[i].name.Contains(SaveAndLoadGame.saver.GetCharacterType()))
            {
                playerOptions[i].SetActive(true);
                activePlayer = playerOptions[i];
            }
            else
            {
                playerOptions[i].SetActive(false);
            }
        }

        this.GetComponent<SplitscreenOrientation>().player1Cam = activePlayer.GetComponentInChildren<Camera>();
        playerUI = GameObject.FindGameObjectWithTag("playerUI");
        playerUI.GetComponent<PlayerUI.PlayerUserInterface>().SetPlayer(activePlayer);
        activePlayer.GetComponent<PlayerStatsBaseClass>().SetIfPlayer2(false);
        IsSplitScreen = SaveAndLoadGame.saver.GetSplitscreen();
        charControllerIndex = 2;
        player2CharController = null;
        player2Alive = false;
        player2 = null;
        if (IsSplitScreen)
        {
            player2 = Instantiate(activePlayer);
            player2.transform.position = new Vector3(player2.transform.position.x + 5f, player2.transform.position.y, player2.transform.position.z);
            this.GetComponent<SplitscreenOrientation>().player2Cam = player2.GetComponentInChildren<Camera>();
            player2.GetComponent<PunchScript>().SetAsPlayer2();
            player2.GetComponentInChildren<RootMotion.CameraController>().SetIsPlayer2();
            player2.GetComponentInChildren<UserControlThirdPerson>().SetIsPlayer2();
            player2CharController = player2.transform.GetChild(charControllerIndex).gameObject;
            activePlayer.GetComponent<PunchScript>().Player2Present = true;
            player2UI = Instantiate(playerUI);
            player2UI.tag += "_2";
            player2UI.GetComponent<PlayerUI.PlayerUserInterface>().SetPlayer(player2);
            player2.GetComponent<PlayerStatsBaseClass>().SetIfPlayer2(true);
            player2Alive = true;
        }

        playerCharController = activePlayer.transform.GetChild(charControllerIndex).gameObject;
        enemyContainer = activePlayer.name.Contains("Cube") ? enemyOctahedronContainer : enemyCubeContainer;
        //if we have allies the setup has to include their targets and health along with some tag changes
        if (hasAllies)
        {
            enemyCubeContainer.SetActive(true);
            enemyOctahedronContainer.SetActive(true);
            if (enemyContainer == enemyOctahedronContainer)
            {
                allyContainer = enemyCubeContainer;
            }
            else
            {
                allyContainer = enemyOctahedronContainer;
            }
            allyContainer.tag = "AllyContainer"; 
            numEnemiesAlive = enemyContainer.transform.childCount;
            oldExpansionNumEnemies = numEnemiesAlive;
            enemiesInWorld = new GameObject[numEnemiesAlive];
            alliesInWorld = new GameObject[allyContainer.transform.childCount / fractionalAllies == 0 ? 1 : allyContainer.transform.childCount / fractionalAllies];
            enemyTargetQueue = new Queue<GameObject>();
            allyTargetQueue = new Queue<GameObject>();
            int iterationAmount = alliesInWorld.Length > numEnemiesAlive ? alliesInWorld.Length : numEnemiesAlive;

            for (int i = 0; i < iterationAmount; i++)
            {
                if(i < numEnemiesAlive)
                {
                    enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
                    enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
                    enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(allyContainer.transform.GetChild(i % alliesInWorld.Length).GetChild(charControllerIndex).gameObject);
                    enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().safeSpot = SafeSpot;
                    if(enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>() != null)
                    {
                        if(player2CharController == null)
                        {
                            enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, null);
                        }
                        else
                        {
                            enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, player2CharController.transform);
                        }
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(0, true);
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(1, IsSplitScreen);
                    }
                    else if(enemyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
                    {
                        switchPlayers = player2CharController == null ? false : !switchPlayers;
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                    }
                    enemiesInWorld[i] = enemyContainer.transform.GetChild(i).gameObject;
                    allyTargetQueue.Enqueue(enemiesInWorld[i]);
                }
                if(i < alliesInWorld.Length)
                {
                    allyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
                    allyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Enemy", false);
                    allyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(enemyContainer.transform.GetChild(i % numEnemiesAlive).GetChild(charControllerIndex).gameObject);
                    if(allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>() != null)
                    {
                        if(player2CharController == null)
                        {
                            allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, null); //might need to change
                        }
                        else
                        {
                            allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, player2CharController.transform); //might need to change
                        }
                        allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().sightRange = 0f; //set sight range to zero so that allied bots never try to switch targets

                    }
                    else if (allyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
                    {
                        //do nothing?
                    }
                    ChangeAllTags(allyContainer.transform.GetChild(i).gameObject, "Ally");
                    allyContainer.transform.GetChild(i).gameObject.tag = "AllyRoot";
                    alliesInWorld[i] = allyContainer.transform.GetChild(i).gameObject;
                    enemyTargetQueue.Enqueue(alliesInWorld[i]);
                }
            }
            //Turn off the remaining allies.
            for(int i = alliesInWorld.Length; i < allyContainer.transform.childCount; i++)
            {
                allyContainer.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            if (enemyContainer == enemyOctahedronContainer)
            {
                enemyCubeContainer.SetActive(false);
            }
            else
            {
                enemyOctahedronContainer.SetActive(false);
            }

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
                    if(player2CharController == null)
                    {
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, null);
                    }
                    else
                    {
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayersTransform(playerCharController.transform, player2CharController.transform);
                    }
                    enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(0, true);
                    enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(1, IsSplitScreen);
                }
                else if (enemyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
                {
                    switchPlayers = player2CharController == null ? false : !switchPlayers;
                    enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(switchPlayers ? player2CharController : playerCharController);
                }
                enemiesInWorld[i] = enemyContainer.transform.GetChild(i).gameObject;
            }
        }
        levelWon = false;
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        //Debug.Log(pauseMenu.GetComponentInChildren<DeathMenu>().ToString());
        deathMenuObj = pauseMenu.GetComponentInChildren<DeathMenu>().gameObject;
        deathMenuObj.SetActive(false);
        winMenuObj = pauseMenu.GetComponentInChildren<WinMenu>().gameObject;
        winMenuObj.SetActive(false);

        playerAlive = true;
        currentMapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        this.GetComponent<SafetyNet>().SetPlayer2(player2);
    }

    protected virtual void Start()
    {
        //Here is where when loading in a game file the data is updated to destroy the enemies that were already killed.
        if (SaveAndLoadGame.saver.GetLoadedFightScene())
        {
            HashSet<int> enemyI = SaveAndLoadGame.saver.GetFightSceneEnemyIndicies();
            for(int i = 0; i < enemiesInWorld.Length; i++)
            {
                if(!enemyI.Contains(enemiesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex()))
                {
                    enemiesInWorld[i].GetComponent<EnemyHealthScript>().KillEnemy();
                }
            }
            if(hasAllies && SaveAndLoadGame.saver.GetFightSceneHasAllies())
            {
                HashSet<int> allyI = SaveAndLoadGame.saver.GetFightSceneAllyIndicies();
                for (int i = 0; i < alliesInWorld.Length; i++)
                {
                    if (!allyI.Contains(alliesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex()))
                    {
                        alliesInWorld[i].GetComponent<EnemyHealthScript>().KillEnemy();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (numEnemiesAlive <= oldExpansionNumEnemies * sightLineExpansionThreshold)
        {
            oldExpansionNumEnemies = numEnemiesAlive;
            StartCoroutine(IncreaseEnemySight(sightExpansionAmount));
        }
        if (numEnemiesAlive <= 0)
        {
            if (!levelWon && (playerAlive || player2Alive))
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


            //StartCoroutine(changeLevel(dominationMap));
        }
    }

    private void ChangeAllTags(GameObject objToChange, string tagToUse)
    {
        objToChange.tag = tagToUse;
        for(int i = 0; i < objToChange.transform.childCount; i++)
        {
            ChangeAllTags(objToChange.transform.GetChild(i).gameObject, tagToUse);
        }
    }

    /// <summary>
    /// Updates how many enemies are alive after one is killed.
    /// </summary>
    /// <param name="index">Index in the respective array.</param>
    /// <param name="tag">Tag of the object sent.</param>
    public virtual void IsKilled(int index, string tag)
    {
        if(hasAllies)
        {
            if (tag.Contains("Enemy"))
            {
                numEnemiesAlive--;
                enemiesInWorld[index] = null;
            }
            else
            {
                alliesInWorld[index] = null;
            }
        }
        else
        {
            numEnemiesAlive--;
            enemiesInWorld[index] = null;
        }
        playerUI.GetComponent<PlayerUI.PlayerUserInterface>().EnemiesLeft(numEnemiesAlive);
        if (IsSplitScreen) player2UI.GetComponent<PlayerUI.PlayerUserInterface>().EnemiesLeft(numEnemiesAlive);
    }

    /// <summary>
    /// Multi-threaded method to change level currently on with a fade 
    /// to black.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeLevel(string levelToLoad)
    {
        float fadeTime = GetComponent<Fade>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        LoadLevel.loader.LoadALevel(levelToLoad);
    }

    /// <summary>
    /// Tells the game controller the player died.
    /// </summary>
    /// <param name="p2">If this is true, the player that died is player 2.</param>
    public virtual void PlayerKilled(bool p2)
    {
        if(p2)
        {
            player2Alive = false;
        }
        else
        {
            playerAlive = false;
        }
        if(!playerAlive && !player2Alive)
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
                if (enemiesInWorld[i] != null && enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>() != null)
                {
                    enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(0, true);
                    enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(1, IsSplitScreen);
                }
            }
        }
        else
        {
            for(int i = 0; i < enemiesInWorld.Length; i++)
            {
                if(enemiesInWorld[i] != null && enemiesInWorld[i].GetComponentInChildren<NormalMovementAI>() != null)
                {
                    ChangeTarget(enemiesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex(), !p2);
                }
                else if(enemiesInWorld[i] != null && enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>() != null)
                {
                    enemiesInWorld[i].GetComponentInChildren<Detect_Movement_AI>().SetIfPlayerIsTargetable(p2 ? 1 : 0, false);
                }
            }
        }

        //LoadLevel.loader.LoadALevel(deathReloadMap); //index of the scene the player is currently on
    }

    /// <summary>
    /// Return the active player in the scene.
    /// </summary>
    /// <returns></returns>
    public GameObject GetActivePlayer()
    {
        return activePlayer;
    }

    /// <summary>
    /// Get the player 2 main game object.
    /// </summary>
    /// <returns>Returns the main game object, or null if no player 2 present.</returns>
    public GameObject GetPlayer2()
    {
        return player2;
    }

    /// <summary>
    /// Function to change the target of the enemy to the player.
    /// </summary>
    /// <param name="indexOfEnemy">This index indicates which enemy to change their target.</param>
    /// <param name="player2">Is this player 2? True if yes.</param>
    public void ChangeTarget(int indexOfEnemy, bool player2)
    {
        if(player2)
        {
            enemiesInWorld[indexOfEnemy].GetComponentInChildren<UserControlAI>().SetMoveTarget(player2CharController);
        }
        else
        {
            enemiesInWorld[indexOfEnemy].GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);
        }
    }

    /// <summary>
    /// Function to set new target for bots to attack.
    /// </summary>
    /// <param name="index">The bot index in their respective array.</param>
    /// <param name="tag">Tag of their root object</param>
    public virtual void SetNewTarget(int index, string tag)
    {
        if(hasAllies)
        {
            if (tag.Contains("Enemy"))
            {
                while(enemyTargetQueue.Count != 0)
                {
                    GameObject target = enemyTargetQueue.Dequeue();
                    if(target != null)
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
                    if (target != null)
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

    /// <summary>
    /// How many enemies are alive in the scene.
    /// </summary>
    /// <returns></returns>
    public int NumberOfEnemiesAlive()
    {
        return numEnemiesAlive;
    }

    /// <summary>
    /// Return active enemy container
    /// </summary>
    /// <returns></returns>
    public GameObject GetEnemyContainer()
    {
        return enemyContainer;
    }


    public virtual void SetTargetHealthPack(int index, GameObject objOfHealth, string tag)
    {
        if(tag.Contains("Ally"))
        {
            if(alliesInWorld[index] != null)
            {
                alliesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(objOfHealth);
            }
        }
        else
        {
            if(enemiesInWorld[index] != null)
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
    protected virtual IEnumerator IncreaseEnemySight(float amount)
    {
        yield return null;
        foreach(GameObject enemy in enemiesInWorld)
        {
            if(enemy != null && enemy.GetComponentInChildren<Detect_Movement_AI>() != null)
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
    public virtual HashSet<int> EnemyAliveIndicies()
    {
        HashSet<int> enemiesAlive = new HashSet<int>();
        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            if(enemiesInWorld[i] != null)
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
    public virtual HashSet<int> AllyAliveIndicies()
    {
        HashSet<int> alliesAlive = new HashSet<int>();
        for (int i = 0; i < alliesInWorld.Length; i++)
        {
            if (alliesInWorld[i] != null)
            {
                alliesAlive.Add(alliesInWorld[i].GetComponent<EnemyHealthScript>().GetEnemyIndex());
            }
        }
        return alliesAlive;
    }
}
