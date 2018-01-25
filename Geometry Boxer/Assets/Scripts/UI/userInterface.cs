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
    private int numEnemiesAlive;
    private bool usingSpecialAttack = false;
    private float playerCoolDownTimer;

    // Use this for initialization
    void Start () {
        enemies = GameObject.FindGameObjectWithTag("EnemyContainer");
        player = GameObject.FindGameObjectWithTag("Player");
        numEnemiesAlive = enemies.transform.childCount;
        enemyCounter.text = numEnemiesAlive.ToString();
        playerCoolDownTimer = coolDownTime;

        PlayerSpecialTimer.text = playerCoolDownTimer.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        enemyCounter.text = numEnemiesAlive.ToString();

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

    public void enemyIsKilled()
    {
        numEnemiesAlive--;
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
