﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.Dynamics;
using RootMotion.Demos;
using System;
using PlayerUI;

public class OctahedronStats : PlayerStatsBaseClass
{
    private float originalHealth;
    private float HealthModifier;
    private Image healthBarBackground;
    private Image healthBarFill;

    protected override void Start()
    {
        base.Start();

        HealthModifier = 1.0f;
        health = SaveAndLoadGame.saver.GetLoadedFightScene() ? SaveAndLoadGame.saver.GetPlayerCurrentHealth() : Health;
        playerUI.GetComponent<PlayerUserInterface>().SetHealth(health);
        originalHealth = Health;
        playerUI.GetComponent<PlayerUserInterface>().SetMaxHealth(originalHealth);
        healthBarBackground = GameObject.FindGameObjectWithTag("HealthBarBackground").GetComponent<Image>();
        healthBarFill = GameObject.FindGameObjectWithTag("HealthBarBackground").transform.GetChild(0).GetComponent<Image>();
        playerUI.GetComponent<PlayerUserInterface>().SetPlayerType(2);
        UpdateHealthUI();
    }

    // Update is called once per frame
    protected override void LateUpdate()
    {
        base.LateUpdate();
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
        gameController.GetComponent<GameControllerScript>().PlayerKilled();

        //Destroy(this.transform.gameObject,deathDelay);  //To be destroyed by game manager if body count exceeds certain amout.
    }

    public override void PlayerBeingReset(Transform resetLocation)
    {
        //base.PlayerBeingReset(resetLocation);
        LoadLevel.loader.ReloadScene();
    }


    public override void ImpactReceived(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyCollision")  //|| (!info.IsName(getUpProne) && !info.IsName(getUpSupine)))
        {
            hitByEnemy = true;
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                float dmgAmount = Math.Abs(collision.impulse.magnitude) / HealthModifier;
                if (dmgAmount > maxDamageAmount)
                {
                    dmgAmount = maxDamageAmount;
                    Debug.Log("Damage capped.");
                }
                SetPlayerHealth(dmgAmount);
            }
            UpdateHealthUI();
        }
        else if (hitByEnemy)
        {
            if (!dead && collision.impulse.magnitude > damageThreshold)
            {
                float dmgAmount = Math.Abs(collision.impulse.magnitude) / HealthModifier;
                if (dmgAmount > maxDamageAmount)
                {
                    dmgAmount = maxDamageAmount;
                    Debug.Log("Damage capped.");
                }
                SetPlayerHealth(dmgAmount);
            }
            UpdateHealthUI();
        }
    }
    public void UpdateHealthUI()
    {
        if(healthBarFill != null)
        {
            healthBarFill.fillAmount = GetPlayerHealth() / originalHealth;
        }
    }
    public float GetOriginalHealth()
    {
        return originalHealth;
    }
    /*
    public OctahedronStats() : base(1000f, 1.0f, 1.0f, 1.0f, 1.0f)
	{

	}
    */
}
