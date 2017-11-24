using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsBaseClass : MonoBehaviour
{
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
}
