using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;
using UnityEngine.AI;

public class SpecialCubeAttackAI : MonoBehaviour, AttackBase
{
    //protected float currentAnimLength;

    public GameObject rightShoulder;
    public GameObject leftShoulder;
    public GameObject rightThigh;
    public GameObject leftThigh;
    [Header("Special form attributes")]
    public GameObject cubeForm;
    public float specialFormSize = 4f;
    public float growSpeed = 3f;
    public float minCooldownTime = 10f;
    public float hangTimeBeforeLaunch = 0.5f;
    public float specialAttackForce = 1000f;
    public int specialAttackUses = 5;
    [Tooltip("The chances it is picked each update loop if not on cooldown.")]
    public int chances = 10;
    [Tooltip("Divides chances by this number to get the odds of picking the special attack each update call if not on cooldown.")]
    public int outOf = 1000;

    private float stoppingDistance;
    private float stoppingThreshold;
    private float jumpDistance;
    private float cooldownTimer;
    private float hangTime;
    private Animator anim;
    private GameObject moveTargetObj;
    private Transform moveTarget;
    private float attackRange;
    private UserControlThirdPerson.State state;
    private NavMeshAgent agent;
    private CharacterPuppet characterPuppet;
    private AudioSource source;
    private SFX_Manager sfxManager;
    private System.Random rand = new System.Random();
    private string leftSwingAnimation = "SwingProp";
    private string rightSwingAnimation = "SwingProp";
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string fall = "Fall";
    private string onGround = "OnGround";
    private float currentAnimLength;

    private Vector3 botStartSize;
    private Vector3 botFinalSize;
    private Vector3 specialStartSize;
    private Vector3 specialEndSize;
    private Vector3 deactivePos;
    private Rigidbody cubeRigid;
    private System.Random randChance;

    private int attackRandomAudio = 20;
    private int swingAnimLayer = 1;
    private int punchAnimLayer = 0;
    private int animationControllerIndex = 0;
    private int characterControllerIndex = 2;
    private int timesLaunched;
    private int[] attackChances = { 50, 30, 20 };

    private bool attackStatus;
    private bool updateCollisionCheck;
    private bool botGrowing;
    private bool launched;
    private bool onCooldown;
    private bool growingSpecial;
    private bool specialActivated;
    private bool isGrounded;

    private System.Random randAttack;



    public List<CharacterAnimations> enemyAnimations = new List<CharacterAnimations>();

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

    public enum Limbs
    {
        leftArm = 0,
        rightArm
    };

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

