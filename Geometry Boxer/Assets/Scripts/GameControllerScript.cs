using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    public int streetScene = 2;
    public float loadLevelTimeOut = 20f;

    private int numEnemiesAlive;
    private GameObject[] enemiesInWorld;
    // Use this for initialization
    void Start()
    {
        enemiesInWorld = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemiesInWorld.Length; i++)
        {
            enemiesInWorld[i].GetComponent<EnemyHealthScript>().SetEnemyIndex(i);
        }
        numEnemiesAlive = enemiesInWorld.Length;

        Debug.Log("numEnemiesAlive: " + numEnemiesAlive);
    }

    // Update is called once per frame
    void Update()
    {
        if(numEnemiesAlive <= 0)
        {
            StartCoroutine(ChangeLevel());
        }
    }

    public void isKilled(int index)
    {
        numEnemiesAlive--;
        Debug.Log("numEnemiesAlive: " + numEnemiesAlive);
    }

    IEnumerator ChangeLevel()
    {
        float fadeTime = GetComponent<Fade>().BeginFade(1);
        Debug.Log("FadeTime: " + fadeTime);
        yield return new WaitForSeconds(fadeTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene(streetScene);
    }
}
