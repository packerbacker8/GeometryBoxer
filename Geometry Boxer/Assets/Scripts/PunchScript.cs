using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            this.GetComponent<Rigidbody>().AddForce(this.transform.forward * 100f);

        }
    }
}
