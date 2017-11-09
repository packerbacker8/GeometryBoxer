using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompressorArmMovement : MonoBehaviour {

    private bool activated = false;
    private bool reverse = false;
    private Vector3 restPosition;


    public GameObject ImpactPoint;
    public float speed = 10f;
	// Use this for initialization
	void Start () {
        restPosition = transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
        if(activated == true)
        {
            if(!reverse)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, ImpactPoint.transform.position, step);
            }
            else if(reverse)
            {
                float step = -(speed * Time.deltaTime)/2.0f;
                transform.position = Vector3.MoveTowards(restPosition, transform.position, step);
            }
        }
    }

    void Activate()
    {
        /*
        if(activated && !reverse)
        {
            reverse = true;
        }
        */
        activated = true;
    }
}
