using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.Demos;

public class EnemyHealthScript : MonoBehaviour
{
    public float EnemyHealth = 1000f;
    public float deathDelay = 20f;
    public float damageThreshold = 100f;

    //Sound Engine Needs
    private AudioSource source;
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
    private int enemyIndex = 0;
    private UserControlAI charController;

    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialize = true;
        source.volume = 0.6f;
        sfxManager = FindObjectOfType<SFX_Manager>();
        dead = false;
        damageIsFromPlayer = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        charController = GetComponentInChildren<UserControlAI>();
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
        //Debug.Log("Impact Force: " + collision.impulse.magnitude);
        string tagOfCollision = collision.gameObject.transform.root.tag;
        if (tagOfCollision == "Player")
        {
            damageIsFromPlayer = true; //after animator states enemy has stood up, change this to false.
        }
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (!dead && collision.impulse.magnitude > damageThreshold && (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            if (!charController.drop)
            {
                EnemyHealth -= Math.Abs(collision.impulse.magnitude);
                if (!source.isPlaying && sfxManager.malePain.Count > 0 && tagOfCollision == "Player")
                {
                    painIndex = rand.Next(0, sfxManager.malePain.Count);
                    source.PlayOneShot(sfxManager.malePain[painIndex], 1f);
                }
            }
        }
    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public void KillEnemy()
    {
        if (!dead)
        {
            anim.Play("Death");

            if (sfxManager.maleDeath.Count > 0)
            {
                source.PlayOneShot(sfxManager.maleDeath[rand.Next(0, sfxManager.maleDeath.Count)]);
            }
            puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
            gameController.GetComponent<GameControllerScript>().isKilled(enemyIndex);
            dead = true;
            //Destroy(this.transform.gameObject,deathDelay);  //To be destroyed by game manager if body count exceeds certain amout.
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

