using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAttackScript : PunchScript
{
    public GameObject ballForm;
    public KeyCode ballFormKey = KeyCode.LeftControl;

    private float ballTime;
    private float timeAsBall;
    private SphereSpecialStats stats;
    private Rigidbody ballRigid;
    private float moveVer;
    private float moveHor;
    private float ballForce;

    // Use this for initialization
    void Start()
    {
        stats = this.GetComponent<SphereSpecialStats>();
        ballRigid = ballForm.GetComponent<Rigidbody>();
        ballForm.SetActive(false);
        ballTime = 10.0f;
        ballForce = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(ballFormKey))
        {
            if(!ballForm.activeSelf) //make this have a cooldown
            {
                ActivateRollAttack();
            }
            else
            {
                DeactivateRollAttack();
            }
        }
        if (ballForm.activeSelf)
        {
            //use different timer at some point
            timeAsBall += Time.deltaTime;
            if(timeAsBall > ballTime)
            {
                DeactivateRollAttack();
                timeAsBall = 0;
            }
            moveHor = Input.GetAxis("Horizontal") * ballForce * stats.GetPlayerSpeed();
            moveVer = Input.GetAxis("Vertical") * ballForce * stats.GetPlayerSpeed();

            ballRigid.AddForce(new Vector3(moveHor, 0, moveVer));
        }
    }

    public void DeactivateRollAttack()
    {
        //play animation of morphing into ball
        for (int i = 0; i < this.transform.childCount; i++) //move camera back to player here
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
        ballForm.SetActive(false);
    }

    public void ActivateRollAttack()
    {
        //play animation of morphing into ball
        for(int i =0; i < this.transform.childCount; i++)
        {
            if(this.transform.GetChild(i).gameObject.tag != "MainCamera") //move camera to follow ball here
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        ballForm.SetActive(true);
    }
}
