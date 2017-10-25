using UnityEngine;

public class QuinCamera : MonoBehaviour
{
    // assign the player here in Inspector
    public Transform target;
    // this is the 'default' offset for the camera
    // I simply used the difference in between player and camera
    // in the scene editor, but you could make it dynamic
    private Vector3 offset = new Vector3(0, 14, -7);
    // the camera distance away from the player; helps keep things smooth
    private float distance = 15f;
    // how quickly the camera rotates around the player
    private float turnSpeed = 5f;

    // custom function to manipulate the camera to always look
    // at the player
    void LookAtPlayer()
    {
        // (re)setting the offset depending on where the player moves the camera
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        // camera to follow the player
        transform.position = target.position + offset.normalized * distance;
        // make the camera look at the player
        transform.LookAt(target.position);
    }

    // let's get the camera to look at the player ASAP
    void Start()
    {
        LookAtPlayer();
    }

    // making changes whilst the game is running
    void LateUpdate()
    {
        LookAtPlayer();
        if (Input.GetKey(KeyCode.X))
        {
            // right clicking to make the camera look at the player
            LookAtPlayer();
        }
        else
        {
            // otherwise let's just follow
            transform.position = target.position + offset.normalized * distance;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            distance++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            distance--;
        }
    }
}
