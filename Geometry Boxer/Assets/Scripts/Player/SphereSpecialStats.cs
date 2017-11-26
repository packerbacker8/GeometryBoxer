using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.Demos;

public class SphereSpecialStats : PlayerStatsBaseClass
{
    public GameObject pelvisJoint;

    private float originalHealth;
    private float minSpeed;
    private float maxSpeed;
    private float minAttackForce;
    private float maxAttackForce;
    private float buildUpAmount;
    private bool switchedMesh;
    private Vector3 originalPos;
    private Rigidbody playerRigidBody;
    private CharacterMeleeDemo charMelDemo;
    private bool isGrounded;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        dead = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");

        health = 5000f;
        originalHealth = health;
        minSpeed = 1.0f;
        maxSpeed = 2.5f;
        minAttackForce = 1.0f;
        maxAttackForce = 2.5f;
        buildUpAmount = 0.1f;
        switchedMesh = false;
        stability = 0.5f;
        speed = 1.0f; //builds up as they move more
        attackForce = 1.0f; //builds up as they move more

        fallDamageMultiplier = 1.5f;

        originalPos = this.transform.position;
        playerRigidBody = pelvisJoint.GetComponent<Rigidbody>();
        charMelDemo = this.transform.GetComponentInChildren<CharacterMeleeDemo>();
        isGrounded = charMelDemo.animState.onGround;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = charMelDemo.animState.onGround;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (playerRigidBody.velocity.magnitude > 0 && (Math.Abs(Input.GetAxis("Horizontal")) > 0 || Math.Abs(Input.GetAxis("Vertical")) != 0) && isGrounded)
        {
            speed += buildUpAmount;
            attackForce += buildUpAmount;
            Mathf.Clamp(speed, minSpeed, maxSpeed);
            Mathf.Clamp(attackForce, minAttackForce, maxAttackForce);
        }
        else if(info.IsName(fall))
        {
            //keep the same as falling maybe?
        }
        else
        {
            speed -= buildUpAmount;
            attackForce -= buildUpAmount;
            Mathf.Clamp(speed, minSpeed, maxSpeed);
            Mathf.Clamp(attackForce, minAttackForce, maxAttackForce);
        }

        if(health <= originalHealth/2 && !switchedMesh)
        {
            switchedMesh = true;
            //switch mesh to lower res
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
}
