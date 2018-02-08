using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;
using UnityEngine.AI;

public class NormalAttackAI : MonoBehaviour, AttackBase {

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

    private int swingAnimLayer = 1;
    private int punchAnimLayer = 0;
    private int animationControllerIndex = 0;
    private int characterControllerIndex = 2;

    public GameObject rightShoulder;
    public GameObject leftShoulder;
    public GameObject rightThigh;
    public GameObject leftThigh;

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
    // Use this for initialization
    void Start () {
        anim = gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
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
                //This is for when puppet has melee object in hand
                if (characterPuppet.propRoot.currentProp != null)
                {
                    anim.Play(rightSwingAnimation, swingAnimLayer);
                }
                else//No melee object in hand of puppet
                {
                    anim.Play(rightSwingAnimation, punchAnimLayer);
                }
            }
        }
    }

    public bool canAttack()
    {
        if(moveTargetObj == null)
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
    public void ThrowUppercut(Limbs limb)
    {
        //base.ThrowUppercut(limb);
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
}
