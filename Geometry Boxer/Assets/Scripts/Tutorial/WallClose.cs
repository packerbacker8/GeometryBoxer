using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClose : MonoBehaviour {
    public GameObject[] enemyStands;
    public GameObject[] weaponStands; 

    public float moveX = 1f;
    public float moveY = 1f;
    public float moveZ = 1f;
    public float speed = 10f;

    private bool closing = false;
    private bool closed = false;
    private Vector3 targetLocation;
	// Use this for initialization
	void Start () {
		targetLocation = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z + moveZ);
    }
	
	// Update is called once per frame
	void Update () {
		if(closing && !closed)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);
            if (transform.position == targetLocation)
            {
                closing = false;
                closed = true;
            }
        }
	}
    void CloseWall()
    {
        closing = true;
    }
}
