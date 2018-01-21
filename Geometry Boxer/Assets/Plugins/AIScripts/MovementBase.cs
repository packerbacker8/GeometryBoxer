using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

using UnityEngine.AI;
namespace Enemy
{
    public interface MovementBase
    {
        
        Vector3 move();

        bool canMove();

        void setUp(float stopDist, float stopThresh, float jumpDis, 
            Transform move);

        void playerFound();

        void playerLost();

        Quaternion rotateStyle();
    }
}