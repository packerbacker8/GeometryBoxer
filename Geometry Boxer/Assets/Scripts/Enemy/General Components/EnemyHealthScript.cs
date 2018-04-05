using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.Demos;
using PlayerUI;

public class EnemyHealthScript : MonoBehaviour
{
    public float EnemyHealth = 1000f;
    [Tooltip("0.99 means to find health pack at 100% health, 0.01 means to find health pack at 2% health.")]
    [Range(0.01f, 0.99f)]
    public float percentToFindHealthPack = 0.5f;
    public float deathDelay = 20f;
    public float damageThreshold = 5f;
    public float heavyDamageOffset = 10f;
    public GameObject deathObject;

    //Sound Engine Needs
    protected AudioSource source;
    protected AudioSource impactSource;
    protected int lightImpactIndex;
    protected int heavyImpactIndex;
    protected int painIndex;
    protected int deathIndex;
    protected System.Random rand = new System.Random();
    protected SFX_Manager sfxManager;
    
    protected bool dead;
    protected bool damageIsFromPlayer;
    protected bool damageFromAllies;
    protected bool findHealth;
    protected int characterControllerIndex = 2;
    protected int animationControllerIndex = 0;
    protected int puppetMasterIndex = 1;
    protected int enemyIndex = 0;
    protected string getUpProne = "GetUpProne";
    protected string getUpSupine = "GetUpSupine";
    protected string damageSource = "Player";
    protected GameObject puppetMast;
    protected GameObject gameController;
    protected GameObject playerUI;
    protected GameObject healthContainer;
    protected UserControlAI charController;
    protected Animator anim;
    
    protected SwapMaterials ShowDmg;
    protected float Val0;
    protected float Val1;
    protected float Val2;
    protected float Val3;
    protected float Val4;
    protected float originalHealth;

    protected virtual void Awake()
    {
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        playerUI = GameObject.FindGameObjectWithTag("playerUI");
        healthContainer = GameObject.FindGameObjectWithTag("HealthContainer");
        charController = GetComponentInChildren<UserControlAI>();

        source = gameObject.AddComponent<AudioSource>();
        impactSource = gameObject.AddComponent<AudioSource>();
        sfxManager = FindObjectOfType<SFX_Manager>();
        ShowDmg = this.GetComponent<SwapMaterials>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
        source.spatialBlend = 0.9f;
        impactSource.spatialBlend = 0.9f;
        source.volume = 1f;
        dead = false;
        damageIsFromPlayer = false;
        findHealth = false;
        

        deathObject.SetActive(false);
        Val4 = 0;
        Val0 = 4 * (EnemyHealth / 5);
        Val1 = 3 * (EnemyHealth / 5);
        Val2 = 2 * (EnemyHealth / 5);
        Val3 = 1 * (EnemyHealth / 5);
        originalHealth = EnemyHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(!findHealth && EnemyHealth < originalHealth * percentToFindHealthPack)
        {
            GetHealth();
        }
        if (EnemyHealth <= 0f && !dead)
        {
            KillEnemy();
        }
    }

    /// <summary>
    /// Function for the enemy to go find a health pack, in some random way.
    /// </summary>
    protected virtual void GetHealth()
    {
        if(healthContainer == null)
        {
            return; //no healthpacks in arena
        }
        findHealth = true;
        System.Random rand = new System.Random();
        rand.Next();
        int randIndex;
        int count = 0;
        GameObject healthObj = null;
        while(healthObj == null && count < healthContainer.transform.childCount)
        {
            randIndex = rand.Next(healthContainer.transform.childCount);
            if (healthContainer.transform.GetChild(randIndex).GetComponent<MeshRenderer>().enabled)
            {
                healthObj = healthContainer.transform.GetChild(randIndex).gameObject;
            }
            count++;
        }
        if(healthObj == null)
        {
            return;
        }
        else
        {
            gameController.GetComponent<GameControllerScript>().SetTargetHealthPack(enemyIndex, healthObj, this.gameObject.tag);
        }
    }

    /// <summary>
    /// Function to call to set target to new target for AI
    /// </summary>
    public void SetOurTarget()
    {
        gameController.GetComponent<GameControllerScript>().SetNewTarget(enemyIndex, this.gameObject.tag);
    }

