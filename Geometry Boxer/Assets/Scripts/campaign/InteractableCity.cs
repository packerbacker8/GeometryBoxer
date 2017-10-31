using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableCity : Interactable
{
    public GUIStyle style;
    public GameObject Player;
    public GameObject MapCamera;
    public GameObject Canvas;
    public int sceneIndex;
   
    private WorldInteraction worldInit;
    private RTSCam cam;

    private void Awake()
    {
        worldInit = Player.GetComponent<WorldInteraction>();
        cam = MapCamera.GetComponent<RTSCam>();
        Canvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if(hit.transform.tag == "Interactable")
                {
                    worldInit.freeze = true;
                    cam.freeze = true;
                    Canvas.SetActive(true);
                }
            }
        }


    }
}
