using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Demos;

public class CubeAttackScript : CubePunchScript
{
    public GameObject pelvisJoint;
    public float PowerUpTimeLimit = 10;
    public KeyCode ballFormKey = KeyCode.LeftControl;

    private Rigidbody playerRigidBody;
    private PlayerHealthScript HealthScript;
    public bool PowerUp = false;
    private float TimePowerUp;
    private Behaviour halo;
    private bool isGrounded;
    private CharacterMeleeDemo charMelDemo;
    private GameObject puppetMast;
    private GameObject gameController;

    // This is puppetMasters user controler, it controls the players movements
    protected UserControlThirdPerson userControl; // user input


    // Use this for initialization
    void Start()
    {
        HealthScript = this.GetComponent<PlayerHealthScript>();
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        TimePowerUp = PowerUpTimeLimit;
        halo = (Behaviour)charController.GetComponent("Halo");
        userControl = charController.GetComponent<UserControlThirdPerson>();
        playerRigidBody = pelvisJoint.GetComponent<Rigidbody>();
        charMelDemo = this.transform.GetComponentInChildren<CharacterMeleeDemo>();
        isGrounded = charMelDemo.animState.onGround;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ballFormKey) && !PowerUp)
        {
            PowerUp = true;
            halo.enabled = true;
            SendMessage("PowerUpActive", true);

            puppetMast.transform.localScale += new Vector3(2F, 2F, 2F);
            charController.transform.localScale += new Vector3(2F, 2F, 2F);

        }

        

        if (PowerUp == true)
        {
            TimePowerUp -= 1 * Time.deltaTime;
            if (TimePowerUp <= 0)
            {
                PowerUp = false;
                halo.enabled = false;

                puppetMast.transform.localScale -= new Vector3(2F, 2F, 2F);
                charController.transform.localScale -= new Vector3(2F, 2F, 2F);
                TimePowerUp = PowerUpTimeLimit;
                SendMessage("PowerUpDeactivated", false);
            }
        }

    }
}


