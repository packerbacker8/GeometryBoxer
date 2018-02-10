﻿using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;
using UnityEngine.AI;

public class NormalAttackAI : MonoBehaviour, AttackBase {
    //protected float currentAnimLength;

    public GameObject rightShoulder;
    public GameObject leftShoulder;
    public GameObject rightThigh;
    public GameObject leftThigh;
    float stoppingDistance;
    float stoppingThreshold;
    float jumpDistance;
    Animator anim;
    GameObject moveTargetObj;
    Transform moveTarget;
    float attackRange;
    UserControlThirdPerson.State state;
    NavMeshAgent agent;
    CharacterPuppet characterPuppet;
    private AudioSource source;
    private SFX_Manager sfxManager;
    private System.Random rand = new System.Random();
    public int attackRandomAudio = 30;
    private string leftSwingAnimation = "SwingProp";
    private string rightSwingAnimation = "SwingProp";
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string fall = "Fall";
    private string onGround = "OnGround";
    private float currentAnimLength;

    private int swingAnimLayer = 1;
    private int punchAnimLayer = 0;
    private int animationControllerIndex = 0;
    private int characterControllerIndex = 2;
    private int[] attackChances = { 50, 30, 20 };

    private bool attackStatus;
    private bool updateCollisionCheck;

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
    void Start () {
        anim = gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        attackStatus = false;
        randAttack = new System.Random();
        updateCollisionCheck = false;
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
            if (rand.Next(0, attackRandomAudio) == 1 && sfxManager.maleAttack.Count > 0 && !source.isPlaying)
            {
                source.PlayOneShot(sfxManager.maleAttack[rand.Next(0, sfxManager.maleAttack.Count)]);
            }

            //If puppet is down, does not try to attack player during stand up anim
            if ((!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && !info.IsName(onGround)))
            {
                int randChoice = randAttack.Next(0,100);
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
        if(moveTargetObj == null || attackStatus)
        {
            return false;
        }
        return (Vector3.Distance(moveTarget.position, transform.position) <= attackRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) ;
    }

    public void setUp(float stopDist, float stopThresh, float jumpDis, 
        GameObject moveObj, CharacterPuppet charPup,  AudioSource src, 
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
        //throw new NotImplementedException();
    }
}
