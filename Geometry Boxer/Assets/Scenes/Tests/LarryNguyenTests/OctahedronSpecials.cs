using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

public class OctahedronSpecials : MonoBehaviour {


	CharacterMeleeDemo charMeleeDemoRef;
	UserControlThirdPerson userControlThirdPersonRef;
    CharacterThirdPerson charThirdPersonref;
	Rigidbody rb;

    //public CharacterController;
	// animClips = anim.runtimeAnimatorController.animationClips;



	float currentTime = 0.0f;
	float SpecialTime = 0.0f;

	bool executingSpecial = false;
	bool specialIsSpeed = false;
    bool tornadoMode = false;
	Animator anim;

	// Use this for initialization
	void Start () {
		charMeleeDemoRef = transform.GetComponentInChildren<CharacterMeleeDemo> ();
		userControlThirdPersonRef = transform.GetComponentInChildren<UserControlThirdPerson> ();
        charThirdPersonref = transform.GetComponentInChildren<CharacterThirdPerson>();
        anim = transform.GetComponentInChildren<Animator> ();
		//AnimatorStateInfo i = anim.GetNextAnimatorStateInfo(0);
		Rigidbody[] arr = transform.GetComponentsInChildren<Rigidbody>();
		rb = arr [11];



	}
	
	// Update is called once per frame
	void Update () {

        //anim.SetFloat("Forward", 1.0f);
        Debug.Log(userControlThirdPersonRef.state.move + " forward: " + charMeleeDemoRef.transform.forward);

        if (tornadoMode)
        {
            Vector3 vec = userControlThirdPersonRef.state.move;
            if (vec.x == 0)
            {
                vec.x = charMeleeDemoRef.transform.forward.x;
            }
            if (vec.y == 0)
            {
                vec.y = charMeleeDemoRef.transform.forward.y;
            }
            if (vec.z == 0)
            {
                vec.z = charMeleeDemoRef.transform.forward.z;
            }
            userControlThirdPersonRef.state.move = vec;
        }
       

        //vec.x = userControlThirdPersonRef.state.move.x;
        // vec.y = userControlThirdPersonRef.state.move.y;
        //userControlThirdPersonRef.state.move = vec;
        //charMeleeDemoRef.transform.Translate(transform.forward * 0.01f);

        //rb = charMeleeDemoRef.GetComponent<Rigidbody>();
        //rb.velocity = transform.forward * 5f;

        if (executingSpecial) {
			currentTime += Time.deltaTime;
			if (currentTime >= SpecialTime) {
				currentTime = 0.0f;
				executingSpecial = false;
                tornadoMode = false;
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
		//if (Input.GetKey(KeyCode.Space) && !executingSpecial) {
		//	//rb.AddForce (-transform.forward * 50000);
		//	rb.velocity = new Vector3(0.0f, 0.0f, 100f);
		//	//executingSpecial = true;
		//}

        if (Input.GetKey(KeyCode.G) && !executingSpecial)
        {
            //anim.SetFloat("Forward", 1.0f);
            //userControlThirdPersonRef.state.move = transform.forward;
            tornadoMode = true;

            executingSpecial = true;
            SpecialTime = 3.0f;
            specialIsSpeed = true;
        }


        //Debug.Log(anim.GetFloat("Forward"));

	}
}
