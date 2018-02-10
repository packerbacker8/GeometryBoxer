using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.Demos;

public class EnemyHealthScript : MonoBehaviour
{
    public float EnemyHealth = 1000f;
    [Tooltip("0.99 means to find health pack at 100% health, 0.01 means to find health pack at 1% health.")]
    [Range(0.01f, 0.99f)]
    public float percentToFindHealthPack = 0.5f;
    public float deathDelay = 20f;
    public float damageThreshold = 5f;
    public float heavyDamageOffset = 10f;
    public GameObject deathObject;

    //Sound Engine Needs
    private AudioSource source;
    private AudioSource impactSource;
    private int lightImpactIndex;
    private int heavyImpactIndex;
    private int painIndex;
    private int deathIndex;
    private System.Random rand = new System.Random();
    private SFX_Manager sfxManager;

    private bool dead;
    private bool damageIsFromPlayer;
    private bool damageFromAllies;
    private bool findHealth;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private int enemyIndex = 0;
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string damageSource = "Player";
    private GameObject puppetMast;
    private GameObject gameController;
    private GameObject playerUI;
    private GameObject healthContainer;
    private UserControlAI charController;
    private Animator anim;

    private SwapMaterials ShowDmg;
    private float Val0;
    private float Val1;
    private float Val2;
    private float Val3;
    private float Val4;
    private float originalHealth;

    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.75f;
        impactSource = gameObject.AddComponent<AudioSource>();
        impactSource.spatialBlend = 0.75f;
        source.spatialize = true;
        source.volume = 0.6f;
        sfxManager = FindObjectOfType<SFX_Manager>();
        dead = false;
        damageIsFromPlayer = false;
        findHealth = false;
        
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        playerUI = GameObject.FindGameObjectWithTag("playerUI");
        healthContainer = GameObject.FindGameObjectWithTag("HealthContainer");
        charController = GetComponentInChildren<UserControlAI>();
        ShowDmg = this.GetComponent<SwapMaterials>();
        deathObject.SetActive(false);
        Val4 = 0;
        Val0 = 4 * (EnemyHealth / 5);
        Val1 = 3 * (EnemyHealth / 5);
        Val2 = 2 * (EnemyHealth / 5);
        Val3 = 1 * (EnemyHealth / 5);
        originalHealth = EnemyHealth;
    }

    // Update is called once per frame
    void Update()
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
    private void GetHealth()
    {
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
    public void KillEnemy()
    {
        if (!dead)
        {
            //anim.Play("Death");

            if (sfxManager.maleDeath.Count > 0)
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
                gameController.GetComponent<GameControllerScript>().isKilled(enemyIndex, this.gameObject.tag);
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
    public void ResetEnemy()
    {
        Debug.Log("y velocity of bot: " + this.transform.GetChild(2).gameObject.GetComponent<Rigidbody>().velocity.y);

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
    public void ChangeOurTarget()
    {
        gameController.GetComponent<GameControllerScript>().ChangeTarget(enemyIndex);
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

