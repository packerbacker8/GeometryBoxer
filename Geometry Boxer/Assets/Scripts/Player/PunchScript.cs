using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;
using RootMotion.Demos;

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
    public KeyCode hookModifierKey = KeyCode.LeftControl; //plus q
    //public KeyCode rightHookKey = KeyCode.E;
    public KeyCode leftUppercutKey = KeyCode.R;
    public KeyCode rightUppercutKey = KeyCode.F;
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
    private int cameraIndex = 3;
    private int behaviorIndex = 0;
    private int puppetArmBehaviorIndex = 1;

    private string leftPunchAnimation = "Hit";
    private string rightPunchAnimation = "Hit";
    private string leftUppercutAnimation = "LeftUpperCut";
    private string leftSwingAnimation = "SwingProp";
    private string rightSwingAnimation = "SwingProp";
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string fall = "Fall";
    private string onGround = "OnGround";

    private GameObject puppetArmBehavior;
    private GameObject puppetMastObject;
    private PuppetMaster puppetMaster;
    private GameObject charController;
    private GameObject cam;

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
        cam = this.transform.GetChild(cameraIndex).gameObject;
        puppetArmBehavior = this.transform.GetChild(behaviorIndex).gameObject.transform.GetChild(puppetArmBehaviorIndex).gameObject;
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        charController.GetComponent<CharacterMeleeDemo>().canMove = true;
        anim = charController.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMastObject = this.transform.GetChild(puppetMasterIndex).gameObject;
        puppetMaster = puppetMastObject.GetComponent<PuppetMaster>();
        numberOfMuscleComponents = puppetMastObject.GetComponent<PuppetMaster>().muscles.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfMuscleComponents < puppetMaster.muscles.Length) //number of muscles increased from beginning, a prop has been picked up
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
                //turn off camera movement and character movement
                StartCoroutine(ToggleCameraAndMovement(controllingLeftArm || controllingRightArm));
                StartCoroutine(ToggleArmPuppetMaster(controllingLeftArm, Limbs.leftArm));
                StartCoroutine(ToggleArmPuppetMaster(controllingRightArm, Limbs.rightArm));
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
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    controllingLeftArm = true;
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    controllingLeftArm = false;
                }
                //Right trigger lets player control right arm independently
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    controllingRightArm = true;

                }
                else if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    controllingRightArm = false;
                }
                //turn off camera movement and character movement
                StartCoroutine(ToggleCameraAndMovement(controllingLeftArm || controllingRightArm));

                StartCoroutine(ToggleArmPuppetMaster(controllingLeftArm, Limbs.leftArm));
                StartCoroutine(ToggleArmPuppetMaster(controllingRightArm, Limbs.rightArm));

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
                else if(Input.GetKeyDown(leftUppercutKey))
                {
                    ThrowUppercut(Limbs.leftArm);
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
                else if(Input.GetKeyDown(rightUppercutKey))
                {
                    ThrowUppercut(Limbs.rightArm);
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
    /// Cause player to throw single uppercut of one arm.  Arm is determined by limb parameter.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    private void ThrowUppercut(Limbs limb)
    {
        if (limb == Limbs.leftArm)
        {
            anim.Play(leftUppercutAnimation, punchAnimLayer);
            //anim.SetInteger("ActionIndex", 2);
        }
        else if (limb == Limbs.rightArm)
        {

            anim.Play(rightPunchAnimation, punchAnimLayer);

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
        //if(useController)  //controller needs a little more force
        //{

        //}
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
        yield return null;
        movementAndCameraDisabled = disable;
        cam.GetComponent<CameraController>().rotateAlways = !disable;
        charController.GetComponent<CharacterMeleeDemo>().canMove = !disable;
        if (disable)
        {
            anim.SetFloat("Forward", 0f);
            anim.SetFloat("Jump", 0f);
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disable"></param>
    /// <param name="limbToDisable"></param>
    /// <returns></returns>
    private IEnumerator ToggleArmPuppetMaster(bool disable, Limbs limbToDisable)
    {
        foreach (Muscle m in puppetMaster.muscles) //NOT RIGHT DOING IT FOR WHOLE BODY NOT JUST ARMS, STILL IN ANIM POSITION
        {
            if(m.name.Contains("arm") || m.name.Contains("hand"))
            {
                if (disable)
                    m.state.pinWeightMlp = 0f;
                else
                    m.state.pinWeightMlp = 1f;
            }
        }
        yield return null;
    }
}
