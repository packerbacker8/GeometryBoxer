using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class userInterface : MonoBehaviour {
    public Text enemyCounter;
    public Text PlayerSpecialTimer;
    public float coolDownTime;

    private GameObject player;
    private GameObject enemies;
    private GameControllerScript gameController;
    private int numEnemiesAlive;
    private bool usingSpecialAttack = false;
    private float playerCoolDownTimer;

    // Use this for initialization
    void Start () {
        enemies = GameObject.FindGameObjectWithTag("EnemyContainer");
        player = GameObject.FindGameObjectWithTag("Player");
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        numEnemiesAlive = enemies.transform.childCount;
        enemyCounter.text = gameController.NumberOfEnemiesAlive().ToString();
        playerCoolDownTimer = coolDownTime;

        PlayerSpecialTimer.text = playerCoolDownTimer.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        enemyCounter.text = gameController.NumberOfEnemiesAlive().ToString();

        if (usingSpecialAttack)
        {
            coolDownTime -= Time.deltaTime;

            if (coolDownTime <= 0)
            {
                usingSpecialAttack = false;
            }
        }

        PlayerSpecialTimer.text = coolDownTime.ToString("n2");
    }

    public void UsedSpecialAttack()
    {
        usingSpecialAttack = true;
    }

    public void SetCoolDownTime(float time)
    {
        coolDownTime = time;
    }
}
