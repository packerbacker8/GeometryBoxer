using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSfx : MonoBehaviour {

    public float damageThreshold = 100f;
    //Sound Engine Needs
    private AudioSource source;
    private int meleeIndex;
    private System.Random rand = new System.Random();
    private SFX_Manager sfxManager;

    // Use this for initialization
    void Start ()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.8f;
        sfxManager = FindObjectOfType<SFX_Manager>();
        meleeIndex = rand.Next(0, sfxManager.meleeMetal.Count);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.root.tag == "EnemyRoot")
        {
            if (collision.impulse.magnitude > damageThreshold)
            {
                if(!source.isPlaying)
                {
                    source.PlayOneShot(sfxManager.meleeMetal[meleeIndex], 1f);
                    meleeIndex = rand.Next(0, sfxManager.meleeMetal.Count);
                }
            }
        }

    }
}
