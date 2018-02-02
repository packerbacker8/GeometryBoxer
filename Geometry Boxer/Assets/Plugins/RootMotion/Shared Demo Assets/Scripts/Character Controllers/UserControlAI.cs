using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using Enemy;
using RootMotion.Dynamics;

namespace RootMotion.Demos
{

    /// <summary>
    /// User input for an AI controlled character controller.
    /// </summary>
    public class UserControlAI : UserControlThirdPerson
    {
        public int attackRandomAudio = 30;

        public float stoppingDistance = 0.5f;
        public float stoppingThreshold = 1.5f;
        public float attackRange = 1f;
        public float jumpDistance = 10f;
        //public int behaviorIndex = 1;

        public MovementBase movementStyle;
        public AttackBase attackStyle;
        public Animator anim;
        public Transform goal;
        public Transform moveTarget;
        public bool drop;

        private GameObject activePlayer;
        private GameObject[] playerOptions;

        private AudioSource source;
        private SFX_Manager sfxManager;
        private System.Random rand = new System.Random();

        private NavMeshAgent agent;
        private CharacterPuppet characterPuppet;
        private BehaviourPuppet behaviourPuppet;

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


        private bool dead;
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
            behaviourPuppet = transform.parent.gameObject.GetComponentInChildren<BehaviourPuppet>();

            anim = transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
            //agent.updatePosition = false; //New line automatically makes it where the agent no longer affects movement
            agent.nextPosition = transform.position;
            drop = false;
            movementStyle = GetComponent<MovementBase>();
            attackStyle = GetComponent<AttackBase>();
            movementStyle.setUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTarget);
            attackStyle.setUp(stoppingDistance, stoppingThreshold,
                jumpDistance, moveTarget, characterPuppet, source, sfxManager, attackRange);
            dead = false;
        }

        protected override void Update()
        {
            if (!dead)
            {
                //float moveSpeed = walkByDefault ? 1.0f : 1.5f;
                //Vector3 targetDir = moveTarget.position - transform.position;
                //Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
                AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

                if (!movementStyle.getPlayerTarget())
                {
                    agent.enabled = false;
                }
                else
                {
                    agent.enabled = true;
                }
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

                attackStyle.attack();
                if (agent.enabled && agent.isOnNavMesh && !((info.IsName(getUpProne) || info.IsName(getUpSupine) || info.IsName(fall)) && anim.GetBool(onGround)))
                {
                    agent.destination = movementStyle.move();
                    if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                    {
                        state.move = agent.velocity;
                    }
                }
                else
                {
                    state.move = Vector3.zero;
                }

                //transform.rotation = Quaternion.LookRotation(newDir);
                transform.rotation = movementStyle.rotateStyle();
            }
            else
            {
                state.move = Vector3.zero;
            }
        }

        /// <summary>
        /// Function to set player's move controller.
        /// </summary>
        /// <param name="move"></param>
        public void SetMoveTarget(Transform move)
        {
            moveTarget = move.GetChild(characterControllerIndex);
        }

        public void deathUpdate()
        {
            dead = true;
            agent.enabled = false;
        }

        /// <summary>
        /// Returns if the enemy is knocked down or active.
        /// </summary>
        /// <returns>If the enemy is in unpinned state returns true, otherwise false.</returns>
        public bool IsKnockedDown()
        {
            return behaviourPuppet.state == BehaviourPuppet.State.Unpinned;
        }
    }
}