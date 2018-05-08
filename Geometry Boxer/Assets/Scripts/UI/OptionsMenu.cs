using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public AudioMixer audioMixerMaster;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Slider musicSlider;
    private Resolution[] resolutions;

	// Use this for initialization
	void Start () {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        float currentMusicVolume;
        audioMixerMaster.GetFloat("MasterVolume",out currentMusicVolume);
        musicSlider.value = currentMusicVolume;

    }
	public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
	public void SetMasterVolume(float volume)
    {
        audioMixerMaster.SetFloat("MasterVolume", volume);
    }
    /*
    public void SetMusicVolume(float volume)
    {
        audioMixerMusic.SetFloat("MusicVolume", volume);
    }
    */
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
