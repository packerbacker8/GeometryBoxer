using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.Dynamics;
using RootMotion.Demos;
using System;
using PlayerUI;

public class CubeSpecialStats : PlayerStatsBaseClass
{
    public float Speed = .7f;
    public float Stability;
    public float AttackForce;
    public float FallDamageMultiplier;
    public float PowerUpTimeLimit = 10f;
    public float specialCooldownTime = 10f;

    private float HealthModifier;
    private float TimePowerUp;
    private float originalHealth;
    private bool PowerUp = false;
    private bool isGrounded;
    private Behaviour halo;
    private CharacterMeleeDemo charMelDemo;
    private Rigidbody playerRigidBody;
    private PlayerHealthScript HealthScript;
    private Image healthBarBackground;
    private Image healthBarFill;

    // This is puppetMasters user controler, it controls the players movements
    protected UserControlThirdPerson userControl; // user input

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //HealthScript = this.GetComponent<PlayerHealthScript>();
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

        HealthModifier = 1.0f;

        if (SaveAndLoadGame.saver.GetLoadedFightScene())
        {
            health = isPlayer2 ? SaveAndLoadGame.saver.GetPlayer2CurrentHealth() : SaveAndLoadGame.saver.GetPlayerCurrentHealth();
        }
        else
        {
            health = Health;
        }

        stability = Stability;
        speed = Speed;
        attackForce = AttackForce;
        fallDamageMultiplier = FallDamageMultiplier;
        int healthbarBackgroundIndex = 0;
        int healthbarFillIndex = 0;
        healthBarBackground = playerUI.transform.GetChild(healthbarBackgroundIndex).GetComponent<Image>();
        healthBarFill = healthBarBackground.transform.GetChild(healthbarFillIndex).GetComponent<Image>();
        originalHealth = Health;
        //HealthScript.PlayerHealth = GetPlayerHealth();
        playerUI.GetComponent<PlayerUserInterface>().SetMaxHealth(originalHealth);
        playerUI.GetComponent<PlayerUserInterface>().SetHealth(health);
        UpdateHealthUI();
        playerUI.GetComponent<PlayerUserInterface>().SetPlayerType(1);
    }

    // Update is called once per frame
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (health <= 0f && !dead)
        {
            dead = true;
            KillPlayer();
        }
        //HealthScript.setCubeHealthModifier(HealthModifier);
        /*
        if (PowerUp == true)
        {
            attackForce = 100;
            userControl.state.move *= 0.5f;
            stability = 100f;
            ApplyStabilityStat();
            HealthModifier = 500.0f;
        }
        else if (playerRigidBody.velocity.magnitude < 1)
        {
            attackForce += 1;
            stability += 1;
            FallDamageMultiplier -= 1f;
            if (FallDamageMultiplier <= 0)
            {
                FallDamageMultiplier = 0f;
            }
            HealthModifier += 1.0f;
        }
        else
        {
            attackForce = 1;
            stability = 1f;
            ApplyStabilityStat();
            userControl.state.move *= 1f;
            FallDamageMultiplier += 1f;
            if (FallDamageMultiplier >= 1000f)
            {
                FallDamageMultiplier = 1000f;
            }
            HealthModifier = 1.0f;

        }
        */
    }
    
    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public override void KillPlayer()
    {
        anim.Play("Death");
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
        {
            gameController.GetComponent<GameControllerScriptTutorial>().PlayerKilled();
        }
        else
        {
            gameController.GetComponent<GameControllerScript>().PlayerKilled(isPlayer2);
        }
    }

    public void PowerUpActive(bool active)
    {
        PowerUp = active;
        SendMessage("CubeActivatedSfx");
    }

    public void PowerUpDeactivated(bool deActivate)
    {
        PowerUp = deActivate;
        SendMessage("CubeDeactivatedSfx");
    }

    /// <summary>
    /// Function receives impulse received by colliders on the enemy characters.
    /// </summary>
    /// <param name="impulseVal"></param>
    public override void ImpactReceived(Collision collision)
    {
        //AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (collision.gameObject.tag.Contains("Enemy"))  //|| (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            hitByEnemy = true;
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                float dmgAmount = Math.Abs(collision.impulse.magnitude) / HealthModifier;
                if (dmgAmount > maxDamageAmount)
                {
                    dmgAmount = maxDamageAmount;
                }
                SetPlayerHealth(dmgAmount);
            }
            UpdateHealthUI();
            playerUI.GetComponent<PlayerUserInterface>().setHitUIimage(true, 1);
        }
        else if (hitByEnemy)
        {
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                float dmgAmount = Math.Abs(collision.impulse.magnitude) / HealthModifier;
                if (dmgAmount > maxDamageAmount)
                {
                    dmgAmount = maxDamageAmount;
                }
                SetPlayerHealth(dmgAmount);
            }
            UpdateHealthUI();
            playerUI.GetComponent<PlayerUserInterface>().setHitUIimage(true, 1);
        }
    }

    /// <summary>
    /// Function to reset player after they have died or flown out of the map.
    /// </summary>
    /// <param name="resetLocation">Location to set the player to.</param>
    public override void PlayerBeingReset(Transform resetLocation)
    {
        //currently just reloading the game
        LoadLevel.loader.ReloadScene();
    }

    public void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = GetPlayerHealth() / originalHealth;
        }
    }
    public float GetOriginalHealth()
    {
        return originalHealth;
    }
}
