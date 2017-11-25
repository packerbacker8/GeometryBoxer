using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeStats : MonoBehaviour
{
    public PlayerHealthScript HealthScript;
    public float Health;
    public float PowerUpTimeLimit = 10;

    private GameObject puppetMast;
    private GameObject gameController;
    private GameObject CharacterController;
    private Animator anim;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";

    private PlayerStatsBaseClass PlayerStats;
    private bool PowerUp = false;
    private float TimePowerUp;
    private Behaviour halo;


    // Use this for initialization
    void Start()
    {
        PlayerStats = new PlayerStatsBaseClass(Health, 1f, 1f, 1f, 1f);
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        CharacterController = this.transform.GetChild(characterControllerIndex).gameObject;
        TimePowerUp = PowerUpTimeLimit;
        halo = (Behaviour)CharacterController.GetComponent("Halo");

        HealthScript.PlayerHealth = PlayerStats.GetPlayerHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && PowerUp == false)
        {
            PowerUp = true;
            halo.enabled = true;
        }

        if (PowerUp)
        {
            TimePowerUp -= 1 * Time.deltaTime;
            if (TimePowerUp <= 0)
            {
                PowerUp = false;
                halo.enabled = false;
                TimePowerUp = PowerUpTimeLimit;
            }
        }

    }

    /// <summary>
    /// Function receives impulse received by colliders on the enemy characters.
    /// </summary>
    /// <param name="impulseVal"></param>
    public void ImpactReceived(Collision collision)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (collision.gameObject.tag == "EnemyCollision" || (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            if (PowerUp == true)
            {
                HealthScript.cubeHealthModifier = 500;

            }
            else
            {
                HealthScript.cubeHealthModifier = 1;
            }

        }

    }
}
