using System;
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
          
            playerTarget = true;
           
        }

        public void playerLost()
        {
           // Debug.Log("Player Lost");
            playerTarget = false;
        }

        public Quaternion rotateStyle()
        {
            //float moveSpeed = walkByDefault ? 1.0f : 1.5f;
            if (playerTarget)
            {
                float moveSpeed = 1.5f;
                Vector3 targetDir = moveTarget.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
                return Quaternion.LookRotation(newDir);
            }
            else
            {
                
                //float angle = Mathf.Sin(Time.time) * 70;
                Vector3 result = new Vector3();
                //return Quaternion.AngleAxis(angle, Vector3.up);
                return Quaternion.Euler(0, -Mathf.PingPong(Time.time * 50, 100), 0);
               
            }
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

        public bool getPlayerTarget()
        {
            return playerTarget;
        }
    }
}
