using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using RootMotion;

public class OctahedronSpecials : PunchScript
{
    public float spinForceMult = 500f;

    CharacterMeleeDemo charMeleeDemoRef;
    UserControlThirdPerson userControlThirdPersonRef;
    CharacterThirdPerson charThirdPersonref;
    Rigidbody rb;

    //public CharacterController;
    // animClips = anim.runtimeAnimatorController.animationClips;

    private float currentTime = 0.0f;
    private float SpecialTime = 0.0f;
    private float coolDownTimer;
    private float octaForce;
    private float launchTime;
    private float launchLength;
    private float floatHeight;
    private float floatOffset;
    private float octahedronExtension;

    private bool executingSpecial = false;
    private bool specialIsSpeed = false;
    private bool tornadoMode = false;
    private bool isGrounded;

    private int isFloating;

    private OctahedronStats stats;

    private MeshCollider specialFormCollider;
    

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        charMeleeDemoRef = transform.GetComponentInChildren<CharacterMeleeDemo>();
        userControlThirdPersonRef = transform.GetComponentInChildren<UserControlThirdPerson>();
        charThirdPersonref = transform.GetComponentInChildren<CharacterThirdPerson>();
        //AnimatorStateInfo i = anim.GetNextAnimatorStateInfo(0);
        Rigidbody[] arr = transform.GetComponentsInChildren<Rigidbody>();
        rb = arr[11];

        stats = this.GetComponent<OctahedronStats>();
        baseStats = this.GetComponent<OctahedronStats>();
        specialFormCollider = specialForm.GetComponent<MeshCollider>();
        specialForm.GetComponent<MeshRenderer>().enabled = false;
        specialFormCollider.enabled = false;   // TEMPORARILY A CUBE COLLIDER UNTIL MESH IS FIXED
        specialRigid = specialForm.GetComponent<Rigidbody>();

        coolDownTimer = 0f;
        octaForce = 1000f;
        launchTime = 0;
        launchLength = 1f;
        floatHeight = 1.5f;
        floatOffset = 0.2f;
        specialRigid.useGravity = false;
        specialRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        octahedronExtension = 5f;
        specialStartSize = new Vector3(0.1f * octahedronExtension, 0.1f, 0.1f);
        specialEndSize = new Vector3(specialFormSize * octahedronExtension, specialFormSize, specialFormSize);

        isGrounded = checkIfGrounded();
        isFloating = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //anim.SetFloat("Forward", 1.0f);
        //Debug.Log(userControlThirdPersonRef.state.move + " forward: " + charMeleeDemoRef.transform.forward);

        if (growingSpecial)
        {
            GrowSpecial();
        }
        else if(playerGrowing)
        {
            GrowPlayer();
        }
        else if (specialForm.GetComponent<MeshRenderer>().enabled)
        {
            UpdatePos(charController.transform, specialForm.transform);
            coolDownTimer += Time.deltaTime;

            isFloating = checkIfFloating();
            //too high
            if (isFloating > 0) 
            {
                specialRigid.useGravity = true;
            }
            //too low
            else if(isFloating < 0)
            {
                specialRigid.useGravity = false;
                specialRigid.AddForce(Vector3.up * octaForce);
            }
            //in range
            else
            {
                specialRigid.velocity = new Vector3(specialRigid.velocity.x, 0f, specialRigid.velocity.z);
                specialRigid.useGravity = false;
            }
            if (coolDownTimer >= specialAttackActiveTime)
            {
                DeactivateSpecialAttack();
                UpdatePos(charController.transform, specialForm.transform);
                coolDownTimer = 0f;
            }
            if ((Input.GetKeyDown(specialAttack)))
            {
                DeactivateSpecialAttack();
                UpdatePos(charController.transform, specialForm.transform);
                coolDownTimer = 0f;
            }
            if (Input.GetKeyDown(useAttack) && specialForm.GetComponent<MeshRenderer>().enabled && !launched) //include jump key for controller
            {
                moveDir = Vector3.forward;
                moveDir = cam.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir = Vector3.Normalize(moveDir);
                moveDir.x = moveDir.x * octaForce * 100f;
                moveDir.z = moveDir.z * octaForce * 100f;
                specialRigid.AddForce(moveDir);
                //octaRigid.AddForce(Vector3.forward * octaForce * 100f);
                launched = true;
                specialRigid.AddForce(Vector3.up * octaForce * 2f);
            }
            else if(launched)
            {
                launchTime += Time.deltaTime;
                if(launchTime >= launchLength)
                {
                    launched = false;
                }
                moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                moveDir = cam.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir = Vector3.Normalize(moveDir);
                moveDir.x = moveDir.x * octaForce * stats.GetPlayerSpeed();
                moveDir.z = moveDir.z * octaForce * stats.GetPlayerSpeed();
                specialRigid.AddForce(moveDir);
                specialRigid.AddTorque(Vector3.up * octaForce * spinForceMult);
            }
            
        }
        else
        {
            UpdatePos(specialForm.transform, charController.transform);
            if ((Input.GetKeyDown(specialAttack)) && !specialForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
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

        /***************************************************************************************************/
        if (tornadoMode)
        {
            Vector3 vec = userControlThirdPersonRef.state.move;
            if (vec.x == 0)
            {
                vec.x = charMeleeDemoRef.transform.forward.x;
            }
            if (vec.y == 0)
            {
                vec.y = charMeleeDemoRef.transform.forward.y;
            }
            if (vec.z == 0)
            {
                vec.z = charMeleeDemoRef.transform.forward.z;
            }
            userControlThirdPersonRef.state.move = vec;
        }


        //vec.x = userControlThirdPersonRef.state.move.x;
        // vec.y = userControlThirdPersonRef.state.move.y;
        //userControlThirdPersonRef.state.move = vec;
        //charMeleeDemoRef.transform.Translate(transform.forward * 0.01f);

        //rb = charMeleeDemoRef.GetComponent<Rigidbody>();
        //rb.velocity = transform.forward * 5f;

        if (executingSpecial)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= SpecialTime)
            {
                currentTime = 0.0f;
                executingSpecial = false;
                tornadoMode = false;
                if (specialIsSpeed)
                {
                    anim.SetFloat("variableAnimSpeed", 1.0f);
                }
            }
        }

