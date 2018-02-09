﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{


    public class Detect_Movement_AI : MonoBehaviour, MovementBase
    {
        public float rotateSpeed = 1.5f;
        public float bounceAngle = 70f;
        public float sightRange = 20f;
        public bool playerTarget;

        private float distance;
        private float stoppingDistance;
        private float stoppingThreshold;
        private float jumpDistance;
        private float startAngle;
        private GameObject moveTargetObj;
        private Transform moveTarget;
        private Transform playerTransform;
        private GameObject gameController;
        private bool inZone;

        /// <summary>
        /// Function to allow game controller to set the transform of the active player at the start so the 
        /// AI can detect if it is close enough to the player yet or not.
        /// </summary>
        /// <param name="player">The transform of the player that the AI is continually 'looking' for.</param>
        public void SetPlayerTransform(Transform player)
        {
            playerTransform = player;
        }

        public bool canMove()
        {
            if(moveTargetObj == null)
            {
                return false;
            }
            return (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance);

        }

        public Vector3 move()
        {
            if (canMove()) //&& playerTarget)
            {

                return moveTarget.position;
            }

            return transform.position;
        }

        void MovementBase.setUp(float stopDist, float stopThresh, float jumpDis, GameObject moveObj)
        {
            stoppingDistance = stopDist;
            stoppingThreshold = stopThresh;
            jumpDistance = jumpDis;
            moveTargetObj = moveObj;
            moveTarget = moveTargetObj.transform;
            //StartCoroutine(checkDistance());
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
            if (moveTargetObj != null)  //was playerTarget
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
            startAngle = transform.eulerAngles.y;
            gameController = GameObject.FindGameObjectWithTag("GameController");
        }

        public bool getPlayerTarget()
        {
            return playerTarget;
        }

        private void Update()
        {
            if (moveTargetObj == null)
            {
                UpdateTarget();
            }
            distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance < sightRange)
            {
                playerTarget = true;
                this.transform.parent.gameObject.GetComponent<EnemyHealthScript>().ChangeOurTarget();
            }
            else
            {
                playerTarget = false;
                if (moveTarget == playerTransform) //allow enemy bots to retarget other bots
                {
                    UpdateTarget();
                }
            }
        }

        /// <summary>
        /// When the current move to target has been detected as null, update the movetargetobj to something new.
        /// </summary>
        public void UpdateTarget()
        {
            gameController.GetComponent<GameControllerScript>().SetNewTarget(this.transform.parent.GetComponent<EnemyHealthScript>().GetEnemyIndex(), this.transform.root.tag);
        }

        private IEnumerator checkDistance()
        {
            while (true)
            {
                distance = Vector3.Distance(moveTarget.position, transform.position);
                if (distance < sightRange)
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