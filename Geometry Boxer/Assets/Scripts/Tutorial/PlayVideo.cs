using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayVideo : MonoBehaviour {

    public VideoPlayer video;
    public string sceneToLoad;
    private float timer = 0f;
    
    // Update is called once per frame
    void Update ()
    {
        timer += Time.deltaTime;
        if((!video.isPlaying && timer > 5f) || (timer > 1f && Input.anyKeyDown))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
	}
}
