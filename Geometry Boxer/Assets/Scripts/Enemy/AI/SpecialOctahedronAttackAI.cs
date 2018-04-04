using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;
using UnityEngine.AI;

public class SpecialOctahedronAttackAI : MonoBehaviour, IAttackBase
{
    public AudioClip specialAttackThrow;
    public GameObject rightShoulder;
    public GameObject leftShoulder;
    public GameObject rightThigh;
    public GameObject leftThigh;
    public List<CharacterAnimations> enemyAnimations = new List<CharacterAnimations>();
    [Header("Special Attack Properties")]
    [Tooltip("The prefab that the spawned projectile will be based on.")]
    public GameObject projectilePrefab;
    public GameObject projectileContainer;
    public float minCooldownTime = 10f;
    public float specialAttackForce = 1000f;
    [Tooltip("How long the projectile will last in the air.")]
    public float timeInAir = 2f;
    [Tooltip("Number of projectiles on the screen at once.")]
    public int numberOfProjectiles = 10;
    [Tooltip("Number of projectiles fired before attack stops.")]
    public int numberShotsFired = 30;
    [Tooltip("The chances it is picked each update loop if not on cooldown.")]
    public int chances = 10;
    [Tooltip("Divides chances by this number to get the odds of picking the special attack each update call if not on cooldown.")]
    public int outOf = 1000;

    private float stoppingDistance;
    private float stoppingThreshold;
    private float jumpDistance;
    private float attackRange;
    private float currentAnimLength;
    private float cooldownTimer;

    private Animator anim;
    private GameObject moveTargetObj;
    private Transform moveTarget;
    private UserControlThirdPerson.State state;
    private NavMeshAgent agent;
    private CharacterPuppet characterPuppet;
    private AudioSource source;
    private SFX_Manager sfxManager;
    private static System.Random rand = new System.Random();
    private static System.Random randAttack;
    private static System.Random randChance;
    private List<GameObject> projectiles;
    private Vector3 startProjectileSize;

    private string leftSwingAnimation = "SwingProp";
    private string rightSwingAnimation = "SwingProp";
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string fall = "Fall";
    private string onGround = "OnGround";

    private int attackRandomAudio = 20;
    private int swingAnimLayer = 1;
    private int punchAnimLayer = 0;
    private int animationControllerIndex = 0;
    private int characterControllerIndex = 2;
    private int projectilesLaunched;
    private int totalProjectilesLaunched;
    private int[] attackChances = { 50, 30, 20 };

    private bool attackStatus;
    private bool updateCollisionCheck;
    private bool onCooldown;
    private bool specialLaunched;

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
        source.spatialBlend = 0.3f;
        anim = gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        attackStatus = false;
        randAttack = new System.Random();
        randChance = new System.Random();
        randChance.Next();
        updateCollisionCheck = false;
        onCooldown = false;
        specialLaunched = false;
        projectiles = new List<GameObject>();
        projectilesLaunched = 0;
        totalProjectilesLaunched = 0;
        startProjectileSize = projectilePrefab.transform.localScale;
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            //parented to the bot object as a whole rather than the container or character controller
            projectiles.Add(Instantiate(projectilePrefab, Vector3.forward, Quaternion.identity, this.transform.parent)); //this.transform.position +  might need to be used for positioning
            if(this.tag.Contains("Ally"))
            {
                projectiles[i].tag = "Ally";
            }
            projectiles[i].SetActive(false);
        }

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
        }

        if(specialLaunched)
        {
            if(totalProjectilesLaunched >= numberShotsFired)
            {
                specialLaunched = false;
                attackStatus = false;
                onCooldown = true;                
            }
            else
            {
                if (projectilesLaunched < numberOfProjectiles)
                {
                    //RotateTheBot();
                    RotateLaunchPoint();
                    FireProjectile();
                }
            }
        }
        else if(!onCooldown && randChance.Next(outOf) < chances)
        {
            specialLaunched = true;
            attackStatus = true;
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            //RotateTheBot();
            RotateLaunchPoint();
            FireProjectile();
        }
        if (onCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= minCooldownTime)
            {
                onCooldown = false;
                cooldownTimer = 0f;
                totalProjectilesLaunched = 0;
                projectilesLaunched = 0;
            }
        }

    }

    /// <summary>
    /// Cause the launch point of the projectile to rotate.
    /// </summary>
    private void RotateLaunchPoint()
    {
        projectileContainer.transform.Rotate(Vector3.up, 360f / projectilesLaunched);
    }

    /// <summary>
    /// Cause the bot to spin as if shooting at projectiles from the spin.
    /// </summary>
    private void RotateTheBot()
    {
        this.transform.Rotate(Vector3.up * 10);
    }

    /// <summary>
    /// Fire a single projectile prefab out from the bot.
    /// </summary>
    private void FireProjectile()
    {
        if(projectilesLaunched < numberOfProjectiles)
        {
            if(!source.isPlaying)
            {
                source.PlayOneShot(specialAttackThrow, 1f);
            }
            int currentProjectile = totalProjectilesLaunched % numberOfProjectiles;
            projectilesLaunched++;
            totalProjectilesLaunched++;
            projectiles[currentProjectile].SetActive(true);
            projectiles[currentProjectile].transform.localScale = startProjectileSize;
            float angle = projectileContainer.transform.localRotation.eulerAngles.y * (Mathf.PI/180);
            projectiles[currentProjectile].transform.position = new Vector3(projectileContainer.transform.position.x + Mathf.Cos(angle), projectileContainer.transform.position.y, projectileContainer.transform.position.z + Mathf.Sin(angle)); // might be slightly less
            Vector3 launchDir = projectiles[currentProjectile].transform.position - projectileContainer.transform.position;
            projectiles[currentProjectile].GetComponent<LaunchAndReset>().Launch(specialAttackForce, timeInAir, launchDir);
        }
    }

    /// <summary>
    /// Gets called when the projectile was reset from being launched.
    /// </summary>
    public void ProjectileWasReset()
    {
        projectilesLaunched--;
    }

    protected virtual void SetCurrentAnimTime(CharacterAnimations currentAnim)
    {
        currentAnimLength = anim.GetCurrentAnimatorStateInfo(currentAnim.animLayer).length * currentAnim.playTime + (anim.GetCurrentAnimatorStateInfo(currentAnim.animLayer).length * currentAnim.transitionTime);
        attackStatus = true;
    }

    public void Attack()
    {
        if (CanAttack())
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (rand.Next(0, attackRandomAudio) == 1 && sfxManager.maleAttack.Count > 0 && !source.isPlaying && rand.Next(0, attackRandomAudio + 1) == attackRandomAudio)
            {
                source.PlayOneShot(sfxManager.maleAttack[rand.Next(0, sfxManager.maleAttack.Count)]);
            }

            //If puppet is down, does not try to attack player during stand up anim
            if ((!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && !info.IsName(onGround)))
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

    public bool CanAttack()
    {
        if (moveTargetObj == null || attackStatus)
        {
            return false;
        }
        return (Vector3.Distance(moveTarget.position, transform.position) <= attackRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
    }

    public void SetUp(float stopDist, float stopThresh, float jumpDis,
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
    public bool IsAttacking()
    {
        return attackStatus;
        //throw new NotImplementedException();
    }
}
