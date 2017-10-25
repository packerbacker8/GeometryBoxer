using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject interactionPoint;
    public Animator playerAnimator;
    public GameObject currentPlayer;
    //public WorldInteraction playerInteractionScript; 

   // public bool isClicked = false;

    public virtual void FindPlayer(GameObject player)
    {
        currentPlayer = player;
        playerAnimator = currentPlayer.GetComponent<Animator>();
    }

    public virtual void Interact(GameObject player)
    {

    }
}