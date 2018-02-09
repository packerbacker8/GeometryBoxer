using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class userInterface : MonoBehaviour
{
    public Text enemyCounter;
    public Text PlayerSpecialTimer;

    private float coolDownTime;
    private float playerCoolDownTimer;
    private GameObject player;
    private GameObject enemies;
    private GameControllerScript gameController;
    private int numEnemiesAlive;
    private bool usingSpecialAttack = false;
    private CubeAttackScript cubePunchScript;
    private OctahedronSpecials octahedronPunchScript;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        enemies = gameController.GetEnemyContainer();
        player = gameController.GetActivePlayer();
        if(player.name.Contains("Cube"))
        {
            cubePunchScript = player.GetComponent<CubeAttackScript>();
            octahedronPunchScript = null;
        }
        else
        {
            octahedronPunchScript = player.GetComponent<OctahedronSpecials>();
            cubePunchScript = null;
        }
        numEnemiesAlive = enemies.transform.childCount;
        enemyCounter.text = gameController.NumberOfEnemiesAlive().ToString();
        playerCoolDownTimer = coolDownTime;
        PlayerSpecialTimer.text = playerCoolDownTimer.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCounter.text = "Enemies Remaining: " + gameController.NumberOfEnemiesAlive().ToString();

        if (cubePunchScript != null)
        {
            if(cubePunchScript.GetOnCooldown())
            {
                coolDownTime -= Time.deltaTime;
                PlayerSpecialTimer.text = "Cooling down: " + coolDownTime.ToString("n2");
                if (coolDownTime <= 0)
                {
                    usingSpecialAttack = false;
                }
            }
            else
            {
                if(cubePunchScript.GetSpecialActivated())
                {
                    PlayerSpecialTimer.text = "Cube Stomp Activated!";
                }
                else
                {
                    coolDownTime = playerCoolDownTimer;
                    PlayerSpecialTimer.text = "Cube Stomp Ready!";
                }
            }
        }
        else if(octahedronPunchScript != null)
        {
            if(octahedronPunchScript.GetOnCooldown())
            {
                coolDownTime -= Time.deltaTime;
                PlayerSpecialTimer.text = "Cooling down: " + coolDownTime.ToString("n2");
                if (coolDownTime <= 0)
                {
                    usingSpecialAttack = false;
                }
            }
            else
            {
                if(octahedronPunchScript.GetSpecialActivated())
                {
                    PlayerSpecialTimer.text = "Octahedron Tornado Activated!";
                }
                else
                {
                    coolDownTime = playerCoolDownTimer;
                    PlayerSpecialTimer.text = "Octahedron Tornado Ready!";
                }
            }
        }

        
    }

    public void UsedSpecialAttack()
    {
        usingSpecialAttack = true;
    }

    public void SetCoolDownTime(float time)
    {
        coolDownTime = time;
        playerCoolDownTimer = time;
    }
}
