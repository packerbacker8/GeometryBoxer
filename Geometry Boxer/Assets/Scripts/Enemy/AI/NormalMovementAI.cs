using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using UnityEngine.AI;

namespace Enemy
{
    public class NormalMovementAI : MonoBehaviour, MovementBase
    {
        float stoppingDistance;
        float stoppingThreshold;
        float jumpDistance;
        Transform moveTarget;
        UserControlThirdPerson.State state;
        


        public bool canMove()
        {
            return (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance);
        }

        public Vector3 move()
        {
                if (canMove()) {
                    //agent.destination = moveTarget.position;
                    //state.move = agent.velocity;
                    return moveTarget.position;
                }
            return transform.position;
        }
        //public void move()
        //{
        //    if (agent.enabled)
        //    {
        //        if (canMove())
        //        {
        //            agent.destination = moveTarget.position;
        //            //return moveTarget.position;
        //        }
        //        else
        //        {
        //            agent.destination = transform.position;
        //            //return transform.position;
        //        }
        //    }
        //    //baseState.move = agent.velocity;
        //    //return baseState;
        //    //return transform.position;
        //}


  

        void MovementBase.setUp(float stopDist, float stopThresh, float jumpDis, Transform move)
        {
            stoppingDistance = stopDist;
            stoppingThreshold = stopThresh;
            jumpDistance = jumpDis;
            //anim = animator;
            moveTarget = move;
            
        }




        // Use this for initialization
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
 