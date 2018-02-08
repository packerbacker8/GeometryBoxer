using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

namespace RootMotion.Demos
{
    public class PropPickUpTriggerWithParticle : MonoBehaviour
    {

        public Prop prop;
        public LayerMask characterLayers;
        public ParticleSystem particles;

        private CharacterPuppet characterPuppet;
        private GameObject light;

        void Start()
        {
            light = this.transform.parent.GetChild(5).gameObject;
        }

        void OnTriggerEnter(Collider collider)
        {
            if(collider.transform.root.tag == "Player" || collider.tag.Contains("Enemy"))
            {
                particles.gameObject.transform.parent = null;
                particles.transform.localScale = Vector3.one;

                //particles.Play();
            }
            if (prop.isPickedUp) return;
            if (!LayerMaskExtensions.Contains(characterLayers, collider.gameObject.layer)) return;

            characterPuppet = collider.GetComponent<CharacterPuppet>();
            if (characterPuppet == null) return;

            if (characterPuppet.puppet.state != BehaviourPuppet.State.Puppet) return;

            if (characterPuppet.propRoot == null) return;
            if (characterPuppet.propRoot.currentProp != null) return;

            characterPuppet.propRoot.currentProp = prop;
            light.SetActive(false);
        }
    }
}

