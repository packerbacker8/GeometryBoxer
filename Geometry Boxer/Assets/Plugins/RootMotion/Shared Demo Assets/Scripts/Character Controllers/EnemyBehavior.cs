using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

namespace RootMotion.Demos
{
    [System.Serializable]
    public abstract class EnemyBehavior
    {
        public abstract void Movement();

        public abstract void Attack();
    }
}