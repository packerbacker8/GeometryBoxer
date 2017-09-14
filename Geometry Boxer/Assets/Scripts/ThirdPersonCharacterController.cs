using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour {

    public float speed = 10f;
    public float turnSpeed = 1f;
    public float jumpHeight = 10f;

    private bool isSprinting = false;

    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode jump = KeyCode.Space;
    public string forward = "w";
    public string backward = "s";
    public string left = "a";
    public string right = "d";

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(sprint) && !isSprinting)
        {
            speed = speed * 2f;
            isSprinting = true;
        }
        if(Input.GetKeyUp(sprint))
        {
            isSprinting = false;
            speed = speed / 2f;
        }
        if (Input.GetKey(forward)) 
        {
            this.transform.position += this.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(backward))
        {
            this.transform.position -= this.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(right))
        {
            this.transform.Rotate(Vector3.up * turnSpeed);
        }
        if (Input.GetKey(left))
        {
            this.transform.Rotate(-Vector3.up * turnSpeed);
        }
        if (Input.GetKey(jump))
        {
            this.transform.position += this.transform.up * jumpHeight * Time.deltaTime;
        }

    }
}
