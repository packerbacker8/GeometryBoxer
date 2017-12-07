using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;
using RootMotion.Demos;

public class PunchScript : MonoBehaviour
{
    
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
    public KeyCode hiKickKey = KeyCode.K;
    [Header("Controller punch buttons.")]
    public string leftJabControllerButton = "LeftBumper";
    public string rightJabControllerButton = "RightBumper";

    [Header("Player Combat Animations")]
    [Tooltip("Array of fighting animations the player character can use.")]
    public List<CharacterAnimations> playerAnimations = new List<CharacterAnimations>();

    /// <summary>
    /// Information relating to a character animation.
    /// </summary>
    [System.Serializable]
    public struct CharacterAnimations
    {
        public int actionIndex;
        public string animName;
        public int animLayer;
        public float transitionTime;
        public float playTime;
    }
    /// <summary>
    /// Private variables for controls in punching and moving arms.
    /// </summary>
    protected bool controllingLeftArm; //controlling left arm or right arm is the same as disabling puppet master control
    protected bool controllingRightArm;
    protected bool leftGrab;
    protected bool rightGrab;
    protected bool movementAndCameraDisabled;
    protected bool useController;

    protected float leftArmXAxis;
    protected float leftArmYAxis;
    protected float rightArmXAxis;
    protected float rightArmYAxis;
    protected float marginOfError;
    protected float currentX;
    protected float currentY;
    protected float oldInputX;
    protected float oldInputY;
    
    protected Animator anim;
    protected int characterControllerIndex = 2;
    protected int animationControllerIndex = 0;
    protected int puppetMasterIndex = 1;
    protected int numberOfMuscleComponents;
    protected int swingAnimLayer = 1;
    protected int punchAnimLayer = 0;
    protected int cameraIndex = 3;
    protected int behaviorIndex = 0;
    protected int puppetArmBehaviorIndex = 1;
    
    protected string leftPunchAnimation = "Hit";
    protected string rightPunchAnimation = "Hit";
    protected string leftUppercutAnimation = "LeftUpperCut";
    protected string rightUppercutAnimation = "RightUpperCut";
    protected string leftSwingAnimation = "SwingProp";
    protected string rightSwingAnimation = "SwingProp";
    protected string getUpProne = "GetUpProne";
    protected string getUpSupine = "GetUpSupine";
    protected string fall = "Fall";
    protected string onGround = "OnGround";
    protected string[] controllerInfo;

    protected GameObject puppetArmBehavior;
    protected GameObject puppetMastObject;
    protected PuppetMaster puppetMaster;
    protected GameObject charController;
    protected GameObject cam;
    protected List<Muscle> armMuscles;

    public enum Limbs
    {
        leftArm = 0,
        rightArm
    };

