using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class PunchScript : MonoBehaviour
{
    public bool useController = true;
    public Rigidbody leftArm;
    public Rigidbody rightArm;
    public float punchForce = 50f;
    [Header("PC punch buttons.")]
    public KeyCode leftPunchKey = KeyCode.Q;
    public KeyCode rightPunchKey = KeyCode.E;
    [Header("Controller punch buttons.")]
    public string leftPunchControllerButton = "LeftBumper";
    public string rightPunchControllerButton = "RightBumper";

    /// <summary>
    /// Private variables for controls in punching and moving arms.
    /// </summary>
    private bool controllingLeftArm;
    private bool controllingRightArm;
    private bool leftGrab;
    private bool rightGrab;
   
    private float leftArmXAxis;
    private float leftArmYAxis;
    private float rightArmXAxis;
    private float rightArmYAxis;

    private Animator anim;
    private int characterControllerIndex = 2;
    private int animationControllerIndex = 0;
    private int puppetMasterIndex = 1;
    private int numberOfMuscleComponents;
    private int swingAnimLayer = 1;
    private int punchAnimLayer = 0;

    private string leftPunchAnimation = "Hit";
    private string rightPunchAnimation = "Hit";
    private string leftSwingAnimation = "SwingProp";
    private string rightSwingAnimation = "SwingProp";
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string fall = "Fall";

    private GameObject puppetMast;

    private enum Limbs
    {
        leftArm = 0,
        rightArm
    };

    // Use this for initialization
    void Start()
    {
        controllingLeftArm = false;
        controllingRightArm = false;
        leftGrab = false;
        rightGrab = false;
        leftArmXAxis = 0f;
        leftArmYAxis = 0f;
        rightArmXAxis = 0f;
        rightArmYAxis = 0f;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        numberOfMuscleComponents = puppetMast.GetComponent<PuppetMaster>().muscles.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfMuscleComponents < puppetMast.GetComponent<PuppetMaster>().muscles.Length) //number of muscles increased from beginning, a prop has been picked up
        {
            rightGrab = true;
        }
        else //number of muscles is the same  or less, i.e. prop lost
        {
            rightGrab = false;
        }
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall)) //prevent use of your arms when you are on the ground and getting up.
        {
            if (useController) //controller controls
            {
                //Left trigger lets player control left arm independently
                if (Input.GetAxis("LeftTrigger") > 0)
                {
                    controllingLeftArm = true;
                    //this.transform.gameObject.SendMessage("UsingLeftArm", controllingLeftArm);
                }
                else
                {
                    controllingLeftArm = false;
                    //this.transform.gameObject.SendMessage("UsingLeftArm", controllingLeftArm);
                }
                //Right trigger lets player control right arm independently
                if (Input.GetAxis("RightTrigger") > 0)
                {
                    controllingRightArm = true;
                }
                else
                {
                    controllingRightArm = false;
                }
                //Left arm punching
                if (controllingLeftArm)
                {
                    leftArmXAxis = Input.GetAxisRaw("HorizontalLeft");
                    leftArmYAxis = Input.GetAxisRaw("VerticalLeft");
                    MoveArm(Limbs.leftArm, leftArmXAxis, leftArmYAxis);
                }
                else if (Input.GetButtonDown(leftPunchControllerButton)) //left bumper
                {
                    ThrowSinglePunch(Limbs.leftArm);
                }
                //Right arm punching
                if (controllingRightArm)
                {
                    rightArmXAxis = Input.GetAxisRaw("HorizontalRight");
                    rightArmYAxis = Input.GetAxisRaw("VerticalRight");
                    MoveArm(Limbs.rightArm, rightArmXAxis, rightArmYAxis);
                }
                else if (Input.GetButtonDown(rightPunchControllerButton))
                {
                    ThrowSinglePunch(Limbs.rightArm);
                }

            }
            else  // keyboard controls
            {
                //Left trigger lets player control left arm independently
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    controllingLeftArm = true;
                    //this.transform.gameObject.SendMessage("UsingLeftArm", controllingLeftArm);
                }
                else
                {
                    controllingLeftArm = false;
                    //this.transform.gameObject.SendMessage("UsingLeftArm", controllingLeftArm);
                }
                //Right trigger lets player control right arm independently
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    controllingRightArm = true;
                }
                else
                {
                    controllingRightArm = false;
                }

                if (controllingLeftArm)
                {
                    leftArmXAxis = Input.GetAxisRaw("Mouse X");
                    leftArmYAxis = Input.GetAxisRaw("Mouse Y");
                    MoveArm(Limbs.leftArm, leftArmXAxis, leftArmYAxis);
                }
                else if (Input.GetKeyDown(leftPunchKey))
                {
                    ThrowSinglePunch(Limbs.leftArm);

                }
                if (controllingRightArm)
                {
                    rightArmXAxis = Input.GetAxisRaw("Mouse X");
                    rightArmYAxis = Input.GetAxisRaw("Mouse Y");
                    MoveArm(Limbs.rightArm, rightArmXAxis, rightArmYAxis);
                }
                else if (Input.GetKeyDown(rightPunchKey))
                {
                    ThrowSinglePunch(Limbs.rightArm);

                }
            }
        }
        else
        {
            //do something if on the ground
        }
    }

    /// <summary>
    /// Cause player to throw single punch of one arm.  Arm is determined by limb parameter.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    private void ThrowSinglePunch(Limbs limb)
    {
        //Rigidbody armToMove = null;
        if(limb == Limbs.leftArm)
        {
            //armToMove = leftArm;
            if(leftGrab)
            {
                anim.Play(leftSwingAnimation, swingAnimLayer);
            }
            else
            {
                anim.Play(leftPunchAnimation, punchAnimLayer);
            }
        }
        else if(limb == Limbs.rightArm)
        {
            //armToMove = rightArm;
            if(rightGrab)
            {
                anim.Play(rightSwingAnimation, swingAnimLayer);
            }
            else
            {
                anim.Play(rightPunchAnimation, punchAnimLayer);
            }
        }
        //armToMove.AddForce((this.transform.forward + this.transform.up) * punchForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Allows player to move limb about as they choose with the controller joysticks.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    /// <param name="xMotion">Gets axis value from input for left and right sticks. In horizontal direction.</param>
    /// <param name="yMotion">Gets axis value from input for left and right sticks. In vertical direction.</param>
    private void MoveArm(Limbs limb, float xMotion, float yMotion)
    {
        Rigidbody armToMove = null;
        if (limb == Limbs.leftArm)
        {
            armToMove = leftArm;
        }
        else if (limb == Limbs.rightArm)
        {
            armToMove = rightArm;
        }
        Vector3 directionToMove = new Vector3(xMotion, yMotion);
        armToMove.AddForce(directionToMove * punchForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Grab script on left hand tells us if something has been grabbed or dropped.
    /// </summary>
    /// <param name="isGrabbed">Boolean to say if something is in hand or not.</param>
    public void ObjectGrabbedLeft(bool isGrabbed)
    {
        leftGrab = isGrabbed;
    }
}
