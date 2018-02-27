using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctahedronActivateSfx : MonoBehaviour {

    public AudioClip activate;
    public AudioClip deactivate;
    public AudioClip spin;
    private AudioSource source;
    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    void OctaActivateSfx()
    {
        source.PlayOneShot(activate, 1f);
    }
    void OctaDeactivateSfx()
    {
        if(source.isPlaying)
        {
            source.Stop();
        }
        source.PlayOneShot(deactivate, 1f);
    }
    void OctaSpinSfx()
    {
        if(!source.isPlaying)
        {
            source.loop = true;
            source.clip = spin;
            source.volume = 0.5f;
            source.Play();
        }
    }
    void OctaSpinStopSfx()
    {
        source.loop = false;
        source.Stop();
    }

}
