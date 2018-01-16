using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStand : MonoBehaviour {

    public bool UseCylinderHeight = true;
    public GameObject weapon;

    public float riseAmount = 10f;
    public float speed = 10f;

    private bool rising = false;
    private Vector3 targetLocation;

	// Use this for initialization
	void Start () {
        if (UseCylinderHeight)
        {
            riseAmount = transform.lossyScale.y;
        }
        targetLocation = new Vector3(transform.position.x, transform.position.y + riseAmount, transform.position.z);
        weapon.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if(rising)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);
            if (transform.position.y >= targetLocation.y)
            {
                rising = false;
                weapon.SetActive(true);
                weapon.transform.parent = null;
            }
        }
	}
    void Rise()
    {
        rising = true;
    }
}
