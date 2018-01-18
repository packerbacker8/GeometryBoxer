using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNet2 : MonoBehaviour {
    public Vector3 respawnPt;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("SafetyNet");
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.GetChild(2).gameObject.GetComponent<Transform>().position = respawnPt;
        }
    }
}
