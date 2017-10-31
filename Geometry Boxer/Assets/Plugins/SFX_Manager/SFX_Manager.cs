using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Manager : MonoBehaviour {

    public AudioSource player;

    public List<AudioClip> playerPain = new List<AudioClip>();

    public List<AudioClip> maleAttack = new List<AudioClip>();
    public List<AudioClip> femaleAttack = new List<AudioClip>();

    public List<AudioClip> malePain = new List<AudioClip>();
    public List<AudioClip> femalePain = new List<AudioClip>();

    public List<AudioClip> maleDeath = new List<AudioClip>();
    public List<AudioClip> femaleDeath = new List<AudioClip>();

    public List<AudioClip> heavyPunches = new List<AudioClip>();
    public List<AudioClip> lightPunches = new List<AudioClip>();
    public List<AudioClip> swishes = new List<AudioClip>();

    public List<AudioClip> meleeMetal = new List<AudioClip>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
