using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using Enemy;
using RootMotion.Dynamics;
using System.Collections;

namespace RootMotion.Demos
{
    /// <summary>
    /// User input for an AI controlled character controller.
    /// </summary>
    public class UserControlAI : UserControlThirdPerson
    {
        public int attackRandomAudio = 30;

        public GameObject safeSpot;

        public float stoppingDistance = 0.5f;
        public float stoppingThreshold = 1.5f;
        public float attackRange = 1f;
        public float jumpDistance = 10f;
        //public int behaviorIndex = 1;

        public IMovementBase movementStyle;
        public IAttackBase attackStyle;
        public Animator anim;
        public Transform goal;
        public GameObject moveTargetObj;
        public Transform moveTarget;
        public bool drop;
        public bool testingPath;

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

        private GameObject characterController;

        private float jumpThreshold = 1.0f;

        private string leftSwingAnimation = "SwingProp";
        private string rightSwingAnimation = "SwingProp";
        private string getUpProne = "GetUpProne";
        private string getUpSupine = "GetUpSupine";
        private string fall = "Fall";
        private string onGround = "OnGround";
        private Rigidbody physicBody;

        private bool dead;
        private bool canFind;
        private bool usingSpecial;
        private bool checkingRespawn;

        private void Awake()
        {
            movementStyle = GetComponent<IMovementBase>();
            attackStyle = GetComponent<IAttackBase>();
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
            //characterController = GetComponent<CharacterController>().gameObject;
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
            canFind = true;
            usingSpecial = false;
            checkingRespawn = false;
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
                    if (moveTargetObj != null)
                    {
                        agent.enabled = true;
                    }
                }
                else if ((!movementStyle.GetPlayerTarget() && moveTargetObj.transform.root.tag.Contains("Player")) || usingSpecial)
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

                attackStyle.Attack();

                if (agent.enabled && agent.isOnNavMesh && !((info.IsName(getUpProne) || info.IsName(getUpSupine) || info.IsName(fall)) && anim.GetBool(onGround)))
                {
                    Vector3 moveResult = movementStyle.Move();
                    //Check if can move and whether is moving or not
                    //
                    if (movementStyle.CanMove() && moveResult == transform.position && canFind)
                    {
                        Debug.Log("Cannot Move Trigger");
                        canFind = false;
                        StartCoroutine(Respawn());
                    }
                    if (moveResult != transform.position)
                    {
                        agent.destination = moveResult;
                        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                        {
                            state.move = agent.velocity;
                        }
                        else
                        {

                            if (testingPath && canFind)
                            {
                                Debug.Log("Path Missing Trigger");
                                canFind = false;
                               
                                StartCoroutine(Respawn());
                            }
                        }
                    }

                    else
                    {
                        physicBody.velocity = Vector3.zero;
                        state.move = Vector3.zero;
                    }

                }
                else
                {
                    state.move = Vector3.zero;
                }

                transform.rotation = movementStyle.RotateStyle();
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
            movementStyle.SetUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTargetObj);
            attackStyle.SetUp(stoppingDistance, stoppingThreshold, jumpDistance, moveTargetObj, characterPuppet, source, sfxManager, attackRange);
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


        private IEnumerator Respawn()
        {
            float timer = 0.5f;
            //yield return new WaitForSeconds(1f);
            while (!canFind)
            {
                //Debug.Log(timer);
                timer -= Time.deltaTime;
                yield return new WaitForSeconds(1f);
                if (timer <= 0)
                {
                    //if (safeSpot != null)
                    //{
                    //    transform.position = safeSpot.transform.position;
                    //}
                    //else
                    //{
                    //    DestroyObject(gameObject);
                    //}
                    Debug.Log("Triggered Respawn");

                    canFind = true;
                    timer = 5f;
                    agent.enabled = false;
                    transform.position = safeSpot.transform.position;
                    agent.enabled = true;
                    agent.nextPosition = safeSpot.transform.position;
                    
                    //DestroyObject(gameObject);
                    yield break;

                }
                else if (agent.enabled && agent.isOnNavMesh)
                {
                    NavMeshPath pathResult = new NavMeshPath();

                    agent.destination = movementStyle.Move();
                    agent.CalculatePath(movementStyle.Move(), pathResult);
                    //if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.destination != transform.position)
                    if (pathResult.status == NavMeshPathStatus.PathComplete && agent.destination != transform.position)
                    {
                        Debug.Log("Path Found reset");
                        canFind = true;
                    }
                }

                if (!movementStyle.CanMove())
                {
                    Debug.Log("Cannot Move reset");
                    canFind = true;
                }

            }
            //timer = 5f;
            

            yield break;
        }

        /// <summary>
        /// Allows toggling of if the enemy is using their special attack without needed to check in user 
        /// control AI every update loop.
        /// </summary>
        /// <param name="state"></param>
        public void SetUsingSpecial(bool state)
        {
            usingSpecial = state;

        }
    }
}
