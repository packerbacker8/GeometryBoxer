using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using RootMotion;

public class OctahedronSpecials : PunchScript
{
    public GameObject octaForm;
    public KeyCode specialAttack = KeyCode.B;
    public KeyCode useAttack = KeyCode.Space;
    public float specialAttackActiveTime = 10f;
    public float specialAttackCooldownTime = 2f;

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

    private bool executingSpecial = false;
    private bool specialIsSpeed = false;
    private bool tornadoMode = false;
    private bool onCooldown;
    private bool growingOcta;
    private bool launched;
    private bool isGrounded;
    private bool isFloating;

    private OctahedronStats stats;
    private Vector3 startOctaSize;
    private Vector3 endOctaSize;
    private Vector3 moveDir;
    private Rigidbody octaRigid;

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

        isGrounded = checkIfGrounded();
        isFloating = false;
        startOctaSize = new Vector3(0.1f, 0.1f, 0.1f);
        endOctaSize = new Vector3(2f, 2f, 2f);
        stats = this.GetComponent<OctahedronStats>();
        octaForm.GetComponent<MeshRenderer>().enabled = false;
        octaForm.GetComponent<BoxCollider>().enabled = false;   // TEMPORARILY A CUBE COLLIDER UNTIL MESH IS FIXED
        octaRigid = octaForm.GetComponent<Rigidbody>();
        onCooldown = false;
        growingOcta = false;
        launched = false;
        coolDownTimer = 0f;
        octaForce = 1000f;
        launchTime = 0;
        launchLength = 1f;
        octaRigid.useGravity = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //anim.SetFloat("Forward", 1.0f);
        //Debug.Log(userControlThirdPersonRef.state.move + " forward: " + charMeleeDemoRef.transform.forward);

