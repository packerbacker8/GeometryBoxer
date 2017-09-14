using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchScript : MonoBehaviour
{

    public bool useController = true;
    public Rigidbody leftArm;
    public Rigidbody rightArm;
    public float punchForce = 50f;

    /// <summary>
    /// Private variables for controls in punching and moving arms.
    /// </summary>
    private bool controllingLeftArm;
    private bool controllingRightArm;
   
    private float leftArmXAxis;
    private float leftArmYAxis;
    private float rightArmXAxis;
    private float rightArmYAxis;
    
    private enum Limbs
    {
        leftArm = 0,
        rightArm
    };

    // Use this for initialization
    void Start()
    {
        controllingLeftArm = false;
        controllingRightArm = false;
        leftArmXAxis = 0f;
        leftArmYAxis = 0f;
        rightArmXAxis = 0f;
        rightArmYAxis = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (useController)
        {
            //Left trigger lets player control left arm independently
            if (Input.GetAxis("LeftTrigger") > 0)
            {
                controllingLeftArm = true;
                this.transform.gameObject.SendMessage("UsingLeftArm", controllingLeftArm);
            }
            else
            {
                controllingLeftArm = false;
                this.transform.gameObject.SendMessage("UsingLeftArm", controllingLeftArm);
            }
            //Right trigger lets player control right arm independently
            if (Input.GetAxis("RightTrigger") > 0)
            {
                controllingRightArm = true;
            }
            else
            {
                controllingRightArm = false;
            }
            //Left arm punching
            if (controllingLeftArm)
            {
                leftArmXAxis = Input.GetAxisRaw("HorizontalLeft");
                leftArmYAxis = Input.GetAxisRaw("VerticalLeft");
                MoveArm(Limbs.leftArm, leftArmXAxis, leftArmYAxis);
            }
            else if (Input.GetButtonDown("LeftBumper")) //left bumper
            {
                ThrowSinglePunch(Limbs.leftArm);
            }
            //Right arm punching
            if (controllingRightArm)
            {
                rightArmXAxis = Input.GetAxisRaw("HorizontalRight");
                rightArmYAxis = Input.GetAxisRaw("VerticalRight");
                MoveArm(Limbs.rightArm, rightArmXAxis, rightArmYAxis);
            }
            else if (Input.GetButtonDown("RightBumper"))
            {
                ThrowSinglePunch(Limbs.rightArm);
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ThrowSinglePunch(Limbs.leftArm);

            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                ThrowSinglePunch(Limbs.rightArm);

            }
        }

    }

    /// <summary>
    /// Cause player to throw single punch of one arm.  Arm is determined by limb parameter.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    private void ThrowSinglePunch(Limbs limb)
    {
        Rigidbody armToMove = null;
        if(limb == Limbs.leftArm)
        {
            armToMove = leftArm;
        }
        else if(limb == Limbs.rightArm)
        {
            armToMove = rightArm;
        }
        armToMove.AddForce((this.transform.forward + this.transform.up) * punchForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Allows player to move limb about as they choose with the controller joysticks.
    /// </summary>
    /// <param name="limb">The value 0 corresponds to left arm, 1 to right arm.</param>
    /// <param name="xMotion">Gets axis value from input for left and right sticks. In horizontal direction.</param>
    /// <param name="yMotion">Gets axis value from input for left and right sticks. In vertical direction.</param>
    private void MoveArm(Limbs limb, float xMotion, float yMotion)
    {
        Rigidbody armToMove = null;
        if (limb == Limbs.leftArm)
        {
            armToMove = leftArm;
        }
        else if (limb == Limbs.rightArm)
        {
            armToMove = rightArm;
        }
        Vector3 directionToMove = new Vector3(xMotion, yMotion);
        armToMove.AddForce(directionToMove * punchForce * 10f);
    }
}
