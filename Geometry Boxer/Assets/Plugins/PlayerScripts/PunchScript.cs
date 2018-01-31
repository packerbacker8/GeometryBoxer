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
    public BoxCollider leftFootCollider;
    [Tooltip("Object designated as the collider in front of the right foot.")]
    public BoxCollider rightFootCollider;
    public float fistGrowMultiplier = 2f;
    public float footGrowMultiplier = 5f;
    [Tooltip("The force by which the rigidbody will move.")]
    public float punchForce = 50f;
    public GameObject rightShoulder;
    public GameObject leftShoulder;
    public GameObject rightThigh;
    public GameObject leftThigh;

    [Header("PC punch buttons.")]
    public KeyCode leftJabKey = KeyCode.Q;
    public KeyCode rightJabKey = KeyCode.E;
    public KeyCode hookModifierKey = KeyCode.LeftControl; //plus q
    //public KeyCode rightHookKey = KeyCode.E;
    public KeyCode leftUppercutKey = KeyCode.R;
    public KeyCode rightUppercutKey = KeyCode.F;
    public KeyCode hiKickKey = KeyCode.K;
    public KeyCode dropWeapon = KeyCode.X;

    [Header("Controller punch buttons.")]
    public string leftJabControllerButton = "LeftBumper";
    public string rightJabControllerButton = "RightBumper";
    public string upperCutButton = "XButton";
    public string hiKickButton = "YButton";
    public string specialAttackButton = "BButton";

    [Header("Special Attack Information")]
    public GameObject specialForm;
    public float specialFormSize = 4f;
    public float growSpeed = 3f;
    public KeyCode specialAttack = KeyCode.B;
    public KeyCode useAttack = KeyCode.Space;
    public float specialAttackForce = 1000f;
    public float specialAttackActiveTime = 10f;
    public float specialAttackCooldownTime = 2f;

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
    /// Class object for expanding a box collider
    /// </summary>
    protected class BoxColliderSizing
    {
        public Vector3 center;
        public Vector3 size;
        public BoxColliderSizing(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = size;
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
    protected bool leftFootAttack;
    protected bool rightFootAttack;
    protected bool playerGrowing;
    protected bool launched;
    protected bool onCooldown;
    protected bool growingSpecial;
    protected bool updateCollisionCheck;

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
    protected Rigidbody specialRigid;
    protected PuppetMaster puppetMaster;
    protected GameObject charController;
    protected GameObject cam;
    protected List<Muscle> armMuscles;
    protected CapsuleColliderSizing leftFistStartSize;
    protected CapsuleColliderSizing rightFistStartSize;
    protected BoxColliderSizing leftFootStartSize;
    protected BoxColliderSizing rightFootStartSize;
    protected Vector3 playerStartSize;
    protected Vector3 playerFinalSize;
    protected Vector3 specialStartSize;
    protected Vector3 specialEndSize;
    protected PlayerStatsBaseClass baseStats;
    protected Vector3 moveDir;

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
        leftFootStartSize = new BoxColliderSizing(leftFootCollider.center, leftFootCollider.size);
        rightFootStartSize = new BoxColliderSizing(rightFootCollider.center, rightFootCollider.size);

        leftGrab = false;
        rightGrab = false;
        leftArmAttack = false;
        rightArmAttack = false;
        leftFootAttack = false;
        rightFootAttack = false;
        movementAndCameraDisabled = false;
        useController = false;
        isAttacking = false;
        playerGrowing = false;
        launched = false;
        onCooldown = false;
        growingSpecial = false;
        updateCollisionCheck = false;
        controllerInfo = Input.GetJoystickNames();
        useController = controllerInfo.Length > 0;

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

        playerStartSize = new Vector3(0.1f, 0.1f, 0.1f);
        playerFinalSize = new Vector3(1.53119f, 1.53119f, 1.53119f);
        specialStartSize = new Vector3(0.1f, 0.1f, 0.1f);
        specialEndSize = new Vector3(specialFormSize, specialFormSize, specialFormSize);
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        useController = controllerInfo.Length > 0;

        if (Input.GetKeyDown(dropWeapon) || (useController && Input.GetAxisRaw("DPadY") == -1))
        {
            charController.GetComponent<CharacterMeleeDemo>().propRoot.currentProp = null;
        }

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
            if(leftFootAttack)
            {
				leftFootCollider.size = leftFootStartSize.size * footGrowMultiplier;
            }
            if(rightFootAttack)
            {
				rightFootCollider.size = rightFootStartSize.size * footGrowMultiplier;
            }
            currentAnimLength -= Time.deltaTime;
            if (currentAnimLength <= 0f)
            {
                isAttacking = false;
                updateCollisionCheck = true;
				leftArmAttack = false;
				rightArmAttack = false;
				leftFootAttack = false;
				rightFootAttack = false;

                leftFistCollider.radius = leftFistStartSize.radius;
                leftFistCollider.height = leftFistStartSize.height;
                rightFistCollider.radius = rightFistStartSize.radius;
                rightFistCollider.height = rightFistStartSize.height;

                leftFootCollider.center = leftFootStartSize.center;
                leftFootCollider.size = leftFootStartSize.size;
                rightFootCollider.center = rightFootStartSize.center;
                rightFootCollider.size = rightFootStartSize.size;
            }
        }
        else
        {
            leftFistCollider.radius = leftFistStartSize.radius;
            leftFistCollider.height = leftFistStartSize.height;
            rightFistCollider.radius = rightFistStartSize.radius;
            rightFistCollider.height = rightFistStartSize.height;

            leftFootCollider.center = leftFootStartSize.center;
            leftFootCollider.size = leftFootStartSize.size;
            rightFootCollider.center = rightFootStartSize.center;
            rightFootCollider.size = rightFootStartSize.size;
            if (!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && anim.GetBool(onGround)) //prevent use of your arms when you are on the ground and getting up.
            {
                if (useController) //controller controls
                {
                    //Left arm punching
                    if (Input.GetButtonDown(leftJabControllerButton)) //left bumper
                    {
                        Debug.Log("LeftJab");
                        leftArmAttack = true;
                        if (Input.GetButton(upperCutButton))
                        {
                            Debug.Log("LeftUpper");
                            ThrowUppercut(Limbs.leftArm);
                        }
                        else
                        {
                            Debug.Log("LeftJab");
                            ThrowSinglePunch(Limbs.leftArm);
                        }
                    }
                    if (Input.GetButtonDown(rightJabControllerButton))
                    {
                        Debug.Log("rightBump");
                        rightArmAttack = true;
                        if (Input.GetButton(upperCutButton))
                        {
                            Debug.Log("RightUpper");
                            ThrowUppercut(Limbs.rightArm);

                        }
                        else
                        {
                            Debug.Log("RightJab");
                            ThrowSinglePunch(Limbs.rightArm);
                        }
                    }
                    if (Input.GetButtonDown(hiKickButton))
                    {
                        Debug.Log("HighKick");
                        ThrowHiKick();
                    }

                }
                else  // keyboard controls
                {
                    if (Input.GetKeyDown(leftJabKey))
                    {
                        Debug.Log("LeftJab");
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
                        rightFootAttack = true;
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
                //Debug.Log("Hit by: " + collision.gameObject.name);
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
    }

    /// <summary>
    /// Turn off the special attack and reactivate the player character.
    /// </summary>
    protected virtual void DeactivateSpecialAttack()
    {
        playerGrowing = true;
        charController.transform.localScale = playerStartSize;
        charController.GetComponent<Rigidbody>().velocity = new Vector3(charController.GetComponent<Rigidbody>().velocity.x, 0, charController.GetComponent<Rigidbody>().velocity.z);
        specialRigid.useGravity = false;
        launched = false;
        UpdatePos(charController.transform, specialForm.transform);
        //play animation of morphing into ball
        isAttacking = false;
        updateCollisionCheck = true;
        for (int i = 0; i < this.transform.childCount; i++) //move camera back to player here
        {
            if (this.transform.GetChild(i).gameObject != specialForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = baseStats.pelvisJoint.transform;
                }
                else if (this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        charController.GetComponent<UserControlMelee>().enabled = true;
        charController.GetComponent<CharacterMeleeDemo>().enabled = true;
        charController.GetComponent<CapsuleCollider>().enabled = true;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        onCooldown = true;
        anim.SetInteger("ActionIndex", -1);
        anim.SetBool("IsStrafing", false);
        if (specialRigid.velocity.sqrMagnitude > 0)
        {
            anim.SetFloat("Forward", 1);
        }
        else
        {
            anim.SetFloat("Forward", 0);
        }
        anim.Play("Grounded Directional");
        SendMessage("PowerUpDeactivated", false, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Turn on the special attack and deactivate the player character.
    /// </summary>
    protected virtual void ActivateSpecialAttack()
    {
        specialRigid.useGravity = true;
        leftFistCollider.radius = leftFistStartSize.radius;
        leftFistCollider.height = leftFistStartSize.height;
        rightFistCollider.radius = rightFistStartSize.radius;
        rightFistCollider.height = rightFistStartSize.height;
        UpdatePos(specialForm.transform, charController.transform);
        isAttacking = true;
        //play animation of morphing into ball
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject != specialForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = specialForm.transform;
                }
                else if (this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        specialForm.SetActive(true);
        charController.SetActive(true);
        specialForm.transform.localScale = specialStartSize;
        specialForm.transform.localRotation = Quaternion.identity;
        specialForm.GetComponent<MeshRenderer>().enabled = true;
        specialForm.GetComponent<BoxCollider>().enabled = true;
        charController.GetComponent<UserControlMelee>().enabled = false;
        charController.GetComponent<CharacterMeleeDemo>().enabled = false;
        charController.GetComponent<CapsuleCollider>().enabled = false;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        SendMessage("PowerUpActive", true, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Update the position of one transform to the target transform of another game object.
    /// This specifically accounts bonus movement of the special form upwards to avoid clipping 
    /// through the floor when spawning.
    /// </summary>
    /// <param name="transformToUpdate">The transform object to move.</param>
    /// <param name="targetTransform">The transform object to move the other object to.</param>
    protected virtual void UpdatePos(Transform transformToUpdate, Transform targetTransform)
    {
        Vector3 targetVec = targetTransform.position;
        transformToUpdate.rotation = Quaternion.Euler(0, transformToUpdate.eulerAngles.y, 0);
        if (transformToUpdate == specialForm.transform)
        {
            targetVec = new Vector3(targetVec.x, targetVec.y + 1f, targetVec.z);
        }
        transformToUpdate.position = targetVec;
    }

    /// <summary>
    /// Raycast from center of special form to ground to see if on the ground.
    /// </summary>
    /// <returns>Returns true if on ground, false otherwise, with some tolerance.</returns>
    protected virtual bool checkIfGrounded()
    {
        Vector3 endPoint = new Vector3(specialForm.transform.position.x, specialForm.transform.position.y - specialForm.GetComponent<BoxCollider>().size.y * 2f, specialForm.transform.position.z);
        //Debug.DrawLine(specialForm.transform.position, endPoint, Color.red, 5f);
        //Debug.Log("Size now: " + specialForm.GetComponent<BoxCollider>().size.y);
        return Physics.Raycast(specialForm.transform.position, -Vector3.up, specialForm.GetComponent<BoxCollider>().size.y * specialForm.transform.localScale.y + 0.1f);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void GrowSpecial()
    {
        specialForm.transform.localScale += specialStartSize * growSpeed;
        if (specialForm.transform.localScale.magnitude >= specialEndSize.magnitude)
        {
            specialForm.transform.localScale = specialEndSize;
            growingSpecial = false;
        }
    }

    /// <summary>
    /// Method that grows the player and shrinks the special form at the same time.
    /// This allows the feeling of changing form.
    /// Defaults to box collider.
    /// </summary>
    protected virtual void GrowPlayer()
    {
        charController.transform.localScale += playerStartSize;
        specialForm.transform.localScale -= specialStartSize * growSpeed;
        UpdatePos(specialForm.transform, charController.transform);
        if (CompareGreaterThanEqualVectors(charController.transform.localScale, playerFinalSize))
        {
            charController.transform.localScale = playerFinalSize;
            specialForm.GetComponent<MeshRenderer>().enabled = false;
            specialForm.GetComponent<BoxCollider>().enabled = false; //CHANGE TO MESH COLLIDER WHEN OCTAHEDRON BROUGHT IN
            specialForm.transform.localScale = specialStartSize;
            specialRigid.velocity = Vector3.zero;
            playerGrowing = false;
        }
        if (CompareLessThanEqualVectors(specialForm.transform.localScale, specialStartSize))
        {
            specialForm.transform.localScale = specialStartSize;

        }
    }

    protected bool CompareLessThanEqualVectors(Vector3 v1, Vector3 v2)
    {
        return v1.x <= v2.x && v1.y <= v2.y && v1.z <= v2.z;
    }

    protected bool CompareGreaterThanEqualVectors(Vector3 v1, Vector3 v2)
    {
        return v1.x >= v2.x && v1.y >= v2.y && v1.z >= v2.z;
    }
    public bool getUseController()
    {
        return useController;
    }
}
