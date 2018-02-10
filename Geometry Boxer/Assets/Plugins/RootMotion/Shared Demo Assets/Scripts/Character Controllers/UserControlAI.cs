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
        public GameObject moveTargetObj;
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
        private Rigidbody physicBody;

        private bool dead;

        private void Awake()
        {
            movementStyle = GetComponent<MovementBase>();
            attackStyle = GetComponent<AttackBase>();
            characterPuppet = GetComponent<CharacterPuppet>();
            playerOptions = new GameObject[3];
            rand.Next(0, 1);
            source = gameObject.AddComponent<AudioSource>();
            source.spatialize = true;
            source.volume = 0.6f;
            sfxManager = FindObjectOfType<SFX_Manager>();
            agent = GetComponent<NavMeshAgent>();
            behaviourPuppet = transform.parent.gameObject.GetComponentInChildren<BehaviourPuppet>();
            physicBody = GetComponent<Rigidbody>();
            anim = transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();

        }

        void Start()
        {

            //agent.updatePosition = false; //New line automatically makes it where the agent no longer affects movement
            agent.nextPosition = transform.position;
            drop = false;
            //movementStyle = GetComponent<MovementBase>();
            //attackStyle = GetComponent<AttackBase>();
            //movementStyle.setUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTarget);
            //attackStyle.setUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTarget, characterPuppet, source, sfxManager, attackRange);
            dead = false;
        }

        protected override void Update()
        {
            if (!dead)   //&& moveTargetObj != null)
            {
                //float moveSpeed = walkByDefault ? 1.0f : 1.5f;
                //Vector3 targetDir = moveTarget.position - transform.position;
                //Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
                AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

                if (moveTargetObj == null)   
                {
                    agent.enabled = false;
                    movementStyle.UpdateTarget();
                    if(moveTargetObj != null)
                    {
                        agent.enabled = true;
                    }
                }
                else if (!movementStyle.getPlayerTarget() && moveTargetObj.transform.root.tag.Contains("Player"))
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
                else if (!agent.enabled || drop)
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
                        if (agent.destination == transform.position)
                        {
                            physicBody.velocity = Vector3.zero;
                            state.move = Vector3.zero;
                        }
                        else
                        {
                            state.move = agent.velocity;
                        }
                        
                       
                    }
                }
                else
                {
                    state.move = Vector3.zero;
                }

                    transform.rotation = movementStyle.rotateStyle();
            }
            else
            {
                state.move = Vector3.zero;
            }
        }

        /// <summary>
        /// Function to set the move object of the enemies. This then makes it the move target transform
        /// they look to find and attack.
        /// </summary>
        /// <param name="move">Expects this game object to be the character controller of the combatant, as that is the part that actually moves.</param>
        public void SetMoveTarget(GameObject moveObj)
        {
            moveTargetObj = moveObj;
            moveTarget = moveTargetObj.transform;
            movementStyle.setUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTargetObj);
            attackStyle.setUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTargetObj, characterPuppet, source, sfxManager, attackRange);
        }

        public void deathUpdate()
        {
            dead = true;
            agent.enabled = false;
            Destroy(this);
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