    // Use this for initialization
    protected virtual void Start()
    {
        controllingLeftArm = false;
        controllingRightArm = false;
        leftGrab = false;
        rightGrab = false;
        movementAndCameraDisabled = false;
        useController = false;
        controllerInfo = Input.GetJoystickNames();
        if(controllerInfo.Length > 0)
        {
            useController = true;
        }
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
        //puppetArmBehavior = this.transform.GetChild(behaviorIndex).gameObject.transform.GetChild(puppetArmBehaviorIndex).gameObject;
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        charController.GetComponent<CharacterMeleeDemo>().canMove = true;
        anim = charController.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMastObject = this.transform.GetChild(puppetMasterIndex).gameObject;
        puppetMaster = puppetMastObject.GetComponent<PuppetMaster>();
        numberOfMuscleComponents = puppetMastObject.GetComponent<PuppetMaster>().muscles.Length;
        armMuscles = new List<Muscle>();
//        foreach (Muscle m in puppetMaster.muscles) //NOT RIGHT DOING IT FOR WHOLE BODY NOT JUST ARMS, STILL IN ANIM POSITION
//        {
//            if (m.name.Contains("arm") || m.name.Contains("hand"))
//            {
//                armMuscles.Add(m);
//            }
//        }
//    }
			foreach (Muscle m in puppetMaster.muscles) 
			{
				if (m.props.group == Muscle.Group.Arm || m.props.group == Muscle.Group.Hand)
				{
					armMuscles.Add(m);
				}
			}
	}
    // Update is called once per frame
    protected virtual void Update()
    {
        useController = controllerInfo.Length > 0;
        
        if (numberOfMuscleComponents < puppetMaster.muscles.Length) //number of muscles increased from beginning, a prop has been picked up
        {
            rightGrab = true;
        }
        else //number of muscles is the same  or less, i.e. prop lost
        {
            rightGrab = false;
        }
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if(!info.IsName("Hit"))
        {
            anim.speed = 1f;
        }
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
                else if (Input.GetKeyDown(leftUppercutKey))
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
                else if (Input.GetKeyDown(rightUppercutKey))
                {
                    ThrowUppercut(Limbs.rightArm);
                }
                else if(Input.GetKeyDown(hiKickKey))
                {
                    ThrowHiKick();
                }
            }
        }
        else
        {
            //do something if down on the ground, ground combat
        }
    }

    /// <summary>
    /// Cause player to throw single punch of one arm.  Arm is determined by limb parameter.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    public virtual void ThrowSinglePunch(Limbs limb)
    {
        CharacterAnimations currentAnim = InitCharacterAnimationStruct();
        foreach (CharacterAnimations action in playerAnimations)
        {
            if (limb == Limbs.leftArm)
            {
                if(leftGrab && action.animName == "SwingProp")
                {
                    currentAnim = action;
                    break;
                }
                else if(!leftGrab && action.animName == "LeftPunch")
                {
                    currentAnim = action;
                    if(anim.GetFloat("Forward") < 0.5f)
                    {
                        currentAnim.animLayer = 0;
                    }
                    break;
                }
            }
            if (limb == Limbs.rightArm)
            {
                
                if (rightGrab && action.animName == "SwingProp")
                {
                    currentAnim = action;
                    break;
                }
                else if (!rightGrab && action.animName == "RightPunch")
                {
                    currentAnim = action;
                    //anim.speed = 5f;
                    if (anim.GetFloat("Forward") < 0.5f)
                    {
                        currentAnim.animLayer = 0;
                    }
                    else
                    {
                        currentAnim.animLayer = 1;
                    }
                    break;
                }
            }
        }
        anim.SetInteger("ActionIndex", currentAnim.actionIndex);
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime); //going to need to determine when animation ends to allow next triggering event
        anim.SetInteger("ActionIndex", -1);
    }

    /// <summary>
    /// Cause player to throw single uppercut of one arm.  Arm is determined by limb parameter.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    public virtual void ThrowUppercut(Limbs limb)
    {
        CharacterAnimations currentAnim = InitCharacterAnimationStruct();
        //anim.Play(leftUppercutAnimation, punchAnimLayer);
        foreach (CharacterAnimations action in playerAnimations)
        {
            if (limb == Limbs.leftArm && action.animName == "LeftUpperCut")
            {
                currentAnim = action;
                break;
            }
            if (limb == Limbs.rightArm && action.animName == "RightUpperCut")
            {
                currentAnim = action;
                if (anim.GetFloat("Forward") < 0.5f)
                {
                    currentAnim.animLayer = 0;
                }
                else
                {
                    currentAnim.animLayer = 1; //forced
                }
                break;
            }
        }

        anim.SetInteger("ActionIndex", currentAnim.actionIndex);
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime);
        anim.SetInteger("ActionIndex", -1);

    }

    /// <summary>
    /// Function to allow players to throw a high kick at the enemies. Only for right leg.
    /// </summary>
    public virtual void ThrowHiKick()
    {
        CharacterAnimations currentAnim = InitCharacterAnimationStruct();
        foreach (CharacterAnimations action in playerAnimations)
        {
            if (action.animName == "HiKick")
            {
                currentAnim = action;
                break;
            }
        }
        anim.SetInteger("ActionIndex", currentAnim.actionIndex);
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime);
        anim.SetInteger("ActionIndex", -1);
    }


    /// <summary>
    /// Allows player to move limb about as they choose with the controller joysticks.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    /// <param name="xMotion">Gets axis value from input for left and right sticks. In horizontal direction.</param>
    /// <param name="yMotion">Gets axis value from input for left and right sticks. In vertical direction.</param>
    public virtual void MoveArm(Limbs limb, float xMotion, float yMotion)
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
    /// Make arms of player go limp.
    /// </summary>
    /// <param name="disable"></param>
    /// <param name="limbToDisable"></param>
    /// <returns></returns>
    private IEnumerator ToggleArmPuppetMaster(bool disable, Limbs limbToDisable)
    {
        foreach (Muscle m in armMuscles) // STILL IN ANIM POSITION
        {
            if (disable)
            {
                m.state.pinWeightMlp = 0f;
                m.state.muscleWeightMlp = 0f;
            }
            else
            {
                m.state.pinWeightMlp = 1f;
                m.state.muscleWeightMlp = 1f;
            }
        }
        yield return null;
    }

    private CharacterAnimations InitCharacterAnimationStruct()
    {
        CharacterAnimations result;
        result.animName = "Grounded Directional";
        result.actionIndex = -1;
        result.animLayer = 1;
        result.playTime = 1f;
        result.transitionTime = 1f;
        return result;
    }
}
