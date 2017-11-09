using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [Tooltip("What index value of the level to load in the build order.")]
    public int dominationMap = 5;
    public float loadLevelTimeOut = 20f;

    private int numEnemiesAlive;
    private GameObject[] enemiesInWorld;
    private GameObject enemyContainer;
    private bool playerAlive;
    private LoadLevel loader;

    // Use this for initialization
    void Start()
    {
        enemyContainer= GameObject.FindGameObjectWithTag("EnemyContainer");
        numEnemiesAlive = enemyContainer.transform.childCount;
        for (int i = 0; i < numEnemiesAlive; i++)
        {
            enemyContainer.transform.GetChild(i).GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
        }
        playerAlive = true;
        loader = this.GetComponent<LoadLevel>();
        //Debug.Log("numEnemiesAlive: " + numEnemiesAlive);
    }

    // Update is called once per frame
    void Update()
    {
        if(numEnemiesAlive <= 0)
        {
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
    IEnumerator changeLevel(int levelToLoad)
    {
        float fadeTime = GetComponent<Fade>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        loader.LoadALevel(levelToLoad);
    }

    /// <summary>
    /// Tells the game controller the player died.
    /// </summary>
    public void playerKilled()
    {
        playerAlive = false;
        loader.ReloadScene(); //index of the scene the player is currently on
    }
}
