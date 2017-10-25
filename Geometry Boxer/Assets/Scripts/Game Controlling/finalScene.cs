using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finalScene : MonoBehaviour {
    public float timeElapsed = 0f;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 12f)
        {
            StartCoroutine(ChangeLevel());
        }
    }

    IEnumerator ChangeLevel()
    {
        float fadeTime = GetComponent<Fade>().BeginFade(1);
        Debug.Log("FadeTime: " + fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Application.Quit();
    }
}
