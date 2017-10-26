using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class PunchScript : MonoBehaviour
{
    [Tooltip("Is there a controller plugged in.")]
    public bool useController = true;
    [Header("Punching Components")]
    [Tooltip("Arm designated as the left arm.  This will move it by its rigidbody.")]
    public Rigidbody leftArm;
    [Tooltip("Arm designated as the right arm.  This will move it by its rigidbody.")]
    public Rigidbody rightArm;
    [Tooltip("The force by which the rigidbody will move.")]
    public float punchForce = 50f;
    [Header("PC punch buttons.")]
    public KeyCode leftJabKey = KeyCode.Q;
    public KeyCode rightJabKey = KeyCode.E;
    //public KeyCode leftPunchKey = KeyCode.Q;
    //public KeyCode rightPunchKey = KeyCode.E;
    //public KeyCode leftPunchKey = KeyCode.Q;
    //public KeyCode rightPunchKey = KeyCode.E;
    [Header("Controller punch buttons.")]
    public string leftJabControllerButton = "LeftBumper";
    public string rightJabControllerButton = "RightBumper";

    /// <summary>
    /// Private variables for controls in punching and moving arms.
    /// </summary>
    private bool controllingLeftArm; //controlling left arm or right arm is the same as disabling puppet master control
    private bool controllingRightArm;
    private bool leftGrab;
    private bool rightGrab;
    private bool movementAndCameraDisabled;

    private float leftArmXAxis;
    private float leftArmYAxis;
    private float rightArmXAxis;
    private float rightArmYAxis;
    private float marginOfError;
    private float currentX;
    private float currentY;
    private float oldInputX;
    private float oldInputY;

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
    private string onGround = "OnGround";

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
        movementAndCameraDisabled = false;
        leftArmXAxis = 0f;
        leftArmYAxis = 0f;
        rightArmXAxis = 0f;
        rightArmYAxis = 0f;
        marginOfError = 0.5f;
        currentX = 0f;
        currentY = 0f;
        oldInputX = 0f;
        oldInputY = 0f;
        anim = this.transform.GetChild(characterControllerIndex).gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMast = this.transform.GetChild(puppetMasterIndex).gameObject;
        numberOfMuscleComponents = puppetMast.GetComponent<PuppetMaster>().muscles.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfMuscleComponents < puppetMast.GetComponent<PuppetMaster>().muscles.Length) //number of muscles increased from beginning, a prop has been picked up
        {
            rightGrab = true;
        }
        else //number of muscles is the same  or less, i.e. prop lost
        {
            rightGrab = false;
        }
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && anim.GetBool(onGround)) //prevent use of your arms when you are on the ground and getting up.
        {
            if (useController) //controller controls
            {
                //Left trigger lets player control left arm independently
                if (Input.GetAxis("LeftTrigger") > 0)
                {
                    controllingLeftArm = true;
                }
                else
                {
                    controllingLeftArm = false;
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
                if ((controllingLeftArm || controllingRightArm) && !movementAndCameraDisabled)
                {
                    //turn off camera movement and character movement
                    StartCoroutine(ToggleCameraAndMovement(true));
                }
                else if (movementAndCameraDisabled)
                {
                    StartCoroutine(ToggleCameraAndMovement(false));
                }
                //Left arm punching
                if (controllingLeftArm)
                {
                    leftArmXAxis = Input.GetAxisRaw("HorizontalLeft");
                    leftArmYAxis = Input.GetAxisRaw("VerticalLeft");
                    float xSquare = (leftArmXAxis - oldInputX) * (leftArmXAxis - oldInputX);
                    float ySquare = (leftArmYAxis - oldInputY) * (leftArmYAxis - oldInputY);
                    if (System.Math.Sqrt(xSquare + ySquare) > marginOfError)
                    {
                        MoveArm(Limbs.leftArm, leftArmXAxis, leftArmYAxis);
                    }
                    oldInputX = leftArmXAxis;
                    oldInputY = leftArmYAxis;
                }
                else if (Input.GetButtonDown(leftJabControllerButton)) //left bumper
                {
                    ThrowSinglePunch(Limbs.leftArm);
                }
                //Right arm punching
                if (controllingRightArm)
                {
                    rightArmXAxis = Input.GetAxisRaw("HorizontalRight");
                    rightArmYAxis = Input.GetAxisRaw("VerticalRight");
                    float xSquare = (rightArmXAxis - oldInputX) * (rightArmXAxis - oldInputX);
                    float ySquare = (rightArmYAxis - oldInputY) * (rightArmYAxis - oldInputY);
                    if (System.Math.Sqrt(xSquare + ySquare) > marginOfError)
                    {
                        MoveArm(Limbs.rightArm, rightArmXAxis, rightArmYAxis);
                    }
                    oldInputX = rightArmXAxis;
                    oldInputY = rightArmYAxis;
                }
                else if (Input.GetButtonDown(rightJabControllerButton))
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
                }
                else
                {
                    controllingLeftArm = false;
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
                if((controllingLeftArm || controllingRightArm) && !movementAndCameraDisabled)
                {
                    //turn off camera movement and character movement
                    StartCoroutine(ToggleCameraAndMovement(true));
                }
                else if(movementAndCameraDisabled)
                {
                    StartCoroutine(ToggleCameraAndMovement(false));
                }
                if (controllingLeftArm)
                {
                    leftArmXAxis = Input.GetAxisRaw("Mouse X");
                    leftArmYAxis = Input.GetAxisRaw("Mouse Y");
                    //currentX = Input.mousePosition.x;
                    //currentY = Input.mousePosition.y;
                    float xSquare = (leftArmXAxis - oldInputX) * (leftArmXAxis - oldInputX);
                    float ySquare = (leftArmYAxis - oldInputY) * (leftArmYAxis - oldInputY);
                    if (System.Math.Sqrt(xSquare + ySquare) > marginOfError)
                    {
                        //leftArmXAxis = (currentX - halfScreenWidth) / halfScreenWidth;
                        //leftArmYAxis = (currentY - halfScreenHeight) / halfScreenHeight;
                        MoveArm(Limbs.leftArm, leftArmXAxis, leftArmYAxis);
                    }

                    oldInputX = currentX;
                    oldInputY = currentY;
                }
                else if (Input.GetKeyDown(leftJabKey))
                {
                    ThrowSinglePunch(Limbs.leftArm);
                }
                if (controllingRightArm)
                {
                    rightArmXAxis = Input.GetAxisRaw("Mouse X");
                    rightArmYAxis = Input.GetAxisRaw("Mouse Y");
                    //currentY = Input.mousePosition.y;
                    //currentX = Input.mousePosition.x;
                    float xSquare = (rightArmXAxis - oldInputX) * (rightArmXAxis - oldInputX);
                    float ySquare = (rightArmYAxis - oldInputY) * (rightArmYAxis - oldInputY);
                    if (System.Math.Sqrt(xSquare + ySquare) > marginOfError)
                    {
                        //rightArmXAxis = (currentX - halfScreenWidth) / halfScreenWidth;
                        //rightArmYAxis = (currentY - halfScreenHeight) / halfScreenHeight;
                        MoveArm(Limbs.rightArm, rightArmXAxis, rightArmYAxis);
                    }

                    oldInputX = currentX;
                    oldInputY = currentY;
                }
                else if (Input.GetKeyDown(rightJabKey))
                {
                    ThrowSinglePunch(Limbs.rightArm);
                }
            }
        }
        else
        {
            //do something if on the ground, ground combat
        }
    }

    /// <summary>
    /// Cause player to throw single punch of one arm.  Arm is determined by limb parameter.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    private void ThrowSinglePunch(Limbs limb)
    {
        if (limb == Limbs.leftArm)
        {
            if (leftGrab)
            {
                anim.Play(leftSwingAnimation, swingAnimLayer);
            }
            else
            {
                anim.Play(leftPunchAnimation, punchAnimLayer);
            }
        }
        else if (limb == Limbs.rightArm)
        {
            if (rightGrab)
            {
                anim.Play(rightSwingAnimation, swingAnimLayer);
            }
            else
            {
                anim.Play(rightPunchAnimation, punchAnimLayer);
            }
        }
    }

    /// <summary>
    /// Allows player to move limb about as they choose with the controller joysticks.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    /// <param name="xMotion">Gets axis value from input for left and right sticks. In horizontal direction.</param>
    /// <param name="yMotion">Gets axis value from input for left and right sticks. In vertical direction.</param>
    private void MoveArm(Limbs limb, float xMotion, float yMotion)
    {
        //use camera's forward to determine the forward (z direction) of the movement
        Rigidbody armToMove = null;
        xMotion = System.Math.Abs(xMotion);  //make go the right direction, so arm doesn't go through body
        Vector2 forceOfMove = new Vector2(xMotion, System.Math.Abs(yMotion)); //the force multiplied to the punch
        float sum = xMotion + System.Math.Abs(yMotion);
        if (sum == 0f)
        {
            return;
        }
        float upMotion = yMotion / sum; //ratio of y to x
        float sideMotion = xMotion / sum; //ratio of x to y
        float forwardMotion = System.Math.Abs(1.0f - (xMotion + yMotion)); //combination of the two to get z
        //Get power off of input axis value as the multiplier
        if (limb == Limbs.leftArm)
        {
            armToMove = leftArm;
            sideMotion *= -1.0f;
        }
        else if (limb == Limbs.rightArm)
        {
            armToMove = rightArm;
            upMotion *= -1.0f;
            forwardMotion *= -1.0f;
        }
        Vector3 directionToMove = new Vector3(upMotion, sideMotion, forwardMotion); //x is up, y is to the side, z is forward 
        armToMove.AddForce(directionToMove * forceOfMove.sqrMagnitude, ForceMode.Impulse);
    }

    /// <summary>
    /// Grab script on left hand tells us if something has been grabbed or dropped.
    /// </summary>
    /// <param name="isGrabbed">Boolean to say if something is in hand or not.</param>
    public void ObjectGrabbedLeft(bool isGrabbed)
    {
        leftGrab = isGrabbed;
    }

    /// <summary>
    /// This function disables the camera rotating and moving.
    /// It also disables the characters movement.
    /// </summary>
    /// <param name="disable">If true, toggles movement and camera off. False, turns them on.</param>
    /// <returns></returns>
    private IEnumerator ToggleCameraAndMovement(bool disable)
    {
        movementAndCameraDisabled = disable;
        yield return null;
    }

    private IEnumerator ToggleArmPuppetMaster(bool disable, Limbs limbToDisable)
    {
        yield return null;
    }
}
