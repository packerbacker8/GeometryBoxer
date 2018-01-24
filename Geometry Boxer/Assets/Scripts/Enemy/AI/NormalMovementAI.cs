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
            if (canMove())
            {
                return moveTarget.position;
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

        void MovementBase.playerFound()
        {
            throw new NotImplementedException();
        }

        void MovementBase.playerLost()
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

        public Quaternion rotateStyle()
        {
            float moveSpeed = 1.5f;
            Vector3 targetDir = moveTarget.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
            return Quaternion.LookRotation(newDir);
        }

        public bool getPlayerTarget()
        {
            return true;
        }
    }
}