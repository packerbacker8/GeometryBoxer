using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using UnityEngine.AI;

namespace Enemy
{
    public interface AttackBase
    {
        bool canAttack();

        void attack();
        void setUp(float stopDist, float stopThresh, float jumpDis, 
        Transform move, CharacterPuppet charPup, AudioSource src,
        SFX_Manager sfx, float rangeAttack);

    }
}