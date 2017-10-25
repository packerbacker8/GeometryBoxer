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
        this.GetComponent<TextMesh>().text = health.ToString();
    }
}
