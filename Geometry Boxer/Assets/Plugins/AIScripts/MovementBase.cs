
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

using UnityEngine.AI;
namespace Enemy
{
    public interface IMovementBase
    {
        Vector3 Move();

        bool CanMove();

        void SetUp(float stopDist, float stopThresh, float jumpDis,
            GameObject move);

        void PlayerFound();

        void PlayerLost();

        Quaternion RotateStyle();

        bool GetPlayerTarget();

        void UpdateTarget();
    }
}