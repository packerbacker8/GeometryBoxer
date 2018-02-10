using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSoundOnAwake : MonoBehaviour {

    [Range (0f,1f)]
    public float volume = 1.0f;

    private AudioSource source;
    private SFX_Manager sfxManager;
    private System.Random rand = new System.Random();
    private int deathIndex;
    
    void Awake()
    {
        sfxManager = FindObjectOfType<SFX_Manager>();
        deathIndex = rand.Next(0, sfxManager.shatterDeath.Count);
        source = this.gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1.0f;
        source.PlayOneShot(sfxManager.shatterDeath[deathIndex], volume);
    }

}
