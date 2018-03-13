using UnityEngine;
using System.Collections;

namespace RootMotion
{

    /// <summary>
    /// 3rd person camera controller.
    /// </summary>
    public class CameraController : MonoBehaviour
    {

        // When to update the camera?
        [System.Serializable]
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        public Transform target; // The target Transform to follow
        public Transform rotationSpace; // If assigned, will use this Transform's rotation as the rotation space instead of the world space. Useful with spherical planets.
        public UpdateMode updateMode = UpdateMode.LateUpdate; // When to update the camera?
        public bool lockCursor = true; // If true, the mouse will be locked to screen center and hidden
        public bool smoothFollow; // If > 0, camera will smoothly interpolate towards the target
        public float followSpeed = 10f; // Smooth follow speed
        public float distance = 10.0f; // The current distance to target
        public float minDistance = 4; // The minimum distance to target
        public float maxDistance = 10; // The maximum distance to target
        public float zoomSpeed = 10f; // The speed of interpolating the distance
        public float zoomSensitivity = 1f; // The sensitivity of mouse zoom
        public float offsetSpeed = 10f;
        public float rotationSensitivity = 3.5f; // The sensitivity of rotation
        public float yMinLimit = -20; // Min vertical angle
        public float yMaxLimit = 160; // Max vertical angle
        public Vector3 offset = new Vector3(0, 1.5f, 0.5f); // The offset from target relative to camera rotation
        public bool rotateAlways = true; // Always rotate to mouse?
        public bool rotateOnLeftButton; // Rotate to mouse when left button is pressed?
        public bool rotateOnRightButton; // Rotate to mouse when right button is pressed?
        public bool rotateOnMiddleButton; // Rotate to mouse when middle button is pressed?

        public float x { get; private set; } // The current x rotation of the camera
        public float y { get; private set; } // The current y rotation of the camera
        public float distanceTarget { get; private set; } // Get/set distance

        private Vector3 targetDistance, position, desiredPos;
        private Quaternion rotation = Quaternion.identity;
        private Vector3 smoothPosition;
        private Camera cam;
        private bool useController;
        private bool isPlayer2;
        private string[] controllerInfo;
        private string controllerHorizon;
        private string controllerVertical;
        private float distanceOffset;
        private int layer;
        private int layermask;

        // Initiate, set the params to the current transformation of the camera relative to the target
        protected virtual void Awake()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            distanceTarget = distance;
            smoothPosition = transform.position;

            cam = GetComponent<Camera>();

            lastUp = rotationSpace != null ? rotationSpace.up : Vector3.up;
            layer = 4;
            layermask = 1 << layer;
        }

        protected virtual void Start()
        {
            useController = false;
            controllerInfo = Input.GetJoystickNames();
            if (controllerInfo.Length > 0)
            {
                useController = true;
            }
            distanceOffset = 0;
            desiredPos = this.transform.position;
            if(controllerInfo.Length > 0)
            {
                controllerHorizon = controllerInfo[0].Length == 19 ? "HorizontalRightPS4" : "HorizontalRight";
                controllerVertical = controllerInfo[0].Length == 19 ? "VerticalRightPS4" : "VerticalRight";
            }
        }


        protected virtual void Update()
        {
            if (updateMode == UpdateMode.Update) UpdateTransform();
        }

        protected virtual void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate) UpdateTransform();
        }

        protected virtual void LateUpdate()
        {
            if (controllerInfo.Length > 0)
            {
                int controlIndex = isPlayer2 && controllerInfo.Length > 1 ? 1 : 0;
                controllerHorizon = controllerInfo[controlIndex].Length == 19 ? "HorizontalRightPS4" : "HorizontalRight";
                controllerVertical = controllerInfo[controlIndex].Length == 19 ? "VerticalRightPS4" : "VerticalRight";
                if (isPlayer2)
                {
                    controllerHorizon += "_2";
                    controllerVertical += "_2";
                }
            }
 
            UpdateInput();

            if (updateMode == UpdateMode.LateUpdate) UpdateTransform();
        }

        // Read the user input
        public void UpdateInput()
        {
            if (target == null || !cam.enabled) return;

            // Cursors
            //Cursor.lockState = lockCursor? CursorLockMode.Locked: CursorLockMode.None;
            //Cursor.visible = lockCursor? false: true;

            // Should we rotate the camera?
            bool rotate = rotateAlways || (rotateOnLeftButton && Input.GetMouseButton(0)) || (rotateOnRightButton && Input.GetMouseButton(1)) || (rotateOnMiddleButton && Input.GetMouseButton(2));
            bool useControl = Input.GetJoystickNames().Length > 0;
            if (useControl && rotate)
            {
                float horizontal = Input.GetAxis(controllerHorizon);
                float vert = Input.GetAxis(controllerVertical);
                x +=  horizontal * rotationSensitivity;
                y = ClampAngle(y + vert * rotationSensitivity, yMinLimit, yMaxLimit);
            }

            if (!isPlayer2 && rotate)
            {
                x += Input.GetAxis("Mouse X") * rotationSensitivity;
                y = ClampAngle(y - Input.GetAxis("Mouse Y") * rotationSensitivity, yMinLimit, yMaxLimit);
            }

            // Distance
            distanceTarget = Mathf.Clamp(distanceTarget + zoomAdd, minDistance, maxDistance);
        }

        // Update the camera transform
        public void UpdateTransform()
        {
            UpdateTransform(Time.deltaTime);
        }

        private Quaternion r = Quaternion.identity;
        private Vector3 lastUp;

        public void UpdateTransform(float deltaTime)
        {
            if (target == null || !cam.enabled) return;

            // Distance
            distance += (distanceTarget - distance) * zoomSpeed * deltaTime;

            // Rotation
            rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);


            if (rotationSpace != null)
            {
                r = Quaternion.FromToRotation(lastUp, rotationSpace.up) * r;
                rotation = r * rotation;

                lastUp = rotationSpace.up;

            }


            // Smooth follow
            if (!smoothFollow)
            {
                smoothPosition = target.position;
            }
            else
            {
                smoothPosition = Vector3.Lerp(smoothPosition, target.position, deltaTime * followSpeed);
            }

            desiredPos = smoothPosition + rotation * (offset - Vector3.forward * distance);

            Vector3 relativePos = target.position - desiredPos;
            RaycastHit hit;

            if (Physics.Raycast(desiredPos, relativePos, out hit, maxDistance/2, layermask))
            {
                //Debug.DrawRay(desiredPos, relativePos, Color.red);
                //distanceOffset = distance - hit.distance + 0.8f;
                distanceOffset += Time.deltaTime * offsetSpeed;
                distanceOffset = Mathf.Clamp(distanceOffset, 0, distance);
            }
            else if (distanceOffset > 0)
            {
                distanceOffset -= Time.deltaTime * offsetSpeed;
                if (distanceOffset < 0)
                {
                    distanceOffset = 0;
                }
            }

            // Position
            position = smoothPosition + rotation * (offset + Vector3.forward * (-distance + distanceOffset));

            // Translating the camera
            transform.position = position;
            transform.rotation = rotation;
        }

        // Zoom input
        private float zoomAdd
        {
            get
            {
                float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
                if (scrollAxis > 0) return -zoomSensitivity;
                if (scrollAxis < 0) return zoomSensitivity;
                return 0;
            }
        }

        // Clamping Euler angles
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        public void SetIsPlayer2()
        {
            isPlayer2 = true;
        }

    }
}

