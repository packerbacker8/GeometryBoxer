using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;
using RootMotion.Demos;

public class PunchScript : MonoBehaviour
{
    [Header("Punching Components")]
    [Tooltip("Object designated as the collider in front of the left fist.")]
    public CapsuleCollider leftFistCollider;
    [Tooltip("Object designated as the collider in front of the right fist.")]
    public CapsuleCollider rightFistCollider;
    [Tooltip("Object designated as the collider in front of the left foot.")]
    public Collider leftFootCollider;
    [Tooltip("Object designated as the collider in front of the right foot.")]
    public Collider rightFootCollider;
    public float fistGrowMultiplier = 2f;
    public float footGrowMultiplier = 5f;
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
    /// Class object for expanding the collider
    /// </summary>
    protected class CapsuleColliderSizing
    {
        public Vector3 pos;
        public float radius;
        public float height;
        public CapsuleColliderSizing(Vector3 startPos, float r, float h)
        {
            pos = startPos;
            radius = r;
            height = h;
        }
    }
    /// <summary>
    /// Private variables for controls in punching and moving arms.
    /// </summary>
    protected bool leftGrab;
    protected bool rightGrab;
    protected bool movementAndCameraDisabled;
    protected bool useController;
    protected bool isAttacking;
    protected bool leftArmAttack;
    protected bool rightArmAttack;

    protected float leftArmXAxis;
    protected float leftArmYAxis;
    protected float rightArmXAxis;
    protected float rightArmYAxis;
    protected float marginOfError;
    protected float currentX;
    protected float currentY;
    protected float oldInputX;
    protected float oldInputY;
    protected float currentAnimLength;

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
    protected CapsuleColliderSizing leftFistStartSize;
    protected CapsuleColliderSizing rightFistStartSize;

    public enum Limbs
    {
        leftArm = 0,
        rightArm
    };

