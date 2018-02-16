using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using UnityEngine.AI;

namespace Enemy
{
    public class NormalMovementAI : MonoBehaviour, IMovementBase
    {
        public float leeway = 0.1f;

        private float stoppingDistance;
        private float stoppingThreshold;
        private float jumpDistance;
        private float moveSpeed;

        private bool tutorialScene;

        private GameObject moveTargetObj;
        private GameObject gameController;
        Transform moveTarget;
        UserControlThirdPerson.State state;

        private void Start()
        {
            gameController = GameObject.FindGameObjectWithTag("GameController");
            moveSpeed = 1.5f;
            tutorialScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Tutorial";
        }

        public bool CanMove()
        {
            if (moveTargetObj == null)
            {
                return false;
            }
            return (Vector3.Distance(moveTarget.position, transform.position) + leeway > stoppingThreshold * stoppingDistance);
        }

        public Vector3 Move()
        {
            if (CanMove())
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
            //anim = animator;
            moveTargetObj = moveObj;
            moveTarget = moveTargetObj.transform;

        }

        void IMovementBase.PlayerFound()
        {
            throw new NotImplementedException();
        }

        void IMovementBase.PlayerLost()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When the current move to target has been detected as null, update the movetargetobj to something new.
        /// </summary>
        public void UpdateTarget()
        {
            //check only needed for regular bots as tutorial scene only uses regular bots
            if(tutorialScene)
            {
                gameController.GetComponent<GameControllerScriptTutorial>().SetNewTarget(this.transform.parent.GetComponent<EnemyHealthScript>().GetEnemyIndex(), this.transform.root.tag);
            }
            else
            {
                gameController.GetComponent<GameControllerScript>().SetNewTarget(this.transform.parent.GetComponent<EnemyHealthScript>().GetEnemyIndex(), this.transform.root.tag);
            }
        }

        public Quaternion RotateStyle()
        {
            if(moveTargetObj == null)
            {
                return transform.rotation;
            }
            Vector3 targetDir = moveTarget.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * moveSpeed, 0.0f);
            return Quaternion.LookRotation(newDir);
        }

        public bool GetPlayerTarget()
        {
            return true;
        }
    }
}