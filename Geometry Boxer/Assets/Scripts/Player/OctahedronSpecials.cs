using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using RootMotion;
using PlayerUI;

public class OctahedronSpecials : PunchScript
{
    public float spinForceMult = 500f;
    public float launchLength = 1f;
    public float angularDragAmount = 2f;
    public float floatHeight = 1f;
    public float octahedronExtension = 5f;
    public int SpinsTillSwitch = 5;
    public bool debugMode = false;

    //public CharacterController;
    // animClips = anim.runtimeAnimatorController.animationClips;

    private float currentTime = 0.0f;
    private float SpecialTime = 0.0f;
    private float coolDownTimer;
    private float floatOffset;
    private float launchTime;
    private bool executingSpecial = false;
    private bool specialIsSpeed = false;
    private bool tornadoMode = false;
    private bool isGrounded;
    private bool floatCapped;

    private int isFloating;
    private int spinCount;
    private int spinDir;

    private MeshCollider specialFormCollider;


    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        baseStats = this.GetComponent<OctahedronStats>();
        specialFormCollider = specialForm.GetComponent<MeshCollider>();
        specialForm.GetComponent<MeshRenderer>().enabled = false;
        specialFormCollider.enabled = false;   // TEMPORARILY A CUBE COLLIDER UNTIL MESH IS FIXED
        specialRigid = specialForm.GetComponent<Rigidbody>();

        coolDownTimer = 0f;
        launchTime = 0;
        floatOffset = 0.2f;
        specialRigid.useGravity = false;
        specialRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        specialStartSize = new Vector3(0.1f * octahedronExtension, 0.1f, 0.1f);
        specialEndSize = new Vector3(specialFormSize * octahedronExtension, specialFormSize, specialFormSize);

