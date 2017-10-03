using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [Tooltip("What index value of the level to load in the build order.")]
    public int streetScene = 2;
    public float loadLevelTimeOut = 20f;

    private int numEnemiesAlive;
    private GameObject[] enemiesInWorld;
    private bool playerAlive;

    // Use this for initialization
    void Start()
    {
        enemiesInWorld = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            enemiesInWorld[i].GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
        }
        numEnemiesAlive = enemiesInWorld.Length;
        playerAlive = true;
        //Debug.Log("numEnemiesAlive: " + numEnemiesAlive);
    }

    // Update is called once per frame
    void Update()
    {
        if(numEnemiesAlive <= 0)
        {
            StartCoroutine(changeLevel(streetScene));
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
    IEnumerator changeLevel(int levelToLoad)
    {
        float fadeTime = GetComponent<Fade>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
    }

    /// <summary>
    /// Tells the game controller the player died.
    /// </summary>
    public void playerKilled()
    {
        playerAlive = false;
        StartCoroutine(changeLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)); //index of the scene the player is currently on
    }
}
