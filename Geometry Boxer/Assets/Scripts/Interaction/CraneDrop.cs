using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneDrop : MonoBehaviour {

    private Rigidbody rig;

	// Use this for initialization
	void Start () {
        rig = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Activate()
    {
        rig.useGravity = true;
    }
}
