using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

using UnityEngine.AI;
namespace Enemy
{
    public interface MovementBase
    {
        void move();

        bool canMove();

        void setUp(float stopDist, float stopThresh, float jumpDis, Animator animator,
            Transform move, UserControlThirdPerson.State baseState, NavMeshAgent baseAgent);
    }
}