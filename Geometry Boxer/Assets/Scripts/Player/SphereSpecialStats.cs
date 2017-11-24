using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class SphereSpecialStats : PlayerStatsBaseClass
{
    private float originalHealth;
    private bool switched;
    // Use this for initialization
    void Start()
    {
        dead = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");

        health = 5000f;
        originalHealth = health;
        switched = false;
        stability = 0.5f;
        speed = 1.0f; //builds up as they move more
        attackForce = 1.0f; //builds up as they move more
        fallDamageMultiplier = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= originalHealth/2 && !switched)
        {

        }
        if (health <= 0f && !dead)
        {
            dead = true;
            KillPlayer();
        }
    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public override void KillPlayer()
    {
        anim.Play("Death");
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
        gameController.GetComponent<GameControllerScript>().playerKilled();

        //Destroy(this.transform.gameObject,deathDelay);  //To be destroyed by game manager if body count exceeds certain amout.
    }

    public void ActivateRollAttack()
    {

    }
}
