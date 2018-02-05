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
    private Animator anim;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private GameObject puppetMast;
    private GameObject gameController;
    private GameObject playerUI;
    private int enemyIndex = 0;
    private UserControlAI charController;

    private SwapMaterials ShowDmg;
    private float Val0;
    private float Val1;
    private float Val2;
    private float Val3;
    private float Val4;

    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        impactSource = gameObject.AddComponent<AudioSource>();
        source.spatialize = true;
        source.volume = 0.6f;
        sfxManager = FindObjectOfType<SFX_Manager>();
        dead = false;
        damageIsFromPlayer = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        playerUI = GameObject.FindGameObjectWithTag("playerUI");
        charController = GetComponentInChildren<UserControlAI>();
        ShowDmg = this.GetComponent<SwapMaterials>();
        deathObject.SetActive(false);
        Val4 = 0;
        Val0 = 4 * (EnemyHealth / 5);
        Val1 = 3 * (EnemyHealth / 5);
        Val2 = 2 * (EnemyHealth / 5);
        Val3 = 1 * (EnemyHealth / 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyHealth <= 0f && !dead)
        {
            KillEnemy();
        }
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
        if (tagOfCollision == "Player")
        {
            damageIsFromPlayer = true; //after animator states enemy has stood up, change this to false.
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
                gameController.GetComponent<GameControllerScript>().isKilled(enemyIndex);
                playerUI.GetComponent<PlayerUserInterface>().enemyIsKilled();
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
        KillEnemy();
        this.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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
}

