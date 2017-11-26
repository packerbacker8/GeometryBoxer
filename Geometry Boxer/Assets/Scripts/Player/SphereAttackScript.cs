using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Demos;

public class SphereAttackScript : PunchScript
{
    public GameObject ballForm;
    public KeyCode ballFormKey = KeyCode.LeftControl;

    private float ballTime;
    private float ballCooldownTime;
    private float timeAsBall;
    private float cooldownTime;
    private SphereSpecialStats stats;
    private Rigidbody ballRigid;
    private float moveVer;
    private float moveHor;
    private float ballForce;
    private bool onCooldown;
    private bool isBall;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        stats = this.GetComponent<SphereSpecialStats>();
        ballRigid = ballForm.GetComponent<Rigidbody>();
        ballForm.GetComponent<MeshRenderer>().enabled = false;
        ballForm.GetComponent<SphereCollider>().enabled = false;
        ballTime = 10.0f;
        ballForce = 100f;
        ballCooldownTime = 2.0f;
        timeAsBall = 0;
        cooldownTime = 0f;
        onCooldown = false;
        isBall = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(!onCooldown)
        {
            if (Input.GetKeyDown(ballFormKey))
            {
                if (!isBall) //make this have a cooldown
                {
                    ActivateRollAttack();
                }
                else
                {
                    Debug.Log("char position " + charController.transform.position);
                    DeactivateRollAttack();
                    UpdatePos(charController.transform, ballForm.transform);
                    Debug.Log("char position " + charController.transform.position);

                }
            }
            if (isBall)
            {
                //use different timer at some point
                timeAsBall += Time.deltaTime;
                if (timeAsBall > ballTime)
                {
                    DeactivateRollAttack();
                }
                moveHor = Input.GetAxis("Horizontal") * ballForce * stats.GetPlayerSpeed();
                moveVer = Input.GetAxis("Vertical") * ballForce * stats.GetPlayerSpeed();

                ballRigid.AddForce(new Vector3(moveHor, 0, moveVer));
                UpdatePos(charController.transform, ballForm.transform);
            }
            else
            {
                UpdatePos(ballForm.transform, charController.transform);
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
    }

    public void UpdatePos(Transform transformToUpdate, Transform targetTransform)
    {
        Vector3 targetVec = targetTransform.position;
        if(transformToUpdate == ballForm.transform)
        {
            transformToUpdate.localRotation = Quaternion.identity;
            targetVec = new Vector3(targetVec.x, targetVec.y + 1.5f, targetVec.z);
        }
        transformToUpdate.position = targetVec;
    }

    public void DeactivateRollAttack()
    {
        //play animation of morphing into ball
        isBall = false;
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
        onCooldown = true;
    }

    public void ActivateRollAttack()
    {
        isBall = true;
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
    }
}
