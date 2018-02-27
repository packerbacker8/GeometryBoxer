using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	public class CharacterPuppet : CharacterThirdPerson {

		[Header("Puppet")]
		
		public PropRoot propRoot;

		public BehaviourPuppet puppet { get; private set; }

        public bool canMove = true;

		protected override void Start() {
			base.Start();

			puppet = transform.parent.GetComponentInChildren<BehaviourPuppet>();
		}

		public override void Move(Vector3 deltaPosition, Quaternion deltaRotation) {
			// Disable movement while the puppet is not balanced or getting up.
			if (puppet == null || puppet.state != BehaviourPuppet.State.Puppet || !canMove) return;

			base.Move(deltaPosition, deltaRotation);
		}

		protected override void Rotate() {
			// Disable rotation while the puppet is not balanced or getting up.
			if (puppet.state != BehaviourPuppet.State.Puppet || !canMove) {
				if (gravityTarget != null) transform.rotation = Quaternion.FromToRotation(transform.up, transform.position - gravityTarget.position) * transform.rotation;

				return;
			}

			base.Rotate();
		}

		protected override bool Jump() {
			if (puppet.state != BehaviourPuppet.State.Puppet || !canMove) return false;

			return base.Jump();
		}
	}
}
