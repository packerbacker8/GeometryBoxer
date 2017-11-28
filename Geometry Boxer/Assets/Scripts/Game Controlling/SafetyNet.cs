using UnityEngine;
using System.Timers;
using System.Threading.Tasks;

public class SafetyNet : MonoBehaviour
{
    [Header("Player Character Options")]
    public GameObject[] playerOptions;

    private GameObject resetLocation;
    private GameObject mainPlayer;
    private GameObject activePlayer;
    private GameObject enemyContainer;
    private GameObject[] enemies;
    private bool sceneHasEnemies;

    private float positiveWorldBoundaries;
    private float negativeWorldBoundaries;

    //private Timer theTimer;
    //check time is how many seconds to wait in milliseconds
    private int checkTime;
    private float timeElapsed;
    // Use this for initialization
    void Start()
    {
        resetLocation = GameObject.FindGameObjectWithTag("Respawn");
        positiveWorldBoundaries = 1000f;
        negativeWorldBoundaries = -1000f;
        for (int i = 0; i < playerOptions.Length; i++)
        {
            if (playerOptions[i].name.Contains(SaveAndLoadGame.saver.GetCharacterType()))
            {
                playerOptions[i].SetActive(true);
                mainPlayer = playerOptions[i];
                activePlayer = playerOptions[i].transform.GetChild(2).gameObject;
            }
            else
            {
                playerOptions[i].SetActive(false);
            }
        }
        enemyContainer = GameObject.FindGameObjectWithTag("EnemyContainer");
        enemies = new GameObject[enemyContainer.transform.childCount];
        if(enemyContainer == null)
        {
            sceneHasEnemies = false;
        }
        else
        {
            sceneHasEnemies = true;
            for (int i = 0; i < enemyContainer.transform.childCount; i++)
            {
                enemies[i] = enemyContainer.transform.GetChild(i).gameObject;
            }
        }

        checkTime = 5;
        timeElapsed = 0;
        //theTimer = new Timer(checkTime);
        //theTimer.Elapsed += new ElapsedEventHandler(HandleSafteyNetCatch);
        //theTimer.AutoReset = true;
        //theTimer.Start();
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > checkTime)
        {
            HandleSafteyNetCatch();
            timeElapsed = 0;
        }
    }

    public void NewSceneIsLoading()
    {
        //theTimer.Stop();
        //theTimer.Dispose();
    }

    private void OnApplicationQuit()
    {
        //if(theTimer != null)
        //{
        //   theTimer.Stop();
        //    theTimer.Dispose();
        //}
    }

    // Update is called once per frame
    private void HandleSafteyNetCatch()
    {
        {
            if (sceneHasEnemies)
            {
                foreach (GameObject enemy in enemies)
                {
                    //check if any axis position is outside the bounds of the world in the positive
                    //and negative direction.
                    if (enemy.transform.GetChild(2).position.x > positiveWorldBoundaries || enemy.transform.GetChild(2).position.y > positiveWorldBoundaries || enemy.transform.GetChild(2).position.z > positiveWorldBoundaries
                        || enemy.transform.GetChild(2).position.x < negativeWorldBoundaries || enemy.transform.GetChild(2).position.y < negativeWorldBoundaries || enemy.transform.GetChild(2).position.z < negativeWorldBoundaries)
                    {
                        enemy.SendMessage("ResetEnemy", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }

            //check if any axis position is outside the bounds of the world in the positive
            //and negative direction.
            if (activePlayer.transform.position.x > positiveWorldBoundaries || activePlayer.transform.position.y > positiveWorldBoundaries
                || activePlayer.transform.position.z > positiveWorldBoundaries || activePlayer.transform.position.x < negativeWorldBoundaries
                || activePlayer.transform.position.y < negativeWorldBoundaries || activePlayer.transform.position.z < negativeWorldBoundaries)
            {
                //For some reason this isn't working properly so we just reload the scene
                mainPlayer.SendMessage("PlayerBeingReset", resetLocation.transform, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
