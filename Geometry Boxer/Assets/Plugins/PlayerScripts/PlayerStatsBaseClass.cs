using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using System;


public class PlayerStatsBaseClass : MonoBehaviour
{
    public float deathDelay = 20f;
    [Header("How much force is required for the player to take damage.")]
    public float damageThreshold = 100f;

    protected bool dead;
    protected Animator anim;
    protected int characterControllerIndex = 2;
    protected int animationControllerIndex = 0;
    protected int puppetMasterIndex = 1;
    protected string getUpProne = "GetUpProne";
    protected string getUpSupine = "GetUpSupine";
    protected string protectedAnim = "Protective";
    protected string fall = "Fall";
    protected GameObject puppetMast;
    protected GameObject gameController;

    protected float health;
    protected float stability;
    protected float speed;
    protected float attackForce;
    protected float fallDamageMultiplier;

    public PlayerStatsBaseClass()
    {
        health = 1000f;
        stability = 1.0f;
        speed = 1.0f;
        attackForce = 1.0f;
        fallDamageMultiplier = 1.0f;
    }

    public PlayerStatsBaseClass(float newHealth, float newStability, float newSpeed, float newForce, float newMult)
    {
        health = newHealth;
        stability = newStability;
        speed = newSpeed;
        attackForce = newForce;
        fallDamageMultiplier = newMult;
    }

    /// <summary>
    /// Get the current health of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <returns>The float that is returned is the current health.</returns>
    public virtual float GetPlayerHealth()
    {
        return health;
    }

    /// <summary>
    /// Set the health of the player. This method can be overridden to behave 
    /// differently.
    /// </summary>
    /// <param name="val">This value changes the health in some way. The default way is to subtract it off the current health.</param>
    public virtual void SetPlayerHealth(float val)
    {
        health -= val;
    }

    /// <summary>
    /// Get the stability level of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <returns>Return the value of the stability level.</returns>
    public virtual float GetPlayerStability()
    {
        return stability;
    }

    /// <summary>
    /// Set the stability level of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <param name="newStability">This value is used to change the stability to itself.  By default stability is set equal to the parameter.</param>
    public virtual void SetPlayerStability(float newStability)
    {
        stability = newStability;
    }

    /// <summary>
    /// Get the speed of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <returns>The value returned is the current speed of the player.</returns>
    public virtual float GetPlayerSpeed()
    {
        return speed;
    }

    /// <summary>
    /// Set the speed of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <param name="newSpeed">This value replaces the current speed value of the player.</param>
    public virtual void SetPlayerSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    /// <summary>
    /// Get the attack force of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <returns>The value returned is the force of the player's basic attacks.</returns>
    public virtual float GetPlayerAttackForce()
    {
        return attackForce;
    }

    /// <summary>
    /// Set the attack force of the player. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <param name="newForce">The attack force is replaced by the passed value.</param>
    public virtual void SetPlayerAttackForce(float newForce)
    {
        attackForce = newForce;
    }

    /// <summary>
    /// Get the fall damage multiplier. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <returns>The value returned is how much falling damage affects this player.</returns>
    public virtual float GetPlayerFallMultiplier()
    {
        return fallDamageMultiplier;
    }

    /// <summary>
    /// Set the fall damage multiplier. This method can be overridden to behave
    /// differently.
    /// </summary>
    /// <param name="newMult">The fall damage multiplier is replaced by this new value.</param>
    public virtual void SetPlayerFallMultiplier(float newMult)
    {
        fallDamageMultiplier = newMult;
    }

    

    /// <summary>
    /// Function receives impulse received by colliders on the enemy characters.
    /// </summary>
    /// <param name="impulseVal"></param>
    public virtual void ImpactReceived(Collision collision)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (collision.gameObject.tag == "EnemyCollision" || (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                SetPlayerHealth(Math.Abs(collision.impulse.magnitude));
            }
        }

    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then removes the object.
    /// </summary>
    public virtual void KillPlayer()
    {
        anim.Play("Death");
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
    }

    /// <summary>
    /// Function to give player more health.
    /// </summary>
    /// <param name="amount">How much health to give.</param>
    public virtual void GiveHealth(int amount)
    {
        health += amount;
    }
}
