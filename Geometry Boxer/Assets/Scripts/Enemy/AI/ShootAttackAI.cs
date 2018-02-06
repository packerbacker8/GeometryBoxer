using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using RootMotion.Demos;
using UnityEngine.AI;

public class ShootAttackAI : MonoBehaviour, AttackBase
{
    float stoppingDistance;
    float stoppingThreshold;
    float jumpDistance;
    Animator anim;
    private GameObject moveTargetObj;
    Transform moveTarget;
    float attackRange;
    UserControlThirdPerson.State state;
    NavMeshAgent agent;
    CharacterPuppet characterPuppet;
    private AudioSource source;
    private SFX_Manager sfxManager;
    private System.Random rand = new System.Random();
    public GameObject projectile;
    public Transform summonPoint;
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

    private bool hasShot = false;

    private GameObject _player;
    // Use this for initialization
    void Start()
    {
        GameObject _player = GameObject.Find("SphereChar_Player");
        anim = gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

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
            if ((!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && !info.IsName(onGround) && !hasShot))
            {
                anim.Play(rightSwingAnimation, punchAnimLayer);
                Vector3 parentPos = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
                GameObject bullet = Instantiate(projectile,summonPoint.position,Quaternion.identity);
//                _player = GameObject.Find("SphereChar_Player");
//                Vector3 test = _player.transform.position;
//                Debug.Log((_player.transform.position - parentPos).ToString());
//                bullet.GetComponent<Rigidbody>().AddForce((_player.transform.position - parentPos).normalized * 1000);
                hasShot = true;
            }
        }
    }

    public bool canAttack()
    {
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
}
