using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextIntro : MonoBehaviour {

    public int levelIndex;
	public float timeElapsed = 0f;
	private bool text1FadeInComplete = false;
	private bool text2FadeInComplete = false;
	private bool fadeOutComplete = false;
	public Text IntroText1;
	public Text IntroText2;

	// Use this for initialization
	void Start () 
	 {
		IntroText1 = GameObject.Find ("Text1").GetComponent<Text>();
		IntroText2 = GameObject.Find ("Text2").GetComponent<Text>();

		IntroText1.color = Color.black;
		IntroText2.color = Color.black;

	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;

		if (timeElapsed > 2f && !text1FadeInComplete) {
			Color curColor1 = IntroText1.color;
			curColor1 += new Color (1f, 1f, 1f, 1f) * Time.deltaTime;
			IntroText1.color = curColor1;
			if (curColor1 == Color.white) 
			{
				text1FadeInComplete = true;
			}
		}
		if (timeElapsed > 8f && !text2FadeInComplete) 
		{
			Color curColor2 = IntroText2.color;
			curColor2 += new Color (1f, 1f, 1f, 1f) * Time.deltaTime;
			IntroText2.color = curColor2;
			if (curColor2 == Color.white) 
			{
				text2FadeInComplete = true;
			}
		}
		if (timeElapsed > 12f) 
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene (levelIndex);
		}
	}
}
