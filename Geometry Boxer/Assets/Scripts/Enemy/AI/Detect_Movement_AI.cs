﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{


    public class Detect_Movement_AI : MonoBehaviour, IMovementBase
    {
        public float rotateSpeed = 1.5f;
        public float bounceAngle = 70f;
        public float sightRange = 20f;
        public float leeway = 0.1f;
        public bool playerTarget;
        
        private float distance;
        private float stoppingDistance;
        private float stoppingThreshold;
        private float jumpDistance;
        private float startAngle;
        private GameObject moveTargetObj;
        private Transform moveTarget;
        private Transform playerTransform;
        private Transform player2Transform;
        private GameObject gameController;
        private bool inZone;
        private bool[] playersTargetable = new bool[2];

        /// <summary>
        /// Function to allow game controller to set the transform of the active player at the start so the 
        /// AI can detect if it is close enough to the player yet or not.
        /// </summary>
        /// <param name="player">The transform of the player that the AI is continually 'looking' for.</param>
        public void SetPlayersTransform(Transform player, Transform player2)
        {
            playerTransform = player;
            player2Transform = player2;
        }

        public bool CanMove()
        {
            if(moveTargetObj == null)
            {
                return false;
            }
            return (Vector3.Distance(moveTarget.position, transform.position) + leeway > stoppingThreshold * stoppingDistance);

        }

        public Vector3 Move()
        {
            if (CanMove()) //&& playerTarget)
            {

                return moveTarget.position;
            }

            return transform.position;
        }

        void IMovementBase.SetUp(float stopDist, float stopThresh, float jumpDis, GameObject moveObj)
        {
            stoppingDistance = stopDist;
            stoppingThreshold = stopThresh;
            jumpDistance = jumpDis;
            moveTargetObj = moveObj;
            moveTarget = moveTargetObj.transform;
            //StartCoroutine(checkDistance());
        }

        public void PlayerFound()
        {
            //playerTarget = true;

        }

        public void PlayerLost()
        {
            //if (playerTarget)
            //{
            //    startAngle = transform.eulerAngles.y;
            //}
            //playerTarget = false;
        }

        public Quaternion RotateStyle()
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
            //StartCoroutine(checkDistance());
        }

        public bool GetPlayerTarget()
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
            if (playersTargetable[0] && distance < sightRange && !moveTarget.Equals(player2Transform))
            {
                playerTarget = true;
                this.transform.parent.gameObject.GetComponent<EnemyHealthScript>().ChangeOurTarget(false);
            }
            else if(playersTargetable[1] && player2Transform != null && (distance = Vector3.Distance(player2Transform.position, transform.position)) < sightRange)
            {
                playerTarget = true;
                this.transform.parent.gameObject.GetComponent<EnemyHealthScript>().ChangeOurTarget(true);
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
                /*
                distance = Vector3.Distance(moveTarget.position, transform.position);
                if (distance < sightRange)
                {
                    Debug.Log("Found Player");
                    playerTarget = true;
                }
                else
                {
                    playerTarget = false;
                }*/
                Debug.Log("Y velocity for detect bots: " + this.GetComponent<Rigidbody>().velocity.y);
                yield return new WaitForSeconds(10f);
            }
        }

        /// <summary>
        /// Function to increase sight radius of detect AI. Multiplies sight radius by amount.
        /// </summary>
        /// <param name="amount">A less than 1.0 amount will decrease sight radius, greater will increase, and 1.0 will keep the same.</param>
        public void IncreaseSight(float amount)
        {
            sightRange *= amount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="isTargetable"></param>
        public void SetIfPlayerIsTargetable(int player, bool isTargetable)
        {
            playersTargetable[player] = isTargetable;
        }
    }
}