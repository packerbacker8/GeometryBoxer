using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagSFX : MonoBehaviour {

    private SFX_Manager sfxManager;
    private AudioSource source;
    private System.Random rand = new System.Random();

    // Use this for initialization
    void Start ()
    {
        sfxManager = FindObjectOfType<SFX_Manager>();
        source = this.gameObject.AddComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision col)
    {
        if(col.transform.root.tag == "Player" && !source.isPlaying && col.impulse.magnitude > 5f)
        {
            Debug.Log(col.impulse.magnitude);
            source.PlayOneShot(sfxManager.lightPunches[rand.Next(0, sfxManager.lightPunches.Count)], col.impulse.magnitude/100f);
        }
    }
}
