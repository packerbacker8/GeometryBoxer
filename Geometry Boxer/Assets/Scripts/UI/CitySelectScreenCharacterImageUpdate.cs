using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitySelectScreenCharacterImageUpdate : MonoBehaviour {

    public GameObject[] playerOptions;
    public GameObject playerImage;

    public Sprite cube;
    public Sprite sphere;
    public Sprite octahedron;
    
    private Image image;
    private GameObject activePlayer;

    private string cubeName = "Cubeman";
    private string sphereName = "Sphere";
    private string octaName = "Octahedron";

    // Use this for initialization
    void Start () {
        for (int i = 0; i < playerOptions.Length; i++)
        {
            if (playerOptions[i].name.Contains(SaveAndLoadGame.saver.GetCharacterType()))
            {
                activePlayer = playerOptions[i];
            }
        }

        Debug.Log("Active Player Name: " + activePlayer.name.ToString());

        image = playerImage.GetComponent<Image>();

        if(activePlayer.name.Contains(cubeName))
        {
            image.sprite = cube;
        }
        else if (activePlayer.name.Contains(sphereName))
        {
            image.sprite = sphere;
        }
        else if (activePlayer.name.Contains(octaName))
        {
            image.sprite = octahedron;
        }
    }
}
