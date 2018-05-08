using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class MusicOptionPauseMenu : MonoBehaviour {

    public AudioMixer audioMixerMaster;

    public void SetMasterVolume(float volume)
    {
        audioMixerMaster.SetFloat("MasterVolume", volume);
    }
}
