using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger_MainMenu : MonoBehaviour {

    public float cameraTransitionSpeed;
    public Camera camera;
    public GameObject ui_root;
    public GameObject player;
    public List<GameObject> cameraAngles = new List<GameObject>();

    private GameObject cameraTarget; 

	// Use this for initialization
	void Start () {
        Time.timeScale = 1.0f;
        if(!ui_root.activeSelf)
        {
            ui_root.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 newPos = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y, cameraTarget.transform.position.z);
        camera.transform.position = Vector3.Lerp(camera.transform.position, newPos, Time.deltaTime * cameraTransitionSpeed);

        var targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

        // Smoothly rotate towards the target point.
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, cameraTransitionSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.tag == "Player")
        {
            ui_root.SetActive(true);
            cameraTarget = cameraAngles[0];
        }
    }
    void OnTriggerStay(Collider col)
    {

    }
    void OnTriggerExit(Collider col)
    {
        if (col.transform.root.tag == "Player")
        {
            ui_root.SetActive(false);
            cameraTarget = cameraAngles[1];
        }
    }
    
    void MoveToOverhead()
    {
        cameraTarget = cameraAngles[1];
    }
    void MoveToMeleeDemo()
    {
        cameraTarget = cameraAngles[2];
    }
    void MoveToDummyDemo()
    {
        cameraTarget = cameraAngles[3];
    }
}
