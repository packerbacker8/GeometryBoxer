using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{

    public float speed = 10f;
    public float turnSpeed = 1f;
    public float jumpHeight = 10f;
    public bool useController = false;

    private bool isSprinting = false;

    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode jump = KeyCode.Space;
    public string forward = "w";
    public string backward = "s";
    public string left = "a";
    public string right = "d";


    private bool leftStickInUse = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(useController)
        {
            if(Input.GetButtonDown("LeftStickButton") && !isSprinting)
            {
                speed = speed * 2f;
                isSprinting = true;
            }
            if(Input.GetButtonUp("LeftStickButton"))
            {
                isSprinting = false;
                speed = speed / 2f;
            }
            if(!leftStickInUse)
            {
                if (Input.GetAxisRaw("VerticalLeft") > 0)
                {
                    this.transform.position += this.transform.forward * speed * Time.deltaTime;
                }
                if (Input.GetAxisRaw("VerticalLeft") < 0)
                {
                    this.transform.position -= this.transform.forward * speed * Time.deltaTime;
                }
                if (Input.GetAxisRaw("HorizontalLeft") > 0)
                {
                    this.transform.Rotate(Vector3.up * turnSpeed);
                }
                if (Input.GetAxisRaw("HorizontalLeft") < 0)
                {
                    this.transform.Rotate(-Vector3.up * turnSpeed);
                }
            }
            
            if (Input.GetButton("Jump"))
            {
                this.transform.position += this.transform.up * jumpHeight * Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetKeyDown(sprint) && !isSprinting)
            {
                speed = speed * 2f;
                isSprinting = true;
            }
            if (Input.GetKeyUp(sprint))
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

    public void UsingLeftArm(bool inUse)
    {
        leftStickInUse = inUse;
    }
}
