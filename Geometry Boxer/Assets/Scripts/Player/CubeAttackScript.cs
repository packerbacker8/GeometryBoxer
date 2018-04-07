﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Demos;

public class CubeAttackScript : PunchScript
{
    public bool PowerUp = false;

    private Rigidbody playerRigidBody;
    private float TimePowerUp;
    private Behaviour halo;

    private bool isGrounded;
    private GameObject gameController;
    private float coolDownTime;
    private float coolDownTimer;


    // This is puppetMasters user controler, it controls the players movements
    protected UserControlThirdPerson userControl; // user input


    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        baseStats = this.GetComponent<CubeSpecialStats>();
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        halo = (Behaviour)charController.GetComponent("Halo");
        userControl = charController.GetComponent<UserControlThirdPerson>();
        /*
        playerUI = GameObject.FindGameObjectWithTag("playerUI");
        if(playerUI)
        {
            playerUI.GetComponent<userInterface>().SetCoolDownTime(specialAttackCooldownTime);
        }*/
        coolDownTime = 2000f;

        playerRigidBody = baseStats.pelvisJoint.GetComponent<Rigidbody>();
        PowerUp = false;
        isGrounded = checkIfGrounded();
        specialStartSize = new Vector3(0.2f, 0.2f, 0.2f);
        specialEndSize = new Vector3(specialFormSize, specialFormSize, specialFormSize);
        specialForm.GetComponent<MeshRenderer>().enabled = false;
        specialForm.GetComponent<BoxCollider>().enabled = false;
        specialRigid = specialForm.GetComponent<Rigidbody>();
        coolDownTimer = 0f;
        specialRigid.useGravity = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!baseStats.IsDead)
        {
            if (updateCollisionCheck)
            {
                //ASSUMES ALL CHARACTER ARMS AND LEGS ARE 3 JOINTS
                GameObject walker = leftShoulder;
                GameObject walker2 = rightShoulder; //need both for sake of combo
                GameObject walker3 = rightThigh;
                while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
                {
                    walker.GetComponent<CollisionReceived>().sendDamage = true;
                    walker2.GetComponent<CollisionReceived>().sendDamage = true;
                    walker3.GetComponent<CollisionReceived>().sendDamage = true;
                    //assumes there is only one child
                    walker = walker.transform.GetChild(0).gameObject;
                    walker2 = walker2.transform.GetChild(0).gameObject;
                    walker3 = walker3.transform.GetChild(0).gameObject;
                }
                if (walker.GetComponent<CollisionReceived>() != null && walker2.GetComponent<CollisionReceived>() != null && walker3.GetComponent<CollisionReceived>() != null)
                {
                    walker.GetComponent<CollisionReceived>().sendDamage = true;
                    walker2.GetComponent<CollisionReceived>().sendDamage = true;
                    walker3.GetComponent<CollisionReceived>().sendDamage = true;
                }

                updateCollisionCheck = false;
            }

            isGrounded = checkIfGrounded();
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
                if (((Input.GetKeyDown(specialAttack) && !IsPlayer2) || Input.GetButtonDown(specialAttackButton)))
                {
                    DeactivateSpecialAttack();
                    UpdatePos(charController.transform, specialForm.transform);
                    coolDownTimer = 0f;
                }
                if ((Input.GetKeyDown(useAttack) || Input.GetButtonDown(activateSpecialAttackButton)) && isGrounded && specialForm.GetComponent<MeshRenderer>().enabled) //include jump key for controller
                {
                    specialRigid.AddForce(Vector3.up * specialAttackForce * 100f);
                    SendMessage("CubeJumpSfx", true, SendMessageOptions.DontRequireReceiver);
                }
                else if ((Input.GetKeyDown(useAttack) || Input.GetButtonDown(activateSpecialAttackButton)) && specialForm.GetComponent<MeshRenderer>().enabled && !launched)
                {
                    specialRigid.AddForce(-Vector3.up * specialAttackForce * 300f);
                    launched = true;
                    SendMessage("CubeStompSfx", true, SendMessageOptions.DontRequireReceiver);
                }
                if (!isGrounded)
                {
                    moveDir = new Vector3(Input.GetAxisRaw(horizontalStick), 0, Input.GetAxisRaw(verticalStick));
                    moveDir = cam.transform.TransformDirection(moveDir);
                    moveDir.y = 0;
                    moveDir = Vector3.Normalize(moveDir);
                    moveDir.x = moveDir.x * specialAttackForce * baseStats.GetPlayerSpeed() * Time.deltaTime;
                    moveDir.z = moveDir.z * specialAttackForce * baseStats.GetPlayerSpeed() * Time.deltaTime;
                    specialRigid.AddForce(moveDir);
                }
            }
            else
            {
                UpdatePos(specialForm.transform, charController.transform);

                if (((Input.GetKeyDown(specialAttack) && !IsPlayer2) || Input.GetButtonDown(specialAttackButton)) && !specialForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
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

        }
        //GrowBigPower();
    }

    /*
     * Overriding these functions as a way to turn off collisions for the arms and 
     * legs when fighting.
     */
    public override void ThrowSinglePunch(Limbs limb)
    {
        base.ThrowSinglePunch(limb);
        if (limb == Limbs.leftArm)
        {
            GameObject walker = leftShoulder;
            GameObject walker2 = rightShoulder; //need both for sake of combo
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
                walker2 = walker2.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null && walker2.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
        else
        {
            GameObject walker = rightShoulder;
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
    }

    public override void ThrowHiKick()
    {
        base.ThrowHiKick();
        GameObject walker = rightThigh;
        while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
        {
            walker.GetComponent<CollisionReceived>().sendDamage = false;
            //assumes there is only one child
            walker = walker.transform.GetChild(0).gameObject;
        }
        if (walker.GetComponent<CollisionReceived>() != null)
        {
            walker.GetComponent<CollisionReceived>().sendDamage = false;
        }
    }

    public override void ThrowUppercut(Limbs limb)
    {
        base.ThrowUppercut(limb);
        if (limb == Limbs.leftArm)
        {
            GameObject walker = leftShoulder;
            GameObject walker2 = rightShoulder; //need both for sake of combo
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
                walker2 = walker2.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null && walker2.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
        else
        {
            GameObject walker = rightShoulder;
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
            }

            if (walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
    }

    /// <summary>
    /// Allows character to grow larger when attack key is pressed. Shrinks back down after a certain
    /// amount of time.
    /// </summary>
    private void GrowBigPower()
    {
        if ((!PowerUp && Input.GetKeyDown(useAttack)) || (!PowerUp && Input.GetButtonDown("AButton")))
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
                onCooldown = true;
            }
        }
    }

    public override void SetAsPlayer2()
    {
        base.SetAsPlayer2();
        leftJabControllerButton = leftJabControllerButton + "_2";
        rightJabControllerButton = rightJabControllerButton + "_2";
        upperCutButton = upperCutButton + "_2";
        hiKickButton = hiKickButton + "_2";
        specialAttackButton = specialAttackButton + "_2";
        activateSpecialAttackButton = activateSpecialAttackButton + "_2";
        horizontalStick += "_2";
        verticalStick += "_2";
    }
}


