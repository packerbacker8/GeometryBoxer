using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [Tooltip("What index value of the level to load in the build order.")]
    public string dominationMap = "CitySelectMap";
    public float loadLevelTimeOut = 20f;
    public GameObject[] playerOptions;

    private string currentMapName;
    private int numEnemiesAlive;
    private GameObject[] enemiesInWorld;
    private GameObject enemyContainer;
    private bool playerAlive;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < playerOptions.Length; i++)
        {
            playerOptions[i].SetActive(playerOptions[i].name.Contains(SaveAndLoadGame.saver.GetCharacterType()));
        }
        enemyContainer = GameObject.FindGameObjectWithTag("EnemyContainer");
        numEnemiesAlive = enemyContainer.transform.childCount;
        for (int i = 0; i < numEnemiesAlive; i++)
        {
            enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
        }
        playerAlive = true;
        currentMapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
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
        SaveAndLoadGame.saver.SetCityStatus(currentMapName, "notconquered");
        LoadLevel.loader.LoadALevel(dominationMap); //index of the scene the player is currently on
    }
}
