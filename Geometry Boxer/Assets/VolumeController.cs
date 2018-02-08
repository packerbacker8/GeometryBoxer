using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour {

    public Slider VolumeSlider;
    private AudioSource[] audios;
	// Use this for initialization
	void Start () {
        audios = this.gameObject.GetComponents<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach(AudioSource a in audios)
        {
            a.volume = VolumeSlider.value;
        }
	}
}
