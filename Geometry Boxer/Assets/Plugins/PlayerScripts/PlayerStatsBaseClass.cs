using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using System;


public class PlayerStatsBaseClass : MonoBehaviour
{
    public GameObject pelvisJoint;
    public float deathDelay = 20f;
    [Header("How much force is required for the player to take damage.")]
    public float damageThreshold = 100f;

    protected bool dead;
    protected bool hitByEnemy;
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
    protected GameObject charController;
    protected BehaviourPuppet behavePuppet;

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
        hitByEnemy = false;
        dead = false;
    }

    public PlayerStatsBaseClass(float newHealth, float newStability, float newSpeed, float newForce, float newMult)
    {
        health = newHealth;
        stability = newStability;
        speed = newSpeed;
        attackForce = newForce;
        fallDamageMultiplier = newMult;
    }

    protected virtual void Start()
    {
        health = 1000f;
        stability = 1.0f;
        speed = 1.0f;
        attackForce = 1.0f;
        fallDamageMultiplier = 1.0f;
        hitByEnemy = false;
        dead = false;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        behavePuppet = this.transform.GetComponentInChildren<BehaviourPuppet>();
    }

    protected virtual void LateUpdate()
    {
        hitByEnemy = CheckIfKnockedDown();
        //Debug.Log(hitByEnemy);
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
    /// Applies stability multiplier to the collision resistance parameter of the
    /// behavior puppet. Method can be overriden to behave differently.
    /// </summary>
    public virtual void ApplyStabilityStat()
    {
        behavePuppet.collisionResistance = new Weight(behavePuppet.collisionResistance.floatValue * stability);
    }

    /// <summary>
    /// Function to reset the player's location when falling outside the saftey net.
    /// </summary>
    /// <param name="resetLocation">Empty game object's transform describing where player should go.</param>
    public virtual void PlayerBeingReset(Transform resetLocation)
    {
        /*
        hitByEnemy = false;
        behavePuppet.Reset(resetLocation.position, Quaternion.identity);
        SetVelocityToZero(puppetMast.transform.GetChild(0).gameObject); //send the pelvis joint as the starting rigid body to stop velocity
        charController.transform.position = resetLocation.position;
        */
    }

    private void SetVelocityToZero(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetVelocityToZero(obj.transform.GetChild(i).gameObject);
        }
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    /// <summary>
    /// Function receives impulse received by colliders on the enemy characters.
    /// </summary>
    /// <param name="collision">collision information sent to this character.</param>
    public virtual void ImpactReceived(Collision collision)
    {

        //AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (collision.gameObject.tag == "EnemyCollision")  //|| (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            hitByEnemy = true;
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                SetPlayerHealth(Math.Abs(collision.impulse.magnitude));
            }
        }
        else if(hitByEnemy)
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

    protected virtual bool CheckIfKnockedDown()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if(behavePuppet.state == BehaviourPuppet.State.Unpinned || info.IsName(fall))
        {
            return hitByEnemy;
        }
        return false;
    }
}
