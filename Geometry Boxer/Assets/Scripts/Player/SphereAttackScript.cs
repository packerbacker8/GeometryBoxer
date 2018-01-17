using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Demos;

public class SphereAttackScript : PunchScript
{
    public GameObject ballForm;
    public GameObject ballShield;
    public KeyCode ballFormKey = KeyCode.LeftControl;

    private float ballTime;
    private float ballCooldownTime;
    private float timeAsBall;
    private float cooldownTime;
    private float maxVelocity;
    private SphereSpecialStats stats;
    private Rigidbody ballRigid;
    private Rigidbody pelvisRigid;
    private float moveVer;
    private float moveHor;
    private Vector3 moveDir;
    private float ballForce;
    private bool onCooldown;
    private bool isBall;

    private CapsuleCollider rightHandCol;
    private CapsuleCollider leftHandCol;
    private float originalRightHandRadius;
    private float originalLeftHandRadius;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        stats = this.GetComponent<SphereSpecialStats>();
        ballRigid = ballForm.GetComponent<Rigidbody>();
        pelvisRigid = stats.pelvisJoint.GetComponent<Rigidbody>();
        ballForm.GetComponent<MeshRenderer>().enabled = false;
        ballForm.GetComponent<SphereCollider>().enabled = false;
        ballShield.gameObject.SetActive(false);
        ballTime = 10.0f;
        ballForce = 1000f;
        ballCooldownTime = 2.0f;
        timeAsBall = 0;
        cooldownTime = 0f;
        maxVelocity = 25f;
        onCooldown = false;
        isBall = false;
        ballRigid.useGravity = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
  
        if (!onCooldown)
        {
            if (Input.GetKeyDown(ballFormKey))
            {
                if (!isBall) //make this have a cooldown
                {
                    ActivateRollAttack();
                    UpdatePos(ballForm.transform, charController.transform);
                }
                else
                {
                    DeactivateRollAttack();
                    UpdatePos(charController.transform, ballForm.transform);
                    charController.transform.position = new Vector3(charController.transform.position.x, charController.transform.position.y + 1.0f, charController.transform.position.z);
                }
            }
            if (isBall)
            {
                
                //use different timer at some point
                timeAsBall += Time.deltaTime;
                if (timeAsBall > ballTime)
                {
                    DeactivateRollAttack();
                    charController.transform.position = new Vector3(charController.transform.position.x, charController.transform.position.y + 1.0f, charController.transform.position.z);
                }
                
                if (Math.Abs(ballRigid.velocity.magnitude) <= maxVelocity) //attempt to limit ball from going too fast
                {
                    //moveHor =  * ballForce * stats.GetPlayerSpeed();
                    //moveVer =  * ballForce * stats.GetPlayerSpeed();
                    moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                    moveDir = cam.transform.TransformDirection(moveDir);
                    moveDir.y = 0;
                    moveDir = Vector3.Normalize(moveDir);
                    moveDir.x = moveDir.x * ballForce * stats.GetPlayerSpeed();
                    moveDir.z = moveDir.z * ballForce * stats.GetPlayerSpeed();
                    ballRigid.AddForce(moveDir);
                }
                UpdatePos(charController.transform, ballForm.transform);
                UpdatePos(ballShield.transform, ballForm.transform);
            }
            else
            {
                UpdatePos(ballForm.transform, charController.transform);
                ballRigid.velocity = pelvisRigid.velocity;
            }
        }
        else
        {
            cooldownTime += Time.deltaTime;
            if(cooldownTime > ballCooldownTime)
            {
                onCooldown = false;
                cooldownTime = 0;
            }
            UpdatePos(ballForm.transform, charController.transform);
        }

        //rightHandCol.radius = originalRightHandRadius;

    }

    public void UpdatePos(Transform transformToUpdate, Transform targetTransform)
    {
        Vector3 targetVec = targetTransform.position;
        if(transformToUpdate == ballForm.transform)
        {
            transformToUpdate.rotation = Quaternion.identity;
            targetVec = new Vector3(targetVec.x, targetVec.y + 2f, targetVec.z);
        }
        transformToUpdate.position = targetVec;
    }

    public void DeactivateRollAttack()
    {
        ballRigid.useGravity = false;
        UpdatePos(charController.transform, ballForm.transform);
        //play animation of morphing into ball
        isBall = false;
        isAttacking = false;
        timeAsBall = 0;
        for (int i = 0; i < this.transform.childCount; i++) //move camera back to player here
        {
            if (this.transform.GetChild(i).gameObject != ballForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = stats.pelvisJoint.transform;
                }
                else if(this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        //ballForm.SetActive(false);
        ballForm.GetComponent<MeshRenderer>().enabled = false;
        ballForm.GetComponent<SphereCollider>().enabled = false;
        charController.GetComponent<UserControlMelee>().enabled = true;
        charController.GetComponent<CharacterMeleeDemo>().enabled = true;
        charController.GetComponent<CapsuleCollider>().enabled = true;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        ballShield.gameObject.SetActive(false);
        onCooldown = true;
        anim.SetInteger("ActionIndex", -1);
        anim.SetBool("IsStrafing", false);
        if(ballRigid.velocity.sqrMagnitude > 0)
        {
            anim.SetFloat("Forward", 1);
        }
        else
        {
            anim.SetFloat("Forward", 0);
        }
        anim.Play("Grounded Directional");
        SendMessage("BallDeactivatedSfx");
    }

    public void ActivateRollAttack()
    {
        ballRigid.useGravity = true;
        leftFistCollider.radius = leftFistStartSize.radius;
        leftFistCollider.height = leftFistStartSize.height;
        rightFistCollider.radius = rightFistStartSize.radius;
        rightFistCollider.height = rightFistStartSize.height;
        UpdatePos(ballForm.transform, charController.transform);
        isBall = true;
        isAttacking = true;
        //play animation of morphing into ball
        for(int i =0; i < this.transform.childCount; i++)
        {
            if(this.transform.GetChild(i).gameObject != ballForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = ballForm.transform;
                }
                else if(this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        ballForm.SetActive(true);
        charController.SetActive(true);
        ballForm.GetComponent<MeshRenderer>().enabled = true;
        ballForm.GetComponent<SphereCollider>().enabled = true;
        charController.GetComponent<UserControlMelee>().enabled = false;
        charController.GetComponent<CharacterMeleeDemo>().enabled = false;
        charController.GetComponent<CapsuleCollider>().enabled = false;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        ballShield.gameObject.SetActive(true);
        UpdatePos(ballShield.transform, charController.transform);
        SendMessage("BallActivatedSfx");
    }
}
