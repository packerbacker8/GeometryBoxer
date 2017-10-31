using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Anim_Sounds : MonoBehaviour {

    //Sound Engine Needs
    private AudioSource source;
    private int swishIndex;
    private int painIndex;
    private int deathIndex;
    private System.Random rand = new System.Random();
    private SFX_Manager sfxManager;

    private string leftPunchAnimation = "Hit";
    private string rightPunchAnimation = "Hit";
    private string leftSwingAnimation = "SwingProp";
    private string rightSwingAnimation = "SwingProp";
    private string getUpProne = "GetUpProne";
    private string getUpSupine = "GetUpSupine";
    private string fall = "Fall";
    private string onGround = "OnGround";

    private bool isPunching = false;

    public Animator anim;
    AnimatorStateInfo info;

    // Use this for initialization
    void Start ()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialize = true;
        source.volume = 0.6f;
        sfxManager = FindObjectOfType<SFX_Manager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Hit") && sfxManager.swishes.Count > 0 && !source.isPlaying)
        {
            swishIndex = rand.Next(0, sfxManager.malePain.Count);
            source.PlayOneShot(sfxManager.swishes[swishIndex], 1f);
        }	
	}
}
