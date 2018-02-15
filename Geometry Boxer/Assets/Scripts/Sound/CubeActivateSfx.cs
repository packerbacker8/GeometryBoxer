using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeActivateSfx : MonoBehaviour {

    public AudioClip activate;
    public AudioClip deactivate;
    private AudioSource source;
    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.8f;
    }

    void CubeActivatedSfx()
    {
        source.PlayOneShot(activate, 1f);
    }
    void CubeDeactivatedSfx()
    {
        source.PlayOneShot(deactivate, 1f);
    }
}
