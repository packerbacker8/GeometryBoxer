﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger_MainMenu_Dummy : MonoBehaviour {
    public GameObject mainTrigger;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        mainTrigger.SendMessage("MoveToDummyDemo");
    }
    private void OnTriggerExit(Collider other)
    {
        mainTrigger.SendMessage("MoveToOverhead");
    }
}
