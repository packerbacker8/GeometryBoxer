using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.Demos
{
    public class DropMeleeOnImpact : MonoBehaviour
    {
        private Animator anim;
        private CharacterPuppet characterPuppet;
        private string getUpProne = "GetUpProne";
        private string getUpSupine = "GetUpSupine";
        private int animationControllerIndex = 0;

        public float dropThreshold = 10f;

        void Start()
        {
            characterPuppet = this.transform.GetComponent<CharacterPuppet>();
            anim = this.gameObject.transform.GetChild(animationControllerIndex).gameObject.GetComponent<Animator>();
        }

        void OnCollisionEnter(Collision collision)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            Debug.Log("Collision detected, Force: " + collision.impulse.magnitude);
            if (collision.impulse.magnitude > dropThreshold || info.IsName(getUpProne) || info.IsName(getUpSupine))
            {
                Debug.Log("Dropping melee");
                characterPuppet.propRoot.currentProp = null;
            }
        }
    }
}