    // Use this for initialization
    void Start()
    {
        anim = gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        attackStatus = false;
        randAttack = new System.Random();
        updateCollisionCheck = false;

        cooldownTimer = 0;
        hangTime = 0;
        timesLaunched = 0;
        botStartSize = new Vector3(0.1f, 0.1f, 0.1f);
        botFinalSize = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        specialStartSize = new Vector3(0.1f, 0.1f, 0.1f);
        specialEndSize = new Vector3(specialFormSize, specialFormSize, specialFormSize);
        cubeRigid = cubeForm.GetComponent<Rigidbody>();
        randChance = new System.Random();
        randChance.Next();
        botGrowing = false;
        launched = false;
        onCooldown = false;
        growingSpecial = false;
        specialActivated = false;
        isGrounded = true;
        cubeForm.GetComponent<BoxCollider>().enabled = false;
        cubeForm.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        currentAnimLength -= Time.deltaTime;
        if (currentAnimLength <= 0f)
        {
            currentAnimLength = 0;
            attackStatus = false;
            updateCollisionCheck = true;
        }
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

            isGrounded = checkIfGrounded();
            if (isGrounded)
            {
                launched = false;
            }
            if (growingSpecial)
            {
                GrowCube();
            }
            else if (botGrowing)
            {
                GrowBot();
            }
            else if (cubeForm.GetComponent<MeshRenderer>().enabled)
            {
                UpdatePos(this.transform, cubeForm.transform);
                if (timesLaunched >= specialAttackUses)
                {
                    DeactivateCubeAttack();
                    UpdatePos(this.transform, cubeForm.transform);
                    timesLaunched = 0;
                }
                if (isGrounded && cubeForm.GetComponent<MeshRenderer>().enabled) //include jump key for controller
                {
                    cubeRigid.AddForce(Vector3.up * specialAttackForce); //launch up
                }
                else if (hangTime < hangTimeBeforeLaunch)
                {
                    hangTime += Time.deltaTime;
                    cubeForm.transform.LookAt(moveTarget);
                }
                else if (cubeForm.GetComponent<MeshRenderer>().enabled && !launched)
                {
                    hangTime = 0;
                    cubeRigid.AddForce(cubeForm.transform.forward * specialAttackForce);
                    launched = true;
                    timesLaunched++;
                }
            }
            else
            {
                //some random chance to turn into a cube, sometimes doesn't
                if (randChance.Next(outOf) < chances && !cubeForm.GetComponent<MeshRenderer>().enabled && !onCooldown)
                {
                    growingSpecial = true;
                    ActivateCubeAttack();
                    UpdatePos(cubeForm.transform, this.transform);
                }
            }

            if (onCooldown)
            {
                cooldownTimer += Time.deltaTime;
                if (cooldownTimer >= minCooldownTime)
                {
                    onCooldown = false;
                    cooldownTimer = 0f;
                }
            }
        }
    }

    protected virtual void SetCurrentAnimTime(CharacterAnimations currentAnim)
    {
        currentAnimLength = anim.GetCurrentAnimatorStateInfo(currentAnim.animLayer).length * currentAnim.playTime + (anim.GetCurrentAnimatorStateInfo(currentAnim.animLayer).length * currentAnim.transitionTime);
        attackStatus = true;
    }

    public void attack()
    {
        if (canAttack())
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (rand.Next(0, attackRandomAudio) == 1 && sfxManager.maleAttack.Count > 0 && !source.isPlaying && rand.Next(0, attackRandomAudio + 1) == attackRandomAudio)
            {
                source.PlayOneShot(sfxManager.maleAttack[rand.Next(0, sfxManager.maleAttack.Count)]);
            }

            //If puppet is down, does not try to attack player during stand up anim
            //also don't pick attack if already attacking (because of say special attack above)
            if ((!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && !info.IsName(onGround)) && !attackStatus)
            {
                int randChoice = randAttack.Next(0, 100);
                //This is for when puppet has melee object in hand
                if (characterPuppet.propRoot.currentProp != null)
                {
                    anim.Play(rightSwingAnimation, swingAnimLayer);
                }
                else//No melee object in hand of puppet
                {
                    //anim.Play(rightSwingAnimation, punchAnimLayer);
                    if (randChoice <= attackChances[0])
                    {
                        ThrowSinglePunch(Limbs.rightArm);
                    }
                    else if (randChoice >= attackChances[0] && randChoice < (attackChances[0] + attackChances[1]))
                    {
                        ThrowUppercut(Limbs.rightArm);
                    }
                    else if (randChoice >= (attackChances[0] + attackChances[1]))
                    {
                        ThrowHiKick();
                    }

                }
            }
        }
    }

    public bool canAttack()
    {
        if (moveTargetObj == null || attackStatus)
        {
            return false;
        }
        return (Vector3.Distance(moveTarget.position, transform.position) <= attackRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
    }

    public void setUp(float stopDist, float stopThresh, float jumpDis,
        GameObject moveObj, CharacterPuppet charPup, AudioSource src,
        SFX_Manager sfx, float rangeAttack)
    {
        stoppingDistance = stopDist;
        stoppingThreshold = stopThresh;
        jumpDistance = jumpDis;
        moveTargetObj = moveObj;
        moveTarget = moveTargetObj.transform;
        characterPuppet = charPup;
        source = src;
        sfxManager = sfx;
        attackRange = rangeAttack;
    }

    public void ThrowUppercut(Limbs limb)
    {
        CharacterAnimations currentAnim = InitCharacterAnimationStruct();
        foreach (CharacterAnimations action in enemyAnimations)
        {
            if (limb == Limbs.rightArm && action.animName == "RightUpperCut")
            {
                //rightFistCollider.enabled = true;
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
    public virtual void ThrowHiKick()
    {
        CharacterAnimations currentAnim = InitCharacterAnimationStruct();
        foreach (CharacterAnimations action in enemyAnimations)
        {
            if (action.animName == "HiKick")
            {
                currentAnim = action;
                break;
            }
        }
        anim.SetInteger("ActionIndex", currentAnim.actionIndex);
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime);
        SetCurrentAnimTime(currentAnim);
        anim.SetInteger("ActionIndex", -1);
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

    public virtual void ThrowSinglePunch(Limbs limb)
    {
        CharacterAnimations currentAnim = InitCharacterAnimationStruct();
        foreach (CharacterAnimations action in enemyAnimations)
        {
            if (limb == Limbs.rightArm && action.animName == "RightPunch")
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

        anim.SetInteger("ActionIndex", currentAnim.actionIndex);
        anim.CrossFadeInFixedTime(currentAnim.animName, currentAnim.transitionTime, currentAnim.animLayer, currentAnim.playTime);
        //going to need to determine when animation ends to allow next triggering event
        SetCurrentAnimTime(currentAnim);
        anim.SetInteger("ActionIndex", -1);

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
    public bool isAttacking()
    {
        return attackStatus;
    }


    /// <summary>
    /// Turn off the special attack and reactivate the player character.
    /// </summary>
    private void DeactivateCubeAttack()
    {
        specialActivated = false;
        this.GetComponent<UserControlAI>().SetUsingSpecial(specialActivated);
        botGrowing = true;
        this.transform.localScale = botStartSize;
        this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
        launched = false;
        attackStatus = false;
        updateCollisionCheck = true;
        UpdatePos(this.transform, cubeForm.transform);
        for (int i = 0; i < this.transform.parent.childCount; i++) 
        {
            if(this.transform.parent.GetChild(i).gameObject != this.gameObject && this.transform.parent.GetChild(i).gameObject != cubeForm)
            {
                this.transform.parent.GetChild(i).gameObject.SetActive(true);
            }
        }
        this.transform.GetChild(0).gameObject.SetActive(true);
        onCooldown = true;
        anim.SetInteger("ActionIndex", -1);
        anim.SetBool("IsStrafing", false);
        if (cubeRigid.velocity.sqrMagnitude > 0)
        {
            anim.SetFloat("Forward", 1);
        }
        else
        {
            anim.SetFloat("Forward", 0);
        }
        anim.Play("Grounded Directional");
    }

    /// <summary>
    /// Turn on the special attack and deactivate the player character.
    /// </summary>
    private void ActivateCubeAttack()
    {
        specialActivated = true;
        this.GetComponent<UserControlAI>().SetUsingSpecial(specialActivated);
        attackStatus = true;
        UpdatePos(cubeForm.transform, this.transform);
        //play animation of morphing into ball
        for (int i = 0; i < this.transform.parent.childCount; i++)
        {
            if (this.transform.parent.GetChild(i).gameObject != this.gameObject && this.transform.parent.GetChild(i).gameObject != cubeForm)
            {
                this.transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        this.transform.GetChild(0).gameObject.SetActive(false);
        cubeForm.GetComponent<BoxCollider>().enabled = true;
        cubeForm.GetComponent<MeshRenderer>().enabled = true;

        cubeForm.transform.localScale = specialStartSize;
        cubeForm.transform.localRotation = Quaternion.identity;
    }


    /// <summary>
    /// Raycast from center of special form to ground to see if on the ground.
    /// </summary>
    /// <returns>Returns true if on ground, false otherwise, with some tolerance.</returns>
    private bool checkIfGrounded()
    {
        Vector3 endPoint = new Vector3(cubeForm.transform.position.x, cubeForm.transform.position.y - cubeForm.GetComponent<BoxCollider>().size.y * 2f, cubeForm.transform.position.z);
        //Debug.DrawLine(specialForm.transform.position, endPoint, Color.red, 5f);
        //Debug.Log("Size now: " + specialForm.GetComponent<BoxCollider>().size.y);
        return Physics.Raycast(cubeForm.transform.position, -Vector3.up, cubeForm.GetComponent<BoxCollider>().size.y * cubeForm.transform.localScale.y + 0.1f);
    }

    /// <summary>
    /// 
    /// </summary>
    private void GrowCube()
    {
        cubeForm.transform.localScale += specialStartSize * growSpeed;
        if (cubeForm.transform.localScale.magnitude >= specialEndSize.magnitude)
        {
            cubeForm.transform.localScale = specialEndSize;
            growingSpecial = false;
        }
    }

    /// <summary>
    /// Method that grows the player and shrinks the special form at the same time.
    /// This allows the feeling of changing form.
    /// Defaults to box collider.
    /// </summary>
    private void GrowBot()
    {
        this.transform.localScale += botStartSize;
        cubeForm.transform.localScale -= specialStartSize * growSpeed;
        UpdatePos(cubeForm.transform, this.transform);
        if (CompareGreaterThanEqualVectors(this.transform.localScale, botFinalSize))
        {
            this.transform.localScale = botFinalSize;
            cubeForm.transform.position = deactivePos;
            cubeForm.transform.localScale = specialStartSize;
            cubeRigid.velocity = Vector3.zero;
            cubeForm.GetComponent<BoxCollider>().enabled = false;
            cubeForm.GetComponent<MeshRenderer>().enabled = false;
            botGrowing = false;
        }
        if (CompareLessThanEqualVectors(cubeForm.transform.localScale, specialStartSize))
        {
            cubeForm.transform.localScale = specialStartSize;
        }
    }

    private bool CompareLessThanEqualVectors(Vector3 v1, Vector3 v2)
    {
        return v1.x <= v2.x && v1.y <= v2.y && v1.z <= v2.z;
    }

    private bool CompareGreaterThanEqualVectors(Vector3 v1, Vector3 v2)
    {
        return v1.x >= v2.x && v1.y >= v2.y && v1.z >= v2.z;
    }

    /// <summary>
    /// Update the position of one transform to the target transform of another game object.
    /// This specifically accounts bonus movement of the special form upwards to avoid clipping 
    /// through the floor when spawning.
    /// </summary>
    /// <param name="transformToUpdate">The transform object to move.</param>
    /// <param name="targetTransform">The transform object to move the other object to.</param>
    private void UpdatePos(Transform transformToUpdate, Transform targetTransform)
    {
        Vector3 targetVec = targetTransform.position;
        transformToUpdate.rotation = Quaternion.Euler(0, transformToUpdate.eulerAngles.y, 0);
        if (transformToUpdate == cubeForm.transform)
        {
            targetVec = new Vector3(targetVec.x, targetVec.y + 1f, targetVec.z);
        }
        transformToUpdate.position = targetVec;
    }

}
