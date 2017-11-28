using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeStats : PlayerStatsBaseClass
{
    public float Health = 15000;
    public float Speed = .7f;
    public float Stability;
    public float AttackForce;
    public float FallDamageMultiplier;

    public float PowerUpTimeLimit = 10;

    private PlayerHealthScript HealthScript;
    private bool PowerUp = false;
    private float TimePowerUp;
    private Behaviour halo;

    // This is puppetMasters user controler, it controls the players movements
    protected UserControlThirdPerson userControl; // user input


    // Use this for initialization
    void Start()
    {
        HealthScript = this.GetComponent<PlayerHealthScript>();
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        TimePowerUp = PowerUpTimeLimit;
        halo = (Behaviour)charController.GetComponent("Halo");
        userControl = charController.GetComponent<UserControlThirdPerson>();

        health = Health;
        stability = Stability;
        speed = Speed;
        attackForce = AttackForce;
        fallDamageMultiplier = FallDamageMultiplier;

        HealthScript.PlayerHealth = GetPlayerHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && PowerUp == false)
        {
            PowerUp = true;
            halo.enabled = true;
            SetPlayerSpeed(.5f);
            SetPlayerAttackForce(2);
            SetPlayerStability(2);
            SetPlayerFallMultiplier(2);
            userControl.state.move *= 0.5f;
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
                HealthScript.setCubeHealthModifier(500);

            }
            else
            {
                HealthScript.setCubeHealthModifier(1);
            }

        }

    }
}
