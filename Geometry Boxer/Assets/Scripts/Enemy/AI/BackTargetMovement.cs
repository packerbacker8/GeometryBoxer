using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using UnityEngine.AI;

namespace Enemy
{
    public class backTargetMovement : MonoBehaviour, MovementBase
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
            if (canMove())
            {
                return moveTarget.transform.position - moveTarget.transform.forward;
            }
            return transform.position;
        }



        void MovementBase.setUp(float stopDist, float stopThresh, float jumpDis, Transform move)
        {
            stoppingDistance = stopDist;
            stoppingThreshold = stopThresh;
            jumpDistance = jumpDis;
            //anim = animator;
            moveTarget = move;

        }
        public void playerFound()
        {
            throw new NotImplementedException();
        }

        public void playerLost()
        {
            throw new NotImplementedException();
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
