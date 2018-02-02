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
        public float bounceAngle;
        private bool inZone;
        private float startAngle;
        public float rotateSpeed;
        public float distance;
        public bool canMove()
        {
            return (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance);

        }

        public Vector3 move()
        {

            if (canMove() && playerTarget)//playerTarget)
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
            StartCoroutine(checkDistance());
        }

        public void playerFound()
        {
            //playerTarget = true;

        }

        public void playerLost()
        {
            //if (playerTarget)
            //{
            //    startAngle = transform.eulerAngles.y;
            //}
            //playerTarget = false;
        }

        public Quaternion rotateStyle()
        {
            //float moveSpeed = walkByDefault ? 1.0f : 1.5f;
            if (playerTarget)
            {
                //float moveSpeed = 1.5f;
                Vector3 targetDir = moveTarget.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * rotateSpeed, 0.0f);
                return Quaternion.LookRotation(newDir);
            }
            else
            {

                float angle = (Mathf.Sin(Time.time) * bounceAngle) + startAngle;
                return Quaternion.AngleAxis(angle, Vector3.up);

            }
        }
        // Use this for initialization
        void Start()
        {
            playerTarget = false;
            bounceAngle = 70f;
            startAngle = transform.eulerAngles.y;
            rotateSpeed = 1.5f;
           
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public bool getPlayerTarget()
        {
            return playerTarget;
        }

        private IEnumerator checkDistance()
        {
            while (true)
            {
                distance = Vector3.Distance(moveTarget.position, transform.position);
                if (distance < 20f)
                {
                    Debug.Log("Found Player");
                    playerTarget = true;
                }
                else
                {
                    playerTarget = false;
                }
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}