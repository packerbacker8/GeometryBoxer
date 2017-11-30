using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallActivateSfx : MonoBehaviour {

    public AudioClip activate;
    public AudioClip deactivate;
    private AudioSource source;
    // Use this for initialization
    void Start () {
		source = gameObject.AddComponent<AudioSource>();
    }
	
	void BallActivatedSfx()
    {
        source.PlayOneShot(activate, 1f);
    }
    void BallDeactivatedSfx()
    {
        source.PlayOneShot(deactivate, 1f);
    }
}