    // Use this for initialization
    protected virtual void Start()
    {
        leftFistStartSize = new CapsuleColliderSizing(leftFistCollider.transform.position, leftFistCollider.radius, leftFistCollider.height);
        rightFistStartSize = new CapsuleColliderSizing(rightFistCollider.transform.position, rightFistCollider.radius, rightFistCollider.height);
        leftGrab = false;
        rightGrab = false;
        leftArmAttack = false;
        rightArmAttack = false;
        movementAndCameraDisabled = false;
        useController = false;
        isAttacking = false;
        controllerInfo = Input.GetJoystickNames();
        if (controllerInfo.Length > 0)
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
        currentAnimLength = 0f;
        cam = this.transform.GetChild(cameraIndex).gameObject;
        //puppetArmBehavior = this.transform.GetChild(behaviorIndex).gameObject.transform.GetChild(puppetArmBehaviorIndex).gameObject;
        charController = this.transform.GetChild(characterControllerIndex).gameObject;
        charController.GetComponent<CharacterMeleeDemo>().canMove = true;
        anim = charController.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        puppetMastObject = this.transform.GetChild(puppetMasterIndex).gameObject;
        puppetMaster = puppetMastObject.GetComponent<PuppetMaster>();
        numberOfMuscleComponents = puppetMastObject.GetComponent<PuppetMaster>().muscles.Length;
        armMuscles = new List<Muscle>();
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
        if (!info.IsName("Hit"))
        {
            anim.speed = 1f;
        }
        if (isAttacking)
        {
            if(leftArmAttack)
            {
                leftFistCollider.radius = leftFistStartSize.radius * fistGrowMultiplier;
                leftFistCollider.height = leftFistStartSize.height * fistGrowMultiplier;
            }
            if(rightArmAttack)
            {
                rightFistCollider.radius = rightFistStartSize.radius * fistGrowMultiplier;
                rightFistCollider.height = rightFistStartSize.height * fistGrowMultiplier;
            }
            currentAnimLength -= Time.deltaTime;
            if (currentAnimLength <= 0f)
            {
                isAttacking = false;
                leftFistCollider.radius = leftFistStartSize.radius;
                leftFistCollider.height = leftFistStartSize.height;
                rightFistCollider.radius = rightFistStartSize.radius;
                rightFistCollider.height = rightFistStartSize.height;
            }
        }
        else
        {
            leftFistCollider.radius = leftFistStartSize.radius;
            leftFistCollider.height = leftFistStartSize.height;
            rightFistCollider.radius = rightFistStartSize.radius;
            rightFistCollider.height = rightFistStartSize.height;
            if (!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && anim.GetBool(onGround)) //prevent use of your arms when you are on the ground and getting up.
            {
                if (useController) //controller controls
                {
                    //Left arm punching

                    if (Input.GetButtonDown(leftJabControllerButton)) //left bumper
                    {
                        ThrowSinglePunch(Limbs.leftArm);
                    }
                    if (Input.GetButtonDown(rightJabControllerButton))
                    {
                        ThrowSinglePunch(Limbs.rightArm);
                    }

                }
                else  // keyboard controls
                {

                    if (Input.GetKeyDown(leftJabKey))
                    {
                        //currently a combo attack
                        leftArmAttack = true;
                        rightArmAttack = true;
                        ThrowSinglePunch(Limbs.leftArm);
                    }
                    else if (Input.GetKeyDown(leftUppercutKey))
                    {
                        leftArmAttack = true;
                        ThrowUppercut(Limbs.leftArm);
                    }

                    if (Input.GetKeyDown(rightJabKey))
                    {
                        rightArmAttack = true;
                        ThrowSinglePunch(Limbs.rightArm);
                    }
                    else if (Input.GetKeyDown(rightUppercutKey))
                    {
                        rightArmAttack = true;
                        ThrowUppercut(Limbs.rightArm);
                    }
                    else if (Input.GetKeyDown(hiKickKey))
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
                if (leftGrab && action.animName == "SwingProp")
                {
                    currentAnim = action;
                    break;
                }
                else if (!leftGrab && action.animName == "LeftPunch")
                {
                    leftFistCollider.enabled = true;
                    currentAnim = action;
                    if (anim.GetFloat("Forward") < 0.5f)
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
                    rightFistCollider.enabled = true;
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
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime);
        //going to need to determine when animation ends to allow next triggering event
        SetCurrentAnimTime(currentAnim);
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
                leftFistCollider.enabled = true;
                currentAnim = action;
                break;
            }
            if (limb == Limbs.rightArm && action.animName == "RightUpperCut")
            {
                rightFistCollider.enabled = true;
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
        SetCurrentAnimTime(currentAnim);
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
                rightFootCollider.enabled = true;
                currentAnim = action;
                break;
            }
        }
        anim.SetInteger("ActionIndex", currentAnim.actionIndex);
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime);
        SetCurrentAnimTime(currentAnim);
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
        /*
         * Currently phased out.
         * 
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
        */
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

    public virtual void ImpactReceived(Collision collision)
    {
        if (collision.gameObject.transform.root.tag.Contains("Enemy") && isAttacking)
        {
            GameObject findingRoot = collision.gameObject;
            while (findingRoot.tag != "EnemyRoot")
            {
                findingRoot = findingRoot.transform.parent.gameObject;
            }
            BehaviourPuppet behavePup = findingRoot.GetComponentInChildren<BehaviourPuppet>();
            //behavePup.SetState(BehaviourPuppet.State.Unpinned);
            //collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(collision.impulse.sqrMagnitude, collision.gameObject.transform.position, 10f, 0f, ForceMode.Impulse);

        }
    }

    protected virtual void SetCurrentAnimTime(CharacterAnimations currentAnim)
    {
        currentAnimLength = anim.GetCurrentAnimatorStateInfo(currentAnim.animLayer).length * currentAnim.playTime + (anim.GetCurrentAnimatorStateInfo(currentAnim.animLayer).length * currentAnim.transitionTime);
        isAttacking = true;
        Debug.Log("current animation length: " + currentAnimLength);
    }
}
