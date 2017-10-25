using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayEnemyHealthNumber : MonoBehaviour {

    private float health;
    private TextMesh textBox;

    // Use this for initialization
    void Start () {
        health = GetComponentInParent<EnemyHealthScript>().EnemyHealth;
        textBox = this.GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        health = GetComponentInParent<EnemyHealthScript>().EnemyHealth;
        this.GetComponent<TextMesh>().text = health.ToString();
    }
}