        if (growingOcta)
        {
            octaForm.transform.localScale += startOctaSize;
            if (octaForm.transform.localScale.magnitude >= endOctaSize.magnitude)
            {
                octaForm.transform.localScale = endOctaSize;
                growingOcta = false;
            }
        }
        else if (octaForm.GetComponent<MeshRenderer>().enabled)
        {
            UpdatePos(charController.transform, octaForm.transform);
            coolDownTimer += Time.deltaTime;
            octaRigid.useGravity = false;

            isFloating = checkIfFloating();
            if(isFloating) //combine two raycasts to see if within certain range, make sure doesn't go too high or too low
            {
                octaRigid.velocity = new Vector3(octaRigid.velocity.x, 0f, octaRigid.velocity.z);
            }
            else
            {
                octaRigid.AddForce(Vector3.up * octaForce);
            }
            if (coolDownTimer >= specialAttackActiveTime)
            {
                DeactivateOctaAttack();
                UpdatePos(charController.transform, octaForm.transform);
                coolDownTimer = 0f;
            }
            if ((Input.GetKeyDown(specialAttack)))
            {
                DeactivateOctaAttack();
                UpdatePos(charController.transform, octaForm.transform);
                coolDownTimer = 0f;
            }
            if (Input.GetKeyDown(useAttack) && octaForm.GetComponent<MeshRenderer>().enabled && !launched) //include jump key for controller
            {
                moveDir = Vector3.forward;
                moveDir = cam.transform.TransformDirection(moveDir);
                moveDir.y = 0;
                moveDir = Vector3.Normalize(moveDir);
                moveDir.x = moveDir.x * octaForce * 100f;
                moveDir.z = moveDir.z * octaForce * 100f;
                octaRigid.AddForce(moveDir);
                //octaRigid.AddForce(Vector3.forward * octaForce * 100f);
                launched = true;
                octaRigid.AddForce(Vector3.up * octaForce * 2f);
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
                octaRigid.AddForce(moveDir);
                octaRigid.AddTorque(Vector3.up * octaForce * 100f);
            }
            
        }
        else
        {
            UpdatePos(octaForm.transform, charController.transform);
            if ((Input.GetKeyDown(specialAttack)) && !octaForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
            {
                growingOcta = true;
                ActivateOctaAttack();
                UpdatePos(octaForm.transform, charController.transform);
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


        //Debug.Log(anim.GetFloat("Forward"));

    }

    /// <summary>
    /// Raycast from center of octahedron to ground to see if on the ground.
    /// </summary>
    /// <returns>Returns true if on ground, false otherwise, with some tolerance.</returns>
    private bool checkIfGrounded()
    {
        return Physics.Raycast(octaForm.transform.position, -Vector3.up, octaForm.GetComponent<BoxCollider>().size.y + 0.1f);
    }

    /// <summary>
    /// Check to see if the octahedron is floating above the ground.
    /// </summary>
    /// <returns>Returns true if the octahedron is greater than a certain distance from the ground, false otherwise.</returns>
    private bool checkIfFloating()
    {
        Vector3 endPoint = new Vector3(octaForm.transform.position.x, octaForm.transform.position.y - octaForm.GetComponent<BoxCollider>().size.y * 2f , octaForm.transform.position.z);
        Debug.DrawLine(octaForm.transform.position, endPoint, Color.red, 5f);
        return !Physics.Raycast(octaForm.transform.position, -Vector3.up, octaForm.GetComponent<BoxCollider>().size.y * 2f);
    }

    /// <summary>
    /// Update the position of one transform to the target transform of another game object.
    /// This specifically accounts bonus movement of the octahedron upwards to avoid clipping 
    /// through the floor when spawning.
    /// </summary>
    /// <param name="transformToUpdate">The transform object to move.</param>
    /// <param name="targetTransform">The transform object to move the other object to.</param>
    private void UpdatePos(Transform transformToUpdate, Transform targetTransform)
    {
        Vector3 targetVec = targetTransform.position;
        transformToUpdate.rotation = Quaternion.identity;
        if (transformToUpdate == octaForm.transform)
        {
            targetVec = new Vector3(targetVec.x, targetVec.y + 3f, targetVec.z);
        }
        transformToUpdate.position = targetVec;
    }

    /// <summary>
    /// Turn off the special octahedron attack and reactivate the player character.
    /// </summary>
    private void DeactivateOctaAttack()
    {
        octaRigid.useGravity = false;
        launched = false;
        UpdatePos(charController.transform, octaForm.transform);
        octaForm.transform.localScale = startOctaSize;
        //play animation of morphing into ball
        isAttacking = false;
        for (int i = 0; i < this.transform.childCount; i++) //move camera back to player here
        {
            if (this.transform.GetChild(i).gameObject != octaForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = stats.pelvisJoint.transform;
                }
                else if (this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        //ballForm.SetActive(false);
        octaForm.GetComponent<MeshRenderer>().enabled = false;
        octaForm.GetComponent<BoxCollider>().enabled = false;
        charController.GetComponent<UserControlMelee>().enabled = true;
        charController.GetComponent<CharacterMeleeDemo>().enabled = true;
        charController.GetComponent<CapsuleCollider>().enabled = true;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        onCooldown = true;
        anim.SetInteger("ActionIndex", -1);
        anim.SetBool("IsStrafing", false);
        if (octaRigid.velocity.sqrMagnitude > 0)
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
    /// Turn on the special octahedron attack and deactivate the player character.
    /// </summary>
    private void ActivateOctaAttack()
    {
        octaRigid.useGravity = true;
        leftFistCollider.radius = leftFistStartSize.radius;
        leftFistCollider.height = leftFistStartSize.height;
        rightFistCollider.radius = rightFistStartSize.radius;
        rightFistCollider.height = rightFistStartSize.height;
        UpdatePos(octaForm.transform, charController.transform);
        isAttacking = true;
        //play animation of morphing into ball
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject != octaForm)
            {
                if (this.transform.GetChild(i).gameObject.tag == "MainCamera") //move camera to follow ball here
                {
                    this.transform.GetChild(i).gameObject.GetComponent<CameraController>().target = octaForm.transform;
                }
                else if (this.transform.GetChild(i).gameObject != charController)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        octaForm.SetActive(true);
        charController.SetActive(true);
        octaForm.GetComponent<MeshRenderer>().enabled = true;
        octaForm.GetComponent<BoxCollider>().enabled = true;
        charController.GetComponent<UserControlMelee>().enabled = false;
        charController.GetComponent<CharacterMeleeDemo>().enabled = false;
        charController.GetComponent<CapsuleCollider>().enabled = false;
        charController.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        SendMessage("PowerUpActive", true, SendMessageOptions.DontRequireReceiver);
    }
}
