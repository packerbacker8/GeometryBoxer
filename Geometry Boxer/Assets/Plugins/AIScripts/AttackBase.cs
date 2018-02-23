using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using UnityEngine.AI;

namespace Enemy
{
    public interface IAttackBase
    {
        bool CanAttack();

        void Attack();
        void SetUp(float stopDist, float stopThresh, float jumpDis, 
        GameObject moveObj, CharacterPuppet charPup, AudioSource src,
        SFX_Manager sfx, float rangeAttack);

        bool IsAttacking();

    }
}