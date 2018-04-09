using System;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.Demos;

public class ArenaEnemyHealthScript : EnemyHealthScript
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void GetHealth()
    {
        base.GetHealth();
    }

    /// <summary>
    /// Function to kill enemy AI unit, plays associated death animation then deactivates the object.
    /// </summary>
    public override void KillEnemy()
    {
        if (!dead)
        {
            charController.gameObject.GetComponent<CharacterPuppet>().propRoot.currentProp = null;
            if (sfxManager != null && sfxManager.maleDeath.Count > 0)
            {
                source.PlayOneShot(sfxManager.maleDeath[rand.Next(0, sfxManager.maleDeath.Count)]);
            }
            puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
            dead = true;
            deathObject.SetActive(true);
            GameObject deathObjClone = Instantiate(deathObject, deathObject.transform.position, deathObject.transform.rotation);
            deathObjClone.GetComponent<ScatterAndDestroy>().BeginDestruction(deathDelay);
            Destroy(deathObjClone);
            deathObject.SetActive(false);
            bool shouldDie = true;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
            {
                gameController.GetComponent<GameControllerScriptTutorial>().isKilled(enemyIndex);
            }
            else
            {
                shouldDie = gameController.GetComponent<GameControllerScript>().IsKilled(enemyIndex, this.gameObject.tag);
            }
            if (shouldDie)
            {
                this.gameObject.SetActive(false);  //To be destroyed by game manager if body count exceeds certain amout.
            }
        }
    }

    /// <summary>
    /// Reset values of the enemy health script for arena bots.
    /// </summary>
    /// <param name="newPosTransform">The new location the bot will spawn at.</param>
    public void ResetValues(Transform newPosTransform)
    {
        //this.transform.position = newPosTransform.position;
        charController.transform.position = newPosTransform.position;
        dead = false;
        damageIsFromPlayer = false;
        findHealth = false;

        deathObject.SetActive(false);
        EnemyHealth = originalHealth;
        puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Alive;
    }

    /// <summary>
    /// Sets the bot's original health to the passed new value. To be used in tandem
    /// with the <c>ResetValues()</c> method when resetting the bots.
    /// </summary>
    /// <param name="newHealth">The health that the original health variable will now be.</param>
    public void SetOriginalHealth(float newHealth)
    {
        originalHealth = newHealth;
    }
}
