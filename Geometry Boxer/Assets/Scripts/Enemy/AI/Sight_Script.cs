using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered started: " + other.gameObject.tag);
        if (other.transform.root.tag == "Player")
        {
            Debug.Log("Player Detected");
            this.SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
        }
    }
}
