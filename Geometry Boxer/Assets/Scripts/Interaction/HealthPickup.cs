using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 100;

    private GameObject player;
    private Vector3 startingPos;
    private int travelAmount;
    private float moveAmount;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player = player.transform.root.gameObject;
        startingPos = this.transform.position;
        travelAmount = 1;
        moveAmount = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y >= startingPos.y + travelAmount)
        {
            moveAmount = -0.05f;
        }
        else if(this.transform.position.y <= startingPos.y - travelAmount)
        {
            moveAmount = 0.05f;
        }
        this.transform.position += new Vector3(0, moveAmount,0);
    }

    public void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.transform.root.tag == "Player")
        {
            player.GetComponent<PlayerHealthScript>().GiveHealth(healAmount);
            this.gameObject.SetActive(false);
        }
    }
}