        isGrounded = checkIfGrounded();
        floatCapped = false;
        isFloating = 0;
        spinCount = 0;
        spinDir = 1;
        specialRigid.maxAngularVelocity = Mathf.Infinity;
        specialRigid.angularDrag = angularDragAmount;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!baseStats.IsDead)
        {
            if (updateCollisionCheck)
            {
                //ASSUMES ALL CHARACTER ARMS AND LEGS ARE 3 JOINTS
                GameObject walker = leftShoulder;
                GameObject walker2 = rightShoulder; //need both for sake of combo
                GameObject walker3 = rightThigh;
                while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
                {
                    walker.GetComponent<CollisionReceived>().sendDamage = true;
                    walker2.GetComponent<CollisionReceived>().sendDamage = true;
                    walker3.GetComponent<CollisionReceived>().sendDamage = true;
                    //assumes there is only one child
                    walker = walker.transform.GetChild(0).gameObject;
                    walker2 = walker2.transform.GetChild(0).gameObject;
                    walker3 = walker3.transform.GetChild(0).gameObject;
                }
                if (walker.GetComponent<CollisionReceived>() != null && walker2.GetComponent<CollisionReceived>() != null && walker3.GetComponent<CollisionReceived>() != null)
                {
                    walker.GetComponent<CollisionReceived>().sendDamage = true;
                    walker2.GetComponent<CollisionReceived>().sendDamage = true;
                    walker3.GetComponent<CollisionReceived>().sendDamage = true;
                }

                updateCollisionCheck = false;
            }

            if (growingSpecial)
            {
                GrowSpecial();
            }
            else if (playerGrowing)
            {
                GrowPlayer();
            }
            else if (specialForm.GetComponent<MeshRenderer>().enabled)
            {
                if (Mathf.Abs(specialRigid.angularVelocity.y) < 5f)
                {
                    SendMessage("OctaSpinStopSfx", true, SendMessageOptions.DontRequireReceiver);
                }

                UpdatePos(charController.transform, specialForm.transform);
                coolDownTimer += Time.deltaTime;

                isFloating = checkIfFloating();
                //too high
                if (isFloating > 0)
                {
                    if (specialRigid.velocity.y > 0f)
                    {
                        floatCapped = false;
                    }
                    if (!floatCapped) specialRigid.velocity = new Vector3(specialRigid.velocity.x, 0f, specialRigid.velocity.z);
                    floatCapped = true;
                    specialRigid.useGravity = true;
                }
                //too low
                else if (isFloating < 0)
                {
                    specialRigid.useGravity = false;
                    specialRigid.AddForce(Vector3.up * specialAttackForce);
                }
                //in range
                else
                {
                    specialRigid.velocity = new Vector3(specialRigid.velocity.x, 0f, specialRigid.velocity.z);
                    specialRigid.useGravity = false;
                }
                moveDir = new Vector3(Input.GetAxisRaw(horizontalStick), 0, Input.GetAxisRaw(verticalStick));
                moveDir = cam.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir = Vector3.Normalize(moveDir);
                moveDir.x = moveDir.x * specialAttackForce * baseStats.GetPlayerSpeed() * Time.deltaTime;
                moveDir.z = moveDir.z * specialAttackForce * baseStats.GetPlayerSpeed() * Time.deltaTime;
                specialRigid.AddForce(moveDir); //Testing playing around with moving the octa hedron around with move keys
                if (coolDownTimer >= specialAttackActiveTime)
                {
                    DeactivateSpecialAttack();
                    UpdatePos(charController.transform, specialForm.transform);
                    coolDownTimer = 0f;
                }
                if ((Input.GetKeyDown(specialAttack) && !IsPlayer2) || Input.GetButtonDown(specialAttackButton))
                {
                    DeactivateSpecialAttack();
                    UpdatePos(charController.transform, specialForm.transform);
                    coolDownTimer = 0f;
                }
                if ((Input.GetKeyDown(useAttack) || Input.GetButtonDown(activateSpecialAttackButton)) && specialForm.GetComponent<MeshRenderer>().enabled && !launched) //include jump key for controller
                {
                    moveDir = Vector3.forward;
                    moveDir = cam.transform.TransformDirection(moveDir);
                    moveDir.y = 0;
                    moveDir = Vector3.Normalize(moveDir);
                    moveDir.x = moveDir.x * specialAttackForce * 100f;
                    moveDir.z = moveDir.z * specialAttackForce * 100f;
                    specialRigid.AddForce(moveDir);
                    launched = true;
                    specialRigid.AddForce(Vector3.up * specialAttackForce * 2f);
                    /*if(specialRigid.angularVelocity.y < 0.5f)
                    {
                        spinDir *= -1;
                    }*/
                    if (spinDir == 1)
                    {
                        spinCount++;
                    }
                    else
                    {
                        spinCount--;
                    }
                    if (spinCount == SpinsTillSwitch)
                    {
                        spinDir = -1;
                    }
                    else if (spinCount == 0)
                    {
                        spinDir = 1;
                    }
                    SendMessage("OctaSpinSfx", true, SendMessageOptions.DontRequireReceiver);
                }
                else if (launched)
                {
                    specialRigid.AddTorque((spinDir * Vector3.up) * specialAttackForce * spinForceMult);
                    launchTime += Time.deltaTime;
                    if (launchTime >= launchLength)
                    {
                        launched = false;
                        launchTime = 0;
                    }
                    SendMessage("OctaSpinSfx", true, SendMessageOptions.DontRequireReceiver);
                }

            }
            else
            {
                UpdatePos(specialForm.transform, charController.transform);
                if (((Input.GetKeyDown(specialAttack) && !IsPlayer2) || Input.GetButtonDown(specialAttackButton)) && !specialForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
                {
                    growingSpecial = true;
                    ActivateSpecialAttack();
                    UpdatePos(specialForm.transform, charController.transform);
                }
            }
            if (onCooldown)
            {
                coolDownTimer += Time.deltaTime;
                if (coolDownTimer >= specialAttackCooldownTime)
                {
                    onCooldown = false;
                    coolDownTimer = 0f;
                }
            }
        }

    }


    protected override bool checkIfGrounded()
    {
        return Physics.Raycast(specialForm.transform.position, -Vector3.up, specialFormCollider.bounds.size.y + 0.1f);
    }

    /// <summary>
    /// Check to see if the octahedron is floating above the ground within a certain range.
    /// </summary>
    /// <returns>Returns 0 if above lower range and below upper range.
    /// Returns greater than 0 if outside range of both upper and lower.
    /// Returns less than 0 if within range of both upper and lower, too low.</returns>
    private int checkIfFloating()
    {
        int result = 0;
        Vector3 endPoint = new Vector3(specialForm.transform.position.x, specialForm.transform.position.y - specialFormCollider.bounds.size.y * floatHeight, specialForm.transform.position.z);
        bool lower = Physics.Raycast(specialForm.transform.position, -Vector3.up, specialFormCollider.bounds.size.y * floatHeight);
        bool upper = Physics.Raycast(specialForm.transform.position, -Vector3.up, specialFormCollider.bounds.size.y * (floatHeight + floatOffset));
        if (lower && upper) //within cast range of both raycasts
        {
            result = -1;
            if (debugMode) Debug.DrawLine(specialForm.transform.position, endPoint, Color.red, 2f);
        }
        else if (!lower && !upper) //outside both
        {
            result = 1;
            if (debugMode) Debug.DrawLine(specialForm.transform.position, endPoint, Color.yellow, 2f);
        }
        else if (debugMode)
        {
            Debug.DrawLine(specialForm.transform.position, endPoint, Color.green, 2f);
            Debug.DrawLine(specialForm.transform.position, new Vector3(endPoint.x, endPoint.y + floatOffset, endPoint.z), Color.blue, 2f);
        }
        return result;
    }


    /// <summary>
    /// Method that grows the player and shrinks the special form at the same time.
    /// This allows the feeling of changing form.
    /// Defaults to box collider.
    /// </summary>
    protected override void GrowPlayer()
    {
        charController.transform.localScale += playerStartSize;
        specialForm.transform.localScale -= specialStartSize * growSpeed;
        UpdatePos(specialForm.transform, charController.transform);
        if (charController.transform.localScale.magnitude >= playerFinalSize.magnitude)
        {
            charController.transform.localScale = playerFinalSize;
            specialForm.GetComponent<MeshRenderer>().enabled = false;
            specialFormCollider.enabled = false; //CHANGE TO MESH COLLIDER WHEN OCTAHEDRON BROUGHT IN
            specialRigid.velocity = Vector3.zero;
            playerGrowing = false;
        }
        if (specialForm.transform.localScale.magnitude <= specialStartSize.magnitude)
        {
            specialForm.transform.localScale = specialStartSize;
        }
    }

    /// <summary>
    /// Turn off the special attack and reactivate the player character.
    /// </summary>
    protected override void DeactivateSpecialAttack()
    {
        SendMessage("OctaDeactivateSfx", true, SendMessageOptions.DontRequireReceiver);
        playerGrowing = true;
        specialRigid.angularDrag = 100f;
        specialRigid.angularVelocity = Vector3.zero;
        charController.transform.localScale = playerStartSize;
        charController.GetComponent<Rigidbody>().useGravity = true;
        charController.GetComponent<Rigidbody>().velocity = new Vector3(charController.GetComponent<Rigidbody>().velocity.x, 0, charController.GetComponent<Rigidbody>().velocity.z);
        charController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        specialRigid.useGravity = false;
        floatCapped = false;
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
        playerUI.GetComponent<PlayerUserInterface>().UsedSpecialAttack();
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
    protected override void ActivateSpecialAttack()
    {
        SendMessage("OctaActivateSfx", true, SendMessageOptions.DontRequireReceiver);
        specialRigid.useGravity = true;
        specialRigid.angularDrag = angularDragAmount;
        leftFistCollider.radius = leftFistStartSize.radius;
        leftFistCollider.height = leftFistStartSize.height;
        rightFistCollider.radius = rightFistStartSize.radius;
        rightFistCollider.height = rightFistStartSize.height;
        UpdatePos(specialForm.transform, charController.transform);
        isAttacking = true;
        playerUI.GetComponent<PlayerUserInterface>().UsingSpecialAttack();
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
        specialForm.GetComponent<MeshCollider>().enabled = true;
        charController.GetComponent<UserControlMelee>().enabled = false;
        charController.GetComponent<CharacterMeleeDemo>().enabled = false;
        charController.GetComponent<CapsuleCollider>().enabled = false;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        charController.GetComponent<Rigidbody>().useGravity = false;
        SendMessage("PowerUpActive", true, SendMessageOptions.DontRequireReceiver);
    }

    /*
     * Overriding these functions as a way to turn off collisions for the arms and 
     * legs when fighting.
     */
    public override void ThrowSinglePunch(Limbs limb)
    {
        base.ThrowSinglePunch(limb);
        if (limb == Limbs.leftArm)
        {
            GameObject walker = leftShoulder;
            GameObject walker2 = rightShoulder; //need both for sake of combo
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
                walker2 = walker2.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null && walker2.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
        else
        {
            GameObject walker = rightShoulder;
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
    }

    public override void ThrowHiKick()
    {
        base.ThrowHiKick();
        GameObject walker = rightThigh;
        while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
        {
            walker.GetComponent<CollisionReceived>().sendDamage = false;
            //assumes there is only one child
            walker = walker.transform.GetChild(0).gameObject;
        }
        if (walker.GetComponent<CollisionReceived>() != null)
        {
            walker.GetComponent<CollisionReceived>().sendDamage = false;
        }
    }

    public override void ThrowUppercut(Limbs limb)
    {
        base.ThrowUppercut(limb);
        if (limb == Limbs.leftArm)
        {
            GameObject walker = leftShoulder;
            GameObject walker2 = rightShoulder; //need both for sake of combo
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
                walker2 = walker2.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null && walker2.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                walker2.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
        else
        {
            GameObject walker = rightShoulder;
            while (walker.transform.childCount > 0 && walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
                //assumes there is only one child
                walker = walker.transform.GetChild(0).gameObject;
            }
            if (walker.GetComponent<CollisionReceived>() != null)
            {
                walker.GetComponent<CollisionReceived>().sendDamage = false;
            }
        }
    }

    public override void SetAsPlayer2()
    {
        base.SetAsPlayer2();
        leftJabControllerButton = leftJabControllerButton + "_2";
        rightJabControllerButton = rightJabControllerButton + "_2";
        upperCutButton = upperCutButton + "_2";
        hiKickButton = hiKickButton + "_2";
        specialAttackButton = specialAttackButton + "_2";
        activateSpecialAttackButton = activateSpecialAttackButton + "_2";
        horizontalStick += "_2";
        verticalStick += "_2";
    }

}