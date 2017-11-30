/*using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using Enemy;

namespace RootMotion.Demos
{

    /// <summary>
    /// User input for an AI controlled character controller.
    /// </summary>
    public class UserControlAI_Orig : UserControlThirdPerson
    {
        public int attackRandomAudio = 30;

        public float stoppingDistance = 0.5f;
        public float stoppingThreshold = 1.5f;
        public float attackRange = 1f;
        public float jumpDistance = 10f;
        public int behaviorIndex = 1;

        public Animator anim;
        public Transform goal;
        public Transform moveTarget;
        public bool drop;

        public MovementBase moveStyle;

        [SerializeField]
        private EnemyBehavior enemyAI;  //Might Remove this Later
        private delegate void AIBehavior();
        AIBehavior ai;

        private GameObject activePlayer;
        private GameObject[] playerOptions;
        private AudioSource source;
        private SFX_Manager sfxManager;
        private System.Random rand = new System.Random();

        private NavMeshAgent agent;
        private CharacterPuppet characterPuppet;

        private int attackIndex;
        private int swingAnimLayer = 1;
        private int punchAnimLayer = 0;
        private int animationControllerIndex = 0;
        private int characterControllerIndex = 2;

        private float jumpThreshold = 1.0f;

        private string leftSwingAnimation = "SwingProp";
        private string rightSwingAnimation = "SwingProp";
        private string getUpProne = "GetUpProne";
        private string getUpSupine = "GetUpSupine";
        private string fall = "Fall";
        private string onGround = "OnGround";

        void Start()
        {
            playerOptions = new GameObject[3];
            rand.Next(0, 1);
            source = gameObject.AddComponent<AudioSource>();
            source.spatialize = true;
            source.volume = 0.6f;
            sfxManager = FindObjectOfType<SFX_Manager>();
            agent = GetComponent<NavMeshAgent>();
            characterPuppet = GetComponent<CharacterPuppet>();
            anim = this.gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
            //agent.updatePosition = false; //New line automatically makes it where the agent no longer affects movement
            agent.nextPosition = transform.position;
            drop = false;

        }

        protected override void Update()
        {
            AIbehavior(behaviorIndex);
        }

        /// <summary>
        /// Function to set player's move controller.
        /// </summary>
        /// <param name="move"></param>
        public void SetMoveTarget(Transform move)
        {
            moveTarget = move.GetChild(characterControllerIndex);
        }

        private void AIbehavior(int behavior)
        {
            float moveSpeed = walkByDefault ? 0.5f : 1f;

            //Determine vector to rotate to target if not facing target
            Vector3 targetDir = moveTarget.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            switch (behavior)
            {
                case 1:
                    jumpBehavior(targetDir, newDir, info);
                    break;
                case 2:
                    break;
                default:
                    defaultBehavior(targetDir, newDir, info);
                    break;
            }

        }

        private void defaultBehavior(Vector3 targetDir, Vector3 newDir, AnimatorStateInfo info)
        {
            if (!(!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && anim.GetBool(onGround)))
            {
                if (!agent.isOnOffMeshLink)
                {
                    agent.nextPosition = transform.position;
                    agent.enabled = false;
                }
                else
                {
                    drop = true;
                }

            }
            else if (!agent.enabled)
            {
                drop = false;
                agent.enabled = true;
                agent.nextPosition = transform.position;
            }

            if (Vector3.Distance(moveTarget.position, this.transform.position) <= attackRange && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {

                //Rand controls chance of random attack sound being played, only while source is not already playing
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
            if (agent.enabled)
            {
                if (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance)
                {
                    agent.destination = moveTarget.position;
                    state.move = agent.velocity;
                }
                else
                {
                    agent.destination = transform.position;
                    state.move = agent.velocity;
                }
            }

            //Always rotate to face the player
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        private void jumpBehavior(Vector3 targetDir, Vector3 newDir, AnimatorStateInfo info)
        {
            if (!(!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && anim.GetBool(onGround)) && !agent.isOnOffMeshLink)
            {
                Debug.Log("Falling");
                //agent.updatePosition = false;
                //agent.nextPosition = transform.position;
                agent.nextPosition = transform.position;
                agent.enabled = false;
                drop = false;

            }
            else if (!agent.enabled)
            {

                //agent.updatePosition = true;
                drop = false;
                agent.enabled = true;
                agent.nextPosition = transform.position;
            }
            //agent.updatePosition = false; //New line automatically makes it where the agent no longer affects movement
            if (Vector3.Distance(moveTarget.position, this.transform.position) <= attackRange && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {

                //Rand controls chance of random attack sound being played, only while source is not already playing
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
            if (agent.enabled)
            {
                if (Vector3.Distance(moveTarget.position, transform.position) < jumpDistance + jumpThreshold && Vector3.Distance(moveTarget.position, transform.position) > jumpDistance - jumpThreshold)
                {

                    agent.enabled = false;
                    state.jump = true;
                }
                else if (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance)
                {
                    //agent.updatePosition = true; 
                    agent.destination = moveTarget.position;
                    state.move = agent.velocity;
                    //agent.Move(agent.velocity);
                }
                else
                {
                    agent.destination = transform.position;
                    state.move = agent.velocity;
                }
            }

            //Always rotate to face the player
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
}*/