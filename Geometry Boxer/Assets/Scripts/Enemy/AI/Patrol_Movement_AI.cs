using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Patrol_Movement_AI : MonoBehaviour, IMovementBase
    {
        public bool playerTarget;
        public float bounceAngle;
        public float rotateSpeed;
        public float leeway = 0.1f;
        public List<GameObject> patrolPositions;

        private float stoppingDistance;
        private float stoppingThreshold;
        private float jumpDistance;
        private float startAngle;
        private float patrolStopDistance = 1f;
        private GameObject playerObj;
        private GameObject gameController;
        private Transform playerTransform;
        private Transform moveTarget;
        private int currentSpot = 0;
        private bool inZone;


        public bool CanMove()
        {

            if (playerTarget)
            {
                return (Vector3.Distance(moveTarget.position, transform.position) + leeway > stoppingThreshold * stoppingDistance);
            }
            return Vector3.Distance(moveTarget.position, transform.position) > patrolStopDistance;
        }

        public Vector3 Move()
        {
            if (CanMove())
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

        void IMovementBase.SetUp(float stopDist, float stopThresh, float jumpDis, GameObject moveObj)
        {
            stoppingDistance = stopDist;
            stoppingThreshold = stopThresh;
            jumpDistance = jumpDis;
            //anim = animator;
            moveTarget = patrolPositions[0].transform;
            playerObj = moveObj;
            playerTransform = playerObj.transform;
        }

        public void PlayerFound()
        {
            playerTarget = true;
            moveTarget = playerTransform;

        }

        public void PlayerLost()
        {
            if (playerTarget)
            {
               moveTarget = patrolPositions[currentSpot].transform;   
            }
            playerTarget = false;
        }

        public Quaternion RotateStyle()
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
            gameController = GameObject.FindGameObjectWithTag("GameController");
        }

        public bool GetPlayerTarget()
        {
            //return playerTarget;
            return true;
        }

        /// <summary>
        /// When the current move to target has been detected as null, update the movetargetobj to something new.
        /// </summary>
        public void UpdateTarget()
        {
            gameController.GetComponent<GameControllerScript>().SetNewTarget(this.transform.parent.GetComponent<EnemyHealthScript>().GetEnemyIndex(), this.transform.root.tag);
        }
    }

}