        if (Input.GetKey(KeyCode.V) && !executingSpecial)
        {
            anim.SetFloat("variableAnimSpeed", 2.0f);
            executingSpecial = true;
            SpecialTime = 3.0f;
            specialIsSpeed = true;
        }
        if (Input.GetKey(KeyCode.B) && !executingSpecial)
        {
            anim.SetFloat("variableAnimSpeed", 6.0f);
            executingSpecial = true;
            SpecialTime = 0.2f;
            specialIsSpeed = true;
        }
        //if (Input.GetKey(KeyCode.Space) && !executingSpecial) {
        //	//rb.AddForce (-transform.forward * 50000);
        //	rb.velocity = new Vector3(0.0f, 0.0f, 100f);
        //	//executingSpecial = true;
        //}

        if (Input.GetKey(KeyCode.G) && !executingSpecial)
        {
            //anim.SetFloat("Forward", 1.0f);
            //userControlThirdPersonRef.state.move = transform.forward;
            tornadoMode = true;

            executingSpecial = true;
            SpecialTime = 3.0f;
            specialIsSpeed = true;
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
    /// Returns less than 0 if within range of both upper and lower.</returns>
    private int checkIfFloating()
    {
        int result = 0;
        Vector3 endPoint = new Vector3(specialForm.transform.position.x, specialForm.transform.position.y - specialFormCollider.bounds.size.y * 2f , specialForm.transform.position.z);
        Debug.DrawLine(specialForm.transform.position, endPoint, Color.red, 5f);
        bool lower = Physics.Raycast(specialForm.transform.position, -Vector3.up, specialFormCollider.bounds.size.y * floatHeight);
        bool upper = Physics.Raycast(specialForm.transform.position, -Vector3.up, specialFormCollider.bounds.size.y * (floatHeight + floatOffset));
        if (lower && upper) //within cast range of both raycasts
        {
            result = -1;
        }
        else if(!lower && !upper) //outside both
        {
            result = 1;
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
        playerGrowing = true;
        charController.transform.localScale = playerStartSize;
        charController.GetComponent<Rigidbody>().velocity = new Vector3(charController.GetComponent<Rigidbody>().velocity.x, 0, charController.GetComponent<Rigidbody>().velocity.z);
        specialRigid.useGravity = false;
        launched = false;
        UpdatePos(charController.transform, specialForm.transform);
        //play animation of morphing into ball
        isAttacking = false;
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
    protected override void ActivateSpecialAttack()
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
        specialForm.GetComponent<MeshCollider>().enabled = true;
        charController.GetComponent<UserControlMelee>().enabled = false;
        charController.GetComponent<CharacterMeleeDemo>().enabled = false;
        charController.GetComponent<CapsuleCollider>().enabled = false;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        SendMessage("PowerUpActive", true, SendMessageOptions.DontRequireReceiver);
    }

}
