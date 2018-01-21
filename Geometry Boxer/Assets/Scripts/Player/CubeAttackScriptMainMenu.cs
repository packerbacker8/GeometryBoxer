using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Demos;

public class CubeAttackScriptMainMenu : PunchScript
{
    public bool PowerUp = false;

    private Rigidbody playerRigidBody;
    private float TimePowerUp;
    private Behaviour halo;

    private CubeSpecialStats stats;

    private bool isGrounded;
    private GameObject gameController;
    private GameObject playerUI;
    private float coolDownTime;
    private float coolDownTimer;
    private float cubeForce;


    // This is puppetMasters user controler, it controls the players movements
    protected UserControlThirdPerson userControl; // user input


    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        stats = this.GetComponent<CubeSpecialStats>();
        baseStats = this.GetComponent<CubeSpecialStats>();
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        halo = (Behaviour)charController.GetComponent("Halo");
        userControl = charController.GetComponent<UserControlThirdPerson>();
        coolDownTime = 10;
        //playerUI = GameObject.FindGameObjectWithTag("playerUI");
        //playerUI.GetComponent<userInterface>().SetCoolDownTime(coolDownTime);
        playerRigidBody = stats.pelvisJoint.GetComponent<Rigidbody>();
        PowerUp = false;
        isGrounded = checkIfGrounded();
        specialStartSize = new Vector3(0.2f, 0.2f, 0.2f);
        specialEndSize = new Vector3(specialFormSize, specialFormSize, specialFormSize);
        specialForm.GetComponent<MeshRenderer>().enabled = false;
        specialForm.GetComponent<BoxCollider>().enabled = false;
        specialRigid = specialForm.GetComponent<Rigidbody>();
        coolDownTimer = 0f;
        cubeForce = 1000f;
        specialRigid.useGravity = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        /*
        isGrounded = checkIfGrounded();
        Debug.Log(isGrounded);
        if (isGrounded)
        {
            launched = false;
        }
        if (growingSpecial)
        {
            GrowSpecial();
        }
        else if (playerGrowing)
        {
            GrowPlayer();
        }
        else if (specialForm.GetComponent<MeshRenderer>().enabled)
        {
            UpdatePos(charController.transform, specialForm.transform);
            coolDownTimer += Time.deltaTime;
            if (coolDownTimer >= specialAttackActiveTime)
            {
                DeactivateSpecialAttack();
                UpdatePos(charController.transform, specialForm.transform);
                coolDownTimer = 0f;
            }
            if ((Input.GetKeyDown(specialAttack) || Input.GetButtonDown("XButton")))
            {
                DeactivateSpecialAttack();
                UpdatePos(charController.transform, specialForm.transform);
                coolDownTimer = 0f;
            }
            if (Input.GetKeyDown(useAttack) && isGrounded && specialForm.GetComponent<MeshRenderer>().enabled) //include jump key for controller
            {
                specialRigid.AddForce(Vector3.up * cubeForce * 100f);
            }
            else if (Input.GetKeyDown(useAttack) && specialForm.GetComponent<MeshRenderer>().enabled && !launched)
            {
                specialRigid.AddForce(-Vector3.up * cubeForce * 300f);
                launched = true;

            }
            if (!isGrounded)
            {
                moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                moveDir = cam.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir = Vector3.Normalize(moveDir);
                moveDir.x = moveDir.x * cubeForce * stats.GetPlayerSpeed();
                moveDir.z = moveDir.z * cubeForce * stats.GetPlayerSpeed();
                specialRigid.AddForce(moveDir);
            }
        }
        else
        {
            UpdatePos(specialForm.transform, charController.transform);

            if ((Input.GetKeyDown(specialAttack) || Input.GetButtonDown("XButton")) && !specialForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
            {
                growingSpecial = true;
                ActivateSpecialAttack();
                UpdatePos(specialForm.transform, charController.transform);
            }
        }

        if (onCooldown)
        {
            coolDownTimer += Time.deltaTime;
            if (coolDownTimer >= specialAttackCooldownTime)
            {
                onCooldown = false;
                coolDownTimer = 0f;
            }
        }

        //GrowBigPower();
        */
    }
    /// <summary>
    /// Allows character to grow larger when attack key is pressed. Shrinks back down after a certain
    /// amount of time.
    /// </summary>
    private void GrowBigPower()
    {
        if (!PowerUp && Input.GetKeyDown(useAttack) || !PowerUp && Input.GetButtonDown("XButton"))
        {
            PowerUp = true;
            halo.enabled = true;
            SendMessage("PowerUpActive", true);

            puppetMastObject.transform.localScale += new Vector3(2F, 2F, 2F);
            charController.transform.localScale += new Vector3(2F, 2F, 2F);

        }
        else
        {
            coolDownTimer += Time.deltaTime;
            if (coolDownTimer >= coolDownTime)
            {
                onCooldown = false;
                coolDownTimer = 0;
                playerUI.GetComponent<userInterface>().SetCoolDownTime(coolDownTime);
            }
        }

        if (PowerUp)
        {
            TimePowerUp -= 1 * Time.deltaTime;
            if (TimePowerUp <= 0)
            {
                PowerUp = false;
                halo.enabled = false;

                puppetMastObject.transform.localScale -= new Vector3(2F, 2F, 2F);
                charController.transform.localScale -= new Vector3(2F, 2F, 2F);
                TimePowerUp = specialAttackActiveTime;
                SendMessage("PowerUpDeactivated", false);
                playerUI.GetComponent<userInterface>().UsedSpecialAttack();
                onCooldown = true;
            }
        }
    }

}
