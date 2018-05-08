using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class MusicOptionPauseMenu : MonoBehaviour {

    public AudioMixer audioMixerMaster;
    public Slider musicSlider;

    public void Awake()
    {
        float currentMusicVolume;
        audioMixerMaster.GetFloat("MasterVolume", out currentMusicVolume);
        musicSlider.value = currentMusicVolume;
        SetMasterVolume(currentMusicVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixerMaster.SetFloat("MasterVolume", volume);
    }
}
