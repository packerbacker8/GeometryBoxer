using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Patrol_Movement_AI : MonoBehaviour, MovementBase
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
        public List<GameObject> patrolPositions;
        private GameObject playerObj;
        private Transform playerTransform;
        int currentSpot = 0;

        float patrolStopDistance = 1f;
        public bool canMove()
        {
            if (playerTarget)
            {
                return (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance);
            }
            return Vector3.Distance(moveTarget.position, transform.position) > patrolStopDistance;
        }

        public Vector3 move()
        {
            if (canMove())
            {
                return moveTarget.position;
            }
            else if (!playerTarget)
            {
                currentSpot++;
                if(currentSpot >= patrolPositions.Count)
                {
                    currentSpot = 0;
                }
                moveTarget = patrolPositions[currentSpot].transform;
            }
            return transform.position;
        }

        void MovementBase.setUp(float stopDist, float stopThresh, float jumpDis, GameObject moveObj)
        {
            stoppingDistance = stopDist;
            stoppingThreshold = stopThresh;
            jumpDistance = jumpDis;
            //anim = animator;
            moveTarget = patrolPositions[0].transform;
            playerObj = moveObj;
            playerTransform = playerObj.transform;
        }

        public void playerFound()
        {
            playerTarget = true;
            moveTarget = playerTransform;

        }

        public void playerLost()
        {
            if (playerTarget)
            {
               moveTarget = patrolPositions[currentSpot].transform;   
            }
            playerTarget = false;
        }

        public Quaternion rotateStyle()
        {
            //float moveSpeed = walkByDefault ? 1.0f : 1.5f;
            //if (playerTarget)
            //{
                //float moveSpeed = 1.5f;
                Vector3 targetDir = moveTarget.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * rotateSpeed, 0.0f);
                return Quaternion.LookRotation(newDir);
            /*}
            else
            {

                float angle = (Mathf.Sin(Time.time) * bounceAngle) + startAngle;
                return Quaternion.AngleAxis(angle, Vector3.up);

            }*/
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
            //return playerTarget;
            return true;
        }
    }

}