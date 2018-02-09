using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSound : MonoBehaviour {

    [Header("Impact sounds that are used for when the enemy comes into contact with the player.")]
    public List<AudioClip> clips = new List<AudioClip>();
    public int index;
    public float punchSoundForceThreshold;

    private AudioSource source;
    private System.Random rand = new System.Random();

	// Use this for initialization
	void Start ()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1.0f;
        source.clip = clips[0];
        source.volume = 0.6f;
	}
	
	public void SendImpactSound(Collision col)
    {
        if(Mathf.Abs(col.impulse.magnitude) > punchSoundForceThreshold && col.transform.root.tag == "Player")
        {
            source.PlayOneShot(clips[index], 1f);
            index = rand.Next(0, clips.Count);
        }
    }
}
