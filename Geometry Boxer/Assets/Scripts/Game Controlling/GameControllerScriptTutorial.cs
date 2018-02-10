using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;


public class GameControllerScriptTutorial : MonoBehaviour
{

    [Tooltip("What string value of the level to load in the build order.")]
    public string mainMenu = "MainMenu";
    public float loadLevelTimeOut = 20f;
    public GameObject[] playerOptions;
    public List<GameObject> enemiesInWorld;

    private string currentMapName;
    private GameObject activePlayer;
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
}
