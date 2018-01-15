using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Demos;

public class CubeAttackScript : PunchScript
{
    public GameObject pelvisJoint;
    public float PowerUpTimeLimit = 10f;
    public KeyCode attacKey = KeyCode.LeftControl;
    public GameObject cubeForm;

    private Rigidbody playerRigidBody;
    private PlayerHealthScript HealthScript;
    public bool PowerUp = false;
    private float TimePowerUp;
    private Behaviour halo;
    private bool isGrounded;
    private CharacterMeleeDemo charMelDemo;
    private GameObject puppetMast;
    private GameObject gameController;
    private CubeSpecialStats stats;

    private Vector3 startCubeSize;
    private Vector3 endCubeSize;
    private Vector3 moveDir;
    private Rigidbody cubeRigid;

    private bool onCooldown;
    private bool growingCube;

    private float coolDownTimer;
    private float cubeForce;

    // This is puppetMasters user controler, it controls the players movements
    protected UserControlThirdPerson userControl; // user input


    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        stats = this.GetComponent<CubeSpecialStats>();
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController");
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        halo = (Behaviour)charController.GetComponent("Halo");
        userControl = charController.GetComponent<UserControlThirdPerson>();
        playerRigidBody = pelvisJoint.GetComponent<Rigidbody>();
        charMelDemo = this.transform.GetComponentInChildren<CharacterMeleeDemo>();
        isGrounded = charMelDemo.animState.onGround;

        PowerUp = false;

        startCubeSize = new Vector3(0.1f, 0.1f, 0.1f);
        endCubeSize = new Vector3(2f, 2f, 2f);
        cubeForm.GetComponent<MeshRenderer>().enabled = false;
        cubeForm.GetComponent<BoxCollider>().enabled = false;
        cubeRigid = cubeForm.GetComponent<Rigidbody>();
        onCooldown = false;
        growingCube = false;
        coolDownTimer = 0f;
        cubeForce = 10f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        isGrounded = checkIfGrounded();
        
        if(growingCube)
        {
            cubeForm.transform.localScale += startCubeSize;
            if(cubeForm.transform.localScale.magnitude >= endCubeSize.magnitude)
            {
                cubeForm.transform.localScale = endCubeSize;
                growingCube = false;
            }
        }
        else if(cubeForm.GetComponent<MeshRenderer>().enabled)
        {
            Debug.Log("If i am on the ground: " + isGrounded);
            UpdatePos(charController.transform, cubeForm.transform);
            coolDownTimer += Time.deltaTime;
            if(coolDownTimer >= PowerUpTimeLimit)
            {
                DeactivateBoxAttack();
                UpdatePos(charController.transform, cubeForm.transform);
                coolDownTimer = 0f;
            }
            if ((Input.GetKeyDown(attacKey) || Input.GetButtonDown("XButton")))
            {
                DeactivateBoxAttack();
                UpdatePos(charController.transform, cubeForm.transform);
                coolDownTimer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && cubeForm.GetComponent<MeshRenderer>().enabled) //include jump key for controller
            {
                cubeRigid.GetComponent<Rigidbody>().AddForce(Vector3.up * cubeForce * 100f);
            }

            if(!isGrounded)
            {
                moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                moveDir = cam.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir = Vector3.Normalize(moveDir);
                moveDir.x = moveDir.x * cubeForce * stats.GetPlayerSpeed();
                moveDir.z = moveDir.z * cubeForce * stats.GetPlayerSpeed();
                cubeRigid.AddForce(moveDir);
            }
        }
        else
        {
            UpdatePos(cubeForm.transform, charController.transform);

            if ((Input.GetKeyDown(attacKey) || Input.GetButtonDown("XButton")) && !cubeForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
            {
                growingCube = true;
                ActivateBoxAttack();
                UpdatePos(cubeForm.transform, charController.transform);
            }
        }
        
        if(onCooldown)
        {
            coolDownTimer += Time.deltaTime;
            if(coolDownTimer >= PowerUpTimeLimit)
            {
                onCooldown = false;
                coolDownTimer = 0f;
            }
        }

        //GrowBigPower();
    }

    private void GrowBigPower()
    {
        if (!PowerUp && Input.GetKeyDown(attacKey) || !PowerUp && Input.GetButtonDown("XButton"))
        {
            PowerUp = true;
            halo.enabled = true;
            SendMessage("PowerUpActive", true);

            puppetMast.transform.localScale += new Vector3(2F, 2F, 2F);
            charController.transform.localScale += new Vector3(2F, 2F, 2F);

        }



        if (PowerUp)
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

    private bool checkIfGrounded()
    {
        return Physics.Raycast(cubeForm.transform.position, -Vector3.up, cubeForm.GetComponent<BoxCollider>().size.y + 0.1f);
    }


    private void UpdatePos(Transform transformToUpdate, Transform targetTransform)
    {
        Vector3 targetVec = targetTransform.position;
        if (transformToUpdate == cubeForm.transform)
        {
            transformToUpdate.rotation = Quaternion.identity;
            targetVec = new Vector3(targetVec.x, targetVec.y + 1f, targetVec.z);
        }
        transformToUpdate.position = targetVec;
    }

    private void DeactivateBoxAttack()
    {
        UpdatePos(charController.transform, cubeForm.transform);
        cubeForm.transform.localScale = startCubeSize;
        //play animation of morphing into ball
        isAttacking = false;
        for (int i = 0; i < this.transform.childCount; i++) //move camera back to player here
        {
            if (this.transform.GetChild(i).gameObject != cubeForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = stats.pelvisJoint.transform;
                }
                else if (this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        //ballForm.SetActive(false);
        cubeForm.GetComponent<MeshRenderer>().enabled = false;
        cubeForm.GetComponent<BoxCollider>().enabled = false;
        charController.GetComponent<UserControlMelee>().enabled = true;
        charController.GetComponent<CharacterMeleeDemo>().enabled = true;
        charController.GetComponent<CapsuleCollider>().enabled = true;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        onCooldown = true;
        anim.SetInteger("ActionIndex", -1);
        anim.SetBool("IsStrafing", false);
        if (cubeRigid.velocity.sqrMagnitude > 0)
        {
            anim.SetFloat("Forward", 1);
        }
        else
        {
            anim.SetFloat("Forward", 0);
        }
        anim.Play("Grounded Directional");
        SendMessage("PowerUpDeactivated");
    }

    private void ActivateBoxAttack()
    {
        leftFistCollider.radius = leftFistStartSize.radius;
        leftFistCollider.height = leftFistStartSize.height;
        rightFistCollider.radius = rightFistStartSize.radius;
        rightFistCollider.height = rightFistStartSize.height;
        UpdatePos(cubeForm.transform, charController.transform);
        isAttacking = true;
        //play animation of morphing into ball
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject != cubeForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = cubeForm.transform;
                }
                else if (this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        cubeForm.SetActive(true);
        charController.SetActive(true);
        cubeForm.GetComponent<MeshRenderer>().enabled = true;
        cubeForm.GetComponent<BoxCollider>().enabled = true;
        charController.GetComponent<UserControlMelee>().enabled = false;
        charController.GetComponent<CharacterMeleeDemo>().enabled = false;
        charController.GetComponent<CapsuleCollider>().enabled = false;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        SendMessage("PowerUpActive");
    }
}


