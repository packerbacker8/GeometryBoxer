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
        //public GameObject player;
        NavMeshAgent agent;
        void Start()
        {
            //player = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            //agent.destination = player.transform.position;
        }

        protected override void Update () {
			float moveSpeed = walkByDefault? 0.5f: 1f;

            if (Vector3.Distance(moveTarget.position, this.transform.position) <= attackRange)
            {
                animator.Play("Hit", 0);
                
            }

                //Vector3 direction = Vector3.Distance(player.transform.position, transform.position);
                //float distance = direction.magnitude;

                //Vector3 normal = transform.up;
                //Vector3.OrthoNormalize(ref normal, ref direction);

                //float sD = state.move != Vector3.zero? stoppingDistance: stoppingDistance * stoppingThreshold;

                //agent.destination = distance > sD? player.transform.position: transform.position;


                //agent.destination =  moveTarget.position;
                //agent.destination = 
                //agent.velocity = state.move;
                if (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance)
                {
                agent.destination = moveTarget.position;
                    state.move = agent.velocity;
                }
                else
                {
                agent.destination = transform.position;
                //Debug.Log("Close To player");
                state.move = agent.velocity;
                }
            
        }
	}
}

