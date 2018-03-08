using UnityEngine;
using System.Collections;

namespace RootMotion.Demos
{

    /// <summary>
    /// User input for a third person character controller.
    /// </summary>
    public class UserControlThirdPerson : MonoBehaviour
    {

        // Input state
        public struct State
        {
            public Vector3 move;
            public Vector3 lookPos;
            public bool crouch;
            public bool jump;
            public int actionIndex;
        }

        public bool walkByDefault;        // toggle for walking state
        public bool canCrouch = true;
        public bool canJump = true;

        public State state = new State();           // The current state of the user input

        protected Transform cam;                    // A reference to the main camera in the scenes transform

        private string jumpButton = "Jump";
        private bool isPlayer2 = false;
        void Start()
        {
            // get the transform of the main camera
            cam = this.transform.root.GetComponentInChildren<Camera>().transform;
        }

        protected virtual void Update()
        {
            int controlIdx = isPlayer2 && Input.GetJoystickNames().Length > 1 ? 1 : 0;
            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[controlIdx].Length == 19)
            {
                changeToPSControl();
            }
            else
            {
                changeToXBoxControl();
            }

            // read inputs
            state.crouch = canCrouch && Input.GetKey(KeyCode.C) && !isPlayer2;
            state.jump = canJump && Input.GetButton(jumpButton);

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            float h2 = Input.GetAxisRaw("Horizontal_2");
            float v2 = Input.GetAxisRaw("Vertical_2");


            // calculate move direction
            Vector3 move = isPlayer2 ? cam.rotation * new Vector3(h2, 0f, v2).normalized : cam.rotation * new Vector3(h, 0f, v).normalized;

            // Flatten move vector to the character.up plane
            if (move != Vector3.zero)
            {
                Vector3 normal = transform.up;
                Vector3.OrthoNormalize(ref normal, ref move);
                state.move = move;
            }
            else state.move = Vector3.zero;

            bool walkToggle = Input.GetKey(KeyCode.LeftShift);

            // We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
            float walkMultiplier = (walkByDefault ? walkToggle ? 1 : 0.5f : walkToggle ? 0.5f : 1);

            state.move *= walkMultiplier;

            // calculate the head look target position
            state.lookPos = transform.position + cam.forward * 100f;
        }

        private void changeToPSControl()
        {
            jumpButton = "BButton";
            if(isPlayer2)
            {
                jumpButton += "_2";
            }
        }

        private void changeToXBoxControl()
        {
            jumpButton = "Jump";
            if (isPlayer2)
            {
                jumpButton += "_2";
            }
        }

        public void SetIsPlayer2()
        {
            isPlayer2 = true;
        }
    }
}

