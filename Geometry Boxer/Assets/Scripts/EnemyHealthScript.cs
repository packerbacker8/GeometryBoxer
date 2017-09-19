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

    private bool dead;
    private Animator anim;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private GameObject puppetMast;

    // Use this for initialization
    void Start()
    {
        dead = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
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
            }
        }

    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public void KillEnemy()
    {
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
        //Destroy(this.transform.gameObject,deathDelay);  //To be destroyed by game manager if body count exceeds certain amout.
    }
}
