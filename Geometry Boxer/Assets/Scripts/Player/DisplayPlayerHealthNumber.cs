using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerHealthNumber : MonoBehaviour {

    private float health;
    private TextMesh textBox;

    // Use this for initialization
    void Start () {
        health = GetComponentInParent<PlayerHealthScript>().PlayerHealth;
        textBox = this.GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        health = GetComponentInParent<PlayerHealthScript>().PlayerHealth;

        //Change made by - Henry
        //I divided the health (Which was 10,000) by 100, so it's out of 100. I also casted it to an int so it's a whole number. 
        //Please try to find a more proper way of doing this in the future 
        this.GetComponent<TextMesh>().text = "Health: " + ((int)(health/100)).ToString();
    }
}
