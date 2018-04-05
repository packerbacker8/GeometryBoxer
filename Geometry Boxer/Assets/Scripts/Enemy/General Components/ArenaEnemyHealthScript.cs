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
            if (sfxManager != null && sfxManager.maleDeath.Count > 0)
            {
                source.PlayOneShot(sfxManager.maleDeath[rand.Next(0, sfxManager.maleDeath.Count)]);
            }
            puppetMast.GetComponent<PuppetMaster>().state = PuppetMaster.State.Dead;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
            {
                gameController.GetComponent<GameControllerScriptTutorial>().isKilled(enemyIndex);
            }
            else
            {
                gameController.GetComponent<GameControllerScript>().IsKilled(enemyIndex, this.gameObject.tag);
            }

            dead = true;

            deathObject.SetActive(true);
            GameObject deathObjClone = Instantiate(deathObject, deathObject.transform.position, deathObject.transform.rotation);
            deathObjClone.GetComponent<ScatterAndDestroy>().BeginDestruction(deathDelay);
            deathObject.SetActive(false);
            ResetValues();
            this.gameObject.SetActive(false);  //To be destroyed by game manager if body count exceeds certain amout.
        }
    }

    /// <summary>
    /// Reset values of the enemy health script for arena bots.
    /// </summary>
    public void ResetValues()
    {
        dead = false;
        damageIsFromPlayer = false;
        findHealth = false;

        deathObject.SetActive(false);
        EnemyHealth = originalHealth;
    }
}
