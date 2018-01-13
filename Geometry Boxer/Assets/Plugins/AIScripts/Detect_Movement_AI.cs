﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{


    public class Detect_Movement_AI : MonoBehaviour, MovementBase
    {
        float stoppingDistance;
        float stoppingThreshold;
        float jumpDistance;
        Transform moveTarget;
        public bool playerTarget;

        public bool canMove()
        {
                return (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance);

        }

        public Vector3 move()
        {
            if (canMove() && playerTarget)
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

        public void playerFound()
        {
            Debug.Log("Player Found");
            playerTarget = true;
            Debug.Log("PlayerTarget: " + playerTarget);
        }

        public void playerLost()
        {
            //throw new NotImplementedException();
        }
        // Use this for initialization
        void Start()
        {
            playerTarget = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