    /// <summary>
    /// Function receives impulse received by colliders on the enemy characters.
    /// </summary>
    /// <param name="collision">The collision and its information recieved by the enemy colliders.</param>
    public void ImpactReceived(Collision collision)
    {
        float lightImpactThreshold = damageThreshold;
        float heavyImpactThreshold = damageThreshold + heavyDamageOffset;
        float collisionMagnitude = collision.impulse.magnitude;
        string tagOfCollision = collision.gameObject.transform.root.tag;
        if (tagOfCollision.Contains(damageSource))
        {
            damageIsFromPlayer = true; //after animator states enemy has stood up, change this to false.
        }
        //way to make enemy bots also take damage from allied bots
        if(damageFromAllies && tagOfCollision.Contains("Ally"))
        {
            damageIsFromPlayer = true;
        }

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (!dead && damageIsFromPlayer)           //&& (!info.IsName(getUpProne) && !info.IsName(getUpSupine)) && !charController.drop)
        {
            if (collisionMagnitude <= heavyImpactThreshold)
            {
                EnemyHealth -= Math.Abs(collisionMagnitude);
            }
            else if (collisionMagnitude > heavyImpactThreshold)
            {
                EnemyHealth -= Math.Abs(collisionMagnitude + heavyDamageOffset);
            }

            if (sfxManager.malePain.Count > 0 && !source.isPlaying && damageIsFromPlayer)
            {
                painIndex = rand.Next(0, sfxManager.malePain.Count);
                lightImpactIndex = rand.Next(0, sfxManager.lightPunches.Count);
                heavyImpactIndex = rand.Next(0, sfxManager.heavyPunches.Count);
                //source.clip = sfxManager.malePain[painIndex];
                //source.Play();
                source.PlayOneShot(sfxManager.malePain[painIndex], 1f);

                if (collisionMagnitude <= heavyImpactThreshold)
                {
                    impactSource.PlayOneShot(sfxManager.lightPunches[lightImpactIndex], 1f);
                }
                else if (collisionMagnitude > heavyImpactThreshold)
                {
                    impactSource.PlayOneShot(sfxManager.heavyPunches[heavyImpactIndex], 1f);
                }
            }
        }
        if (damageIsFromPlayer)
        {
            damageIsFromPlayer = charController.IsKnockedDown();
        }
        if (EnemyHealth > Val0)
        {
            ShowDmg.SetMaterial0();
        }
        else if (EnemyHealth > Val1)
        {
            ShowDmg.SetMaterial1();
        }
        else if (EnemyHealth > Val2)
        {
            ShowDmg.SetMaterial2();
        }
        else if (EnemyHealth > Val3)
        {
            ShowDmg.SetMaterial3();
        }
        else if (EnemyHealth > Val4)
        {
            ShowDmg.SetMaterial4();
        }
        else
        {
            ShowDmg.SetMaterial5();
        }
    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public virtual void KillEnemy()
    {
        if (!dead)
        {
            //anim.Play("Death");

            if (sfxManager != null && sfxManager.maleDeath.Count > 0)
            {
                source.PlayOneShot(sfxManager.maleDeath[rand.Next(0, sfxManager.maleDeath.Count)]);
            }
            puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
            {
                gameController.GetComponent<GameControllerScriptTutorial>().isKilled(enemyIndex);
            }
            else
            {
                gameController.GetComponent<GameControllerScript>().IsKilled(enemyIndex, this.gameObject.tag);
            }

            dead = true;

            deathObject.SetActive(true);
            deathObject.GetComponent<ScatterAndDestroy>().BeginDestruction(deathDelay);

            Destroy(this.transform.gameObject);  //To be destroyed by game manager if body count exceeds certain amout.
        }
    }

    /// <summary>
    /// Helper function to kill enemy and freeze them if they go outside the bounds of the world.
    /// </summary>
    public virtual void ResetEnemy()
    {
        this.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        KillEnemy();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="val"></param>
    public void SetEnemyIndex(int val)
    {
        enemyIndex = val;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetEnemyIndex()
    {
        return enemyIndex;
    }

    /// <summary>
    /// Helper function to ask if the enemy is dead or not.
    /// </summary>
    /// <returns>Returns bool indicating if enemy is dead or not. Dead = true.</returns>
    public bool GetIsDead()
    {
        return dead;
    }

    /// <summary>
    /// Function to allow the changing of where damage can come from against the bot.
    /// </summary>
    /// <param name="source">Set to 'Player' to take damage from the player, and 'Enemy' to take damage from other bots.</param>
    /// <param name="allies">If this is true, the enemy bot will also take damage from collisions tagged Ally.</param>
    public void SetDamageSource(string source, bool allies)
    {
        damageSource = source;
        damageFromAllies = allies;
    }

    /// <summary>
    /// Helper function called by detect movement ai to tell the game controller to change the target of 
    /// the given enemy based on its index.
    /// </summary>
    /// <param name="player2">Is this player 2?</param>
    public void ChangeOurTarget(bool player2)
    {
        gameController.GetComponent<GameControllerScript>().ChangeTarget(enemyIndex, player2);
    }

    /// Return the health the enemy originally started out with.
    /// </summary>
    /// <returns>Float of health the enemy started with.</returns>
    public float GetEnemyOriginalHealth()
    {
        return originalHealth;
    }

    /// <summary>
    /// Give the enemy bot more health from pickup.
    /// </summary>
    /// <param name="toAdd">How much the health is going to up by.</param>
    public void AddHealth(float toAdd)
    {
        EnemyHealth += toAdd;
    }
}

