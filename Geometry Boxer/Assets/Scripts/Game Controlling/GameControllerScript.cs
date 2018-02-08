using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Enemy;

public class GameControllerScript : MonoBehaviour
{
    [Tooltip("What string value of the level to load in the build order.")]
    public string dominationMap = "CitySelectMap";
    public float loadLevelTimeOut = 20f;
    public GameObject[] playerOptions;
    public GameObject enemyCubeContainer;
    public GameObject enemyOctahedronContainer;
    [Tooltip("If set to true, the enemy container that matches the type of the player will be used as a group of allies in the fight.")]
    public bool hasAllies = false;

    private string currentMapName;
    private int numEnemiesAlive;
    private int charControllerIndex;
    private GameObject activePlayer;
    private GameObject enemyContainer;
    private GameObject[] enemiesInWorld;
    private GameObject allyContainer;
    private GameObject[] alliesInWorld;
    private GameObject playerCharController;
    private bool playerAlive;

    // Use this for initialization
    void Awake()
    {
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
        charControllerIndex = 2;
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
            enemiesInWorld = new GameObject[numEnemiesAlive];
            alliesInWorld = new GameObject[allyContainer.transform.childCount];
            int iterationAmount = alliesInWorld.Length > numEnemiesAlive ? alliesInWorld.Length : numEnemiesAlive;

            for (int i = 0; i < iterationAmount; i++)
            {
                if(i < numEnemiesAlive)
                {
                    enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
                    enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
                    enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(allyContainer.transform.GetChild(i % alliesInWorld.Length).GetChild(charControllerIndex).gameObject);
                    if(enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>() != null)
                    {
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayerTransform(playerCharController.transform);
                    }
                    else if(enemyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
                    {
                        enemyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);
                    }
                    enemiesInWorld[i] = enemyContainer.transform.GetChild(i).gameObject;
                }
                if(i < alliesInWorld.Length)
                {
                    allyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
                    allyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Enemy", false);
                    allyContainer.transform.GetChild(i).GetComponentInChildren<UserControlAI>().SetMoveTarget(enemyContainer.transform.GetChild(i % numEnemiesAlive).GetChild(charControllerIndex).gameObject);
                    if(allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>() != null)
                    {
                        allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().SetPlayerTransform(playerCharController.transform); //might need to change
                        allyContainer.transform.GetChild(i).GetComponentInChildren<Detect_Movement_AI>().sightRange = 0f; //set sight range to zero so that allied bots never try to switch targets

                    }
                    else if (allyContainer.transform.GetChild(i).GetComponentInChildren<NormalMovementAI>() != null)
                    {
                        //do nothing?
                    }
                    ChangeAllTags(allyContainer.transform.GetChild(i).gameObject, "Ally");
                    allyContainer.transform.GetChild(i).gameObject.tag = "AllyRoot";
                    alliesInWorld[i] = allyContainer.transform.GetChild(i).gameObject;
                }
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
            enemiesInWorld = new GameObject[numEnemiesAlive];
            for (int i = 0; i < numEnemiesAlive; i++)
            {
                enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
                enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetDamageSource("Player", true);
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
        }

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
            SaveAndLoadGame.saver.SetCityStatus(currentMapName, "conquered");
            StartCoroutine(changeLevel(dominationMap));
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
    public void isKilled(int index, string tag)
    {
        if(hasAllies)
        {
            if (tag.Contains("Enemy"))
            {
                numEnemiesAlive--;
                /*
                bool[] alliesToChange = new bool[alliesInWorld.Length];
                for (int i = 0; i < alliesInWorld.Length; i++)
                {
                    if ( alliesInWorld[i] != null && alliesInWorld[i].GetComponentInChildren<UserControlAI>().moveTargetObj.transform.parent.gameObject == enemiesInWorld[index])
                    {
                        alliesToChange[i] = true;
                    }
                }*/
                enemiesInWorld[index] = null;
                //SetNewTarget(i, alliesInWorld[i].tag);
            }
            else
            {
                /*
                for (int i = 0; i < enemiesInWorld.Length; i++)
                {
                    if (alliesInWorld[index] != null && enemiesInWorld[i] != null && enemiesInWorld[i].GetComponentInChildren<UserControlAI>().moveTargetObj.transform.parent.gameObject == alliesInWorld[index])
                    {
                        SetNewTarget(i, enemiesInWorld[i].tag);

                    }
                }*/
                alliesInWorld[index] = null;
            }
        }
        else
        {
            numEnemiesAlive--;
            enemiesInWorld[index] = null;
        }
        
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
    public void playerKilled()
    {
        playerAlive = false;
        SaveAndLoadGame.saver.SetCityStatus(currentMapName, "notconquered");
        LoadLevel.loader.LoadALevel(dominationMap); //index of the scene the player is currently on
    }

    public GameObject GetActivePlayer()
    {
        return activePlayer;
    }

    /// <summary>
    /// Function to change the target of the enemy to the player.
    /// </summary>
    /// <param name="indexOfEnemy">This index indicates which enemy to change their target.</param>
    public void ChangeTarget(int indexOfEnemy)
    {
        enemiesInWorld[indexOfEnemy].GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);
    }

    /// <summary>
    /// Function to set new target for bots to attack.
    /// </summary>
    /// <param name="index">The bot index in their respective array.</param>
    /// <param name="tag">Tag of their root object</param>
    public void SetNewTarget(int index, string tag)
    {
        if(tag.Contains("Enemy"))
        {
            for(int i = 0; i < alliesInWorld.Length; i++)
            {
                if(alliesInWorld[i] != null)
                {
                    enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(alliesInWorld[i].transform.GetChild(charControllerIndex).gameObject);
                    return;
                }
            }
            enemiesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(playerCharController);
        }
        else
        {
            for (int i = 0; i < enemiesInWorld.Length; i++)
            {
                if (enemiesInWorld[i] != null)
                {
                    alliesInWorld[index].GetComponentInChildren<UserControlAI>().SetMoveTarget(enemiesInWorld[i].transform.GetChild(charControllerIndex).gameObject);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Function to change the allied bot's target after their original target has died.
    /// </summary>
    /// <param name="indexOfAlly">Index indicates which ally to change their target for.</param>
    private void SwitchAlliedTarget(int indexOfAlly)
    {

    }

    /// <summary>
    /// How many enemies are alive in the scene.
    /// </summary>
    /// <returns></returns>
    public int NumberOfEnemiesAlive()
    {
        return numEnemiesAlive;
    }
}
