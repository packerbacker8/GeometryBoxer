using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

namespace RootMotion.Demos
{
    public class EnemyBehavior
    {
        private Vector3 _targetDir;
        private Vector3 _newDir;
        private AnimatorStateInfo _info;

        public EnemyBehavior(Vector3 targetDir, Vector3 newDir, AnimatorStateInfo info)
        {
            _targetDir = targetDir;
            _newDir = newDir;
            _info = info;
        }
        public void defaultMovement()
        {

        }
    }
}