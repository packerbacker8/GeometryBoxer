using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneDrop : MonoBehaviour {

    private Rigidbody rig;
    public float impactSoundThreshold = 100f;
    //Sound Engine Needs
    private AudioSource source;
    private int meleeIndex;
    private System.Random rand = new System.Random();
    private SFX_Manager sfxManager;

    // Use this for initialization
    void Start () {
        rig = this.gameObject.GetComponent<Rigidbody>();
        source = gameObject.AddComponent<AudioSource>();
        sfxManager = FindObjectOfType<SFX_Manager>();
        meleeIndex = rand.Next(0, sfxManager.meleeMetal.Count);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Activate()
    {
        transform.parent = null;
        rig.useGravity = true;
        if (!source.isPlaying)
        {
            source.PlayOneShot(sfxManager.meleeMetal[meleeIndex], 1f);
            meleeIndex = rand.Next(0, sfxManager.meleeMetal.Count);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > impactSoundThreshold)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(sfxManager.meleeMetal[meleeIndex], 1f);
                meleeIndex = rand.Next(0, sfxManager.meleeMetal.Count);
            }
        }
    }
}
