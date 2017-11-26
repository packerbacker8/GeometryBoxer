﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger_MainMenu_Melee : MonoBehaviour {

    public GameObject mainTrigger;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.tag == "Player")
        {
            mainTrigger.SendMessage("MoveToMeleeDemo");
        }
    }
    private void OnTriggerExit(Collider col)
    {
        //mainTrigger.SendMessage("MoveToOverhead");
    }
}
