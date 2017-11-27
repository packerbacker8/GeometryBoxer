using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

public class OctahedronSpecials : MonoBehaviour {


	CharacterMeleeDemo charMeleeDemoRef;
	UserControlThirdPerson userControlThirdPersonRef;
	Rigidbody rb;
	// animClips = anim.runtimeAnimatorController.animationClips;



	float currentTime = 0.0f;
	float SpecialTime = 0.0f;

	bool executingSpecial = false;
	bool specialIsSpeed = false;
	Animator anim;

	// Use this for initialization
	void Start () {
		charMeleeDemoRef = transform.GetComponentInChildren<CharacterMeleeDemo> ();
		userControlThirdPersonRef = transform.GetComponentInChildren<UserControlThirdPerson> ();
		anim = transform.GetComponentInChildren<Animator> ();
		//AnimatorStateInfo i = anim.GetNextAnimatorStateInfo(0);
		Rigidbody[] arr = transform.GetComponentsInChildren<Rigidbody>();
		rb = arr [11];



	}
	
	// Update is called once per frame
	void Update () {

		if (executingSpecial) {
			currentTime += Time.deltaTime;
			if (currentTime >= SpecialTime) {
				currentTime = 0.0f;
				executingSpecial = false;
				if (specialIsSpeed) {
					anim.SetFloat ("variableAnimSpeed", 1.0f);
				}
			}
		}
			
		if (Input.GetKey(KeyCode.V) && !executingSpecial) {
			anim.SetFloat ("variableAnimSpeed", 2.0f);
			executingSpecial = true;
			SpecialTime = 3.0f;
			specialIsSpeed = true;
		}
		if (Input.GetKey(KeyCode.B) && !executingSpecial) {
			anim.SetFloat ("variableAnimSpeed", 6.0f);
			executingSpecial = true;
			SpecialTime = 0.2f;
			specialIsSpeed = true;
		}
		if (Input.GetKey(KeyCode.Space) && !executingSpecial) {
			//rb.AddForce (-transform.forward * 50000);
			rb.velocity = new Vector3(0.0f, 0.0f, 100f);
			//executingSpecial = true;
		}






	}
}
