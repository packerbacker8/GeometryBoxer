using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitscreenOrientation : MonoBehaviour
{
    public Camera player1Cam;
    public Camera player2Cam;
    [Tooltip("How is the screen split by the cameras? True is horizontal splitscreen, false vertical.")]
    public bool Horizontal = true;

    private bool isSplitscreen;

    private void Start()
    {
        isSplitscreen = this.GetComponent<GameControllerScript>().IsSplitScreen;
        SetupCameraRect();
    }

    public void ChangeOrientation(bool horizon)
    {
        Horizontal = horizon;
        SetupCameraRect();
    }

    public void SetupCameraRect()
    {
        if(isSplitscreen)
        {
            if(Horizontal)
            {
                player1Cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                player2Cam.rect = new Rect(0, 0, 1, 0.5f);
            }
            else
            {
                player1Cam.rect = new Rect(0, 0, 0.5f, 1);
                player2Cam.rect = new Rect(0.5f, 0, 0.5f, 1);
            }
        }
        else
        {
            player1Cam.rect = new Rect(0, 0, 1, 1);
        }
    }
}
