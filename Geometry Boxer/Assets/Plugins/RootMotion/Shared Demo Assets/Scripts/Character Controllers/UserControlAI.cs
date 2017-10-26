using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace RootMotion.Demos {
	
	/// <summary>
	/// User input for an AI controlled character controller.
	/// </summary>
	public class UserControlAI : UserControlThirdPerson {

		public Transform moveTarget;
		public float stoppingDistance = 0.5f;
		public float stoppingThreshold = 1.5f;
        
        public float attackRange = 1f;
        public Animator animator;

        public Transform goal;
        NavMeshAgent agent;

        //Sound Engine Needs
        private AudioSource source;
        private int attackIndex;
        private System.Random rand = new System.Random();
        private SFX_Manager sfxManager;

        private float jumpThreshold = 1.0f;

        void Start()
        {
            rand.Next(0, 1);
            source = gameObject.AddComponent<AudioSource>();
            sfxManager = FindObjectOfType<SFX_Manager>();
            agent = GetComponent<NavMeshAgent>();
        }

        protected override void Update () {
			float moveSpeed = walkByDefault? 0.5f: 1f;

            //Determine vector to rotate to target if not facing target
            Vector3 targetDir = moveTarget.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
            
            //Enemy is within distance to attack player AND is NOT already playing attack anim
            if (Vector3.Distance(moveTarget.position, this.transform.position) <= attackRange && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                if(rand.Next(0,10) == 1)//Chances of attack sound being played
                {
                    source.PlayOneShot(sfxManager.maleAttack[rand.Next(0, sfxManager.maleAttack.Count)]);
                }    
                animator.Play("Hit", 0);
            }
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

            //Always rotate to face the player
            transform.rotation = Quaternion.LookRotation(newDir);
        }
	}
}

