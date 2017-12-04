using RootMotion.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpecialStats : PlayerStatsBaseClass
{
    public GameObject pelvisJoint;
    public float Health = 15000;
    public float Speed = .7f;
    public float Stability;
    public float AttackForce;
    public float FallDamageMultiplier;
    public float PowerUpTimeLimit = 10;

    private float HealthModifier;
    private Rigidbody playerRigidBody;
    private PlayerHealthScript HealthScript;
    private bool PowerUp = false;
    private float TimePowerUp;
    private Behaviour halo;
    private bool isGrounded;
    private CharacterMeleeDemo charMelDemo;

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
        charMelDemo = this.transform.GetComponentInChildren<CharacterMeleeDemo>();
        isGrounded = charMelDemo.animState.onGround;
        playerRigidBody = pelvisJoint.GetComponent<Rigidbody>();

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
        HealthScript.setCubeHealthModifier(HealthModifier);
        if (playerRigidBody.velocity.magnitude < 1)
        {
            attackForce += 1;
            stability += 1;
            FallDamageMultiplier += 1;
            HealthModifier += 1;
        }
        else
        {
            attackForce = 1;
            stability = 1;
            FallDamageMultiplier = 1;
            HealthModifier = 1;
        }

        if (PowerUp == true)
        {
            attackForce = 100;
            userControl.state.move *= 0.5f;
            stability = 100f;
            ApplyStabilityStat();
            HealthScript.setCubeHealthModifier(500);
        }
        else
        {
            attackForce = 1;
            stability = 1f;
            ApplyStabilityStat();
            userControl.state.move *= 1f;
            HealthScript.setCubeHealthModifier(1);
        }


    }

    public void PowerUpActive(bool active)
    {
        PowerUp = active;
    }

    public void PowerUpDeactivated(bool deActivate)
    {
        PowerUp = deActivate;
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
                HealthScript.setCubeHealthModifier(500); // when we change the health script you will need to fix this

            }
            else
            {
                HealthScript.setCubeHealthModifier(HealthModifier);
            }

        }

    }
}
