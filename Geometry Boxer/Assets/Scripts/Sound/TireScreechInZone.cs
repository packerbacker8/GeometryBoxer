﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireScreechInZone : MonoBehaviour {

    public AudioClip clip;

    private AudioSource source;

	// Use this for initialization
	void Start () {
        source = this.GetComponent<AudioSource>();
        source.clip = clip;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.transform.name.Contains("JEEP"))
        {
            source.Play();
        }
    }
}
