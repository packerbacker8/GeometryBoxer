using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

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
    private Animator anim;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private GameObject puppetMast;
    private GameObject gameController;
    private int enemyIndex = 0;

    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        sfxManager = FindObjectOfType<SFX_Manager>();
        dead = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if(EnemyHealth <= 0f && !dead)
        {
            dead = true;
            KillEnemy();
        }
    }

    /// <summary>
    /// Function receives impulse received by colliders on the enemy characters.
    /// </summary>
    /// <param name="impulseVal"></param>
    public void ImpactReceived(Collision collision)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (collision.gameObject.tag == "Player" || (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                EnemyHealth -= Math.Abs(collision.impulse.magnitude);
                source.PlayOneShot(sfxManager.malePain[painIndex], 1f);
                painIndex = rand.Next(0, sfxManager.malePain.Count);
                //Debug.Log("Enemy health: " + EnemyHealth);
            }
        }

    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public void KillEnemy()
    {
        anim.Play("Death");
        source.PlayOneShot(sfxManager.maleDeath[rand.Next(0, sfxManager.maleDeath.Count)]);
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
        gameController.GetComponent<GameControllerScript>().isKilled(enemyIndex);
        
        //Destroy(this.transform.gameObject,deathDelay);  //To be destroyed by game manager if body count exceeds certain amout.
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
}
