using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNet : MonoBehaviour
{
    [Header("Player Character Options")]
    public GameObject[] playerOptions;

    private GameObject activePlayer;
    private GameObject enemyContainer;
    private bool sceneHasEnemies;
    private float positiveWorldBoundaries;
    // Use this for initialization
    void Start()
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
        enemyContainer = GameObject.FindGameObjectWithTag("EnemyContainer");
        if(enemyContainer == null)
        {
            sceneHasEnemies = false;
        }
        else
        {
            sceneHasEnemies = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneHasEnemies)
        {

        }
        else
        {

        }
    }
}
