using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	public class PropPickUpTrigger : MonoBehaviour {

		public Prop prop;
		public LayerMask characterLayers;
        public AudioClip pickup;

        private CharacterPuppet characterPuppet;
        private GameObject light;
        private AudioSource source;

        void Start()
        {
            light = this.transform.parent.GetChild(5).gameObject;
            source = this.gameObject.AddComponent<AudioSource>();
        }

        void OnTriggerEnter(Collider collider) {
			if (prop.isPickedUp) return;
			if (!LayerMaskExtensions.Contains(characterLayers, collider.gameObject.layer)) return;

			characterPuppet = collider.GetComponent<CharacterPuppet>();
			if (characterPuppet == null) return;

			if (characterPuppet.puppet.state != BehaviourPuppet.State.Puppet) return;

			if (characterPuppet.propRoot == null) return;
			if (characterPuppet.propRoot.currentProp != null) return;

			characterPuppet.propRoot.currentProp = prop;
            light.SetActive(false);
            source.PlayOneShot(pickup, 0.5f);
        }
	}
}
