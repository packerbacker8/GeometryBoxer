using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour {

    private GameObject _player;

	// Use this for initialization
	void Start () {
        _player = GameObject.Find("SphereChar_Player/Character Controller");
        Vector3 test = _player.transform.position;
        Debug.Log((_player.transform.position - transform.position).ToString());
        this.GetComponent<Rigidbody>().AddForce((_player.transform.position - transform.position).normalized * 3000);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 test = _player.transform.position;
        Debug.Log((_player.transform.position - transform.position).ToString());
        this.GetComponent<Rigidbody>().AddForce((_player.transform.position - transform.position).normalized * 3000);
    }
}
