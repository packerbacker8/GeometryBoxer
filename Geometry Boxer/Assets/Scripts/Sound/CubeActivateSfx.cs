using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeActivateSfx : MonoBehaviour {

    public AudioClip activate;
    public AudioClip deactivate;
    public AudioClip stomp;
    public AudioClip jump;
    private AudioSource source;
    // Use this for initialization
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    void CubeActivatedSfx()
    {
        source.PlayOneShot(activate, 1f);
    }
    void CubeDeactivatedSfx()
    {
        source.PlayOneShot(deactivate, 1f);
    }
    void CubeJumpSfx()
    {
        source.PlayOneShot(jump, 1f);
    }
    void CubeStompSfx()
    {
        source.PlayOneShot(stomp, 0.5f);
    }
}
