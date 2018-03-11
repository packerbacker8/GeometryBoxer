using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class PlayerHealthScript : MonoBehaviour
{
    public float PlayerHealth = 10000f;
    public float deathDelay = 20f;
    [Header("How much force is required for the player to take damage.")]
    public float damageThreshold = 100f;

    private bool dead;
    private Animator anim;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private GameObject puppetMast;
    private GameObject gameController;
    private float cubeHealthModifier = 1f;


    // Use this for initialization
    void Start()
    {
        dead = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerHealth <= 0f && !dead)
        {
            dead = true;
            KillPlayer();
        }
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
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                PlayerHealth -= Math.Abs(collision.impulse.magnitude) / cubeHealthModifier;
            }
        }

    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public void KillPlayer()
    {
        anim.Play("Death");
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
        gameController.GetComponent<GameControllerScript>().PlayerKilled(false);

        //Destroy(this.transform.gameObject,deathDelay);  //To be destroyed by game manager if body count exceeds certain amout.
    }

    /// <summary>
    /// Function to give player more health.
    /// </summary>
    /// <param name="amount">How much health to give.</param>
    public void GiveHealth(int amount)
    {
        PlayerHealth += amount;
    }

    public void setCubeHealthModifier(float amount)
    {
        cubeHealthModifier = amount;
    }
}
