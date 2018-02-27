using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialImpactSfx : MonoBehaviour {

    public AudioClip impactSound;

    private AudioSource source;
	// Use this for initialization
	void Start () {
        source = this.gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.5f;
	}
    private void OnCollisionEnter(Collision collision)
    {
        source.PlayOneShot(impactSound, 1f);
    }
}
