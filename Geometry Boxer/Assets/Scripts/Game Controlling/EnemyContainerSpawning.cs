using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainerSpawning : MonoBehaviour
{
    [Tooltip("Which sort of enemy do you want to spawn?")]
    public GameObject enemyTypeToSpawn;
    [Tooltip("How many enemies do you want in this container?")]
    public int numberOfEnemies;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject.Instantiate(enemyTypeToSpawn, this.transform, true);
        }
    }
}
