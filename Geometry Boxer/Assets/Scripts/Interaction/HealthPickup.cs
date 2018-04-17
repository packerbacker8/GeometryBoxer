using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 100;
    public float RespawnDelay = 10f;
    public float RemoveHealthEffect = 3f;
    public GameObject healthGainedEffectPrefab;
    public AudioClip healthPickup;

    private bool waiting;
    private float travelAmount;
    private float moveAmount;
    private float timer;
    private Vector3 startingPos;
    private AudioSource source;
    private GameObject orb;

    // Use this for initialization
    void Start()
    {
        startingPos = this.transform.position;
        travelAmount = 0.5f;
        moveAmount = 0.5f;
        waiting = false;
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.75f;
        orb = this.gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y >= startingPos.y + travelAmount)
        {
            moveAmount = -0.5f * Time.deltaTime;
        }
        else if(this.transform.position.y <= startingPos.y - travelAmount)
        {
            moveAmount = 0.5f * Time.deltaTime;
        }
        this.transform.position += new Vector3(0, moveAmount,0);
        if(waiting)
        {
            timer += Time.deltaTime;
            if(timer >= RespawnDelay)
            {
                waiting = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                this.gameObject.GetComponent<SphereCollider>().enabled = true;
                orb.SetActive(true);
            }
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        GameObject colObj = col.transform.root.gameObject;

        //This is a tag for hazards like traps and the jeep
        if (col.transform.tag == "PlayerEnemy") return;

        bool destroy = false;
        GameObject healthThingy = null;
        if (col.gameObject.transform.root.tag == "Player")
        {
            float currentHealth = colObj.GetComponent<PlayerStatsBaseClass>().GetPlayerHealth();
            float healthToAdd = healAmount;
            if (colObj.GetComponent<OctahedronStats>() != null)
            {
                //If player is not full health
                if (colObj.GetComponent<OctahedronStats>().GetOriginalHealth() > currentHealth)
                {
                    //Set amount of health to add either to amount of pickup if less than difference of currentHealth and player's max health. Otherwise, add the difference
                    healthToAdd = colObj.GetComponent<OctahedronStats>().GetOriginalHealth() - currentHealth > healAmount ? healAmount : colObj.GetComponent<OctahedronStats>().GetOriginalHealth() - currentHealth;
                    colObj.GetComponent<OctahedronStats>().GiveHealth((int)healthToAdd);
                    colObj.GetComponent<OctahedronStats>().UpdateHealthUI();
                    destroy = true;
                }
            }
            else if (col.transform.root.gameObject.GetComponent<CubeSpecialStats>() != null)
            {
                if (colObj.GetComponent<CubeSpecialStats>().GetOriginalHealth() > currentHealth)
                {
                    healthToAdd = colObj.GetComponent<CubeSpecialStats>().GetOriginalHealth() - currentHealth > healAmount ? healAmount : colObj.GetComponent<CubeSpecialStats>().GetOriginalHealth() - currentHealth;
                    colObj.GetComponent<CubeSpecialStats>().GiveHealth((int)healthToAdd);
                    colObj.GetComponent<CubeSpecialStats>().UpdateHealthUI();
                    destroy = true;
                }
            }
            else if (col.transform.root.gameObject.GetComponent<SphereSpecialStats>() != null)
            {
                if (colObj.GetComponent<SphereSpecialStats>().GetOriginalHealth() > currentHealth)
                {
                    healthToAdd = colObj.GetComponent<SphereSpecialStats>().GetOriginalHealth() - currentHealth > healAmount ? healAmount : colObj.GetComponent<SphereSpecialStats>().GetOriginalHealth() - currentHealth;
                    colObj.GetComponent<SphereSpecialStats>().GiveHealth((int)healthToAdd);
                    colObj.GetComponent<SphereSpecialStats>().UpdateHealthUI();
                    destroy = true;
                }
            }
            if(destroy)
            {
                healthThingy = Instantiate(healthGainedEffectPrefab, colObj.GetComponentInChildren<UserControlMelee>().transform.position, colObj.GetComponentInChildren<UserControlMelee>().transform.rotation, colObj.GetComponentInChildren<UserControlMelee>().transform);
            }
        }
        else if(col.gameObject.transform.root.tag.Contains("Enemy") || col.gameObject.transform.root.tag.Contains("Ally"))
        {
            colObj = col.gameObject;
            while(!colObj.tag.Contains("EnemyRoot") && !colObj.tag.Contains("AllyRoot"))
            {
                colObj = colObj.transform.parent.gameObject;
            }
            float currentHealth = colObj.GetComponent<EnemyHealthScript>().EnemyHealth;
            float originalHealth = colObj.GetComponent<EnemyHealthScript>().GetEnemyOriginalHealth();
            float healthToAdd = healAmount;
            if (originalHealth > currentHealth)
            {
                healthToAdd = originalHealth - currentHealth > healAmount ? healAmount : originalHealth - currentHealth;
                colObj.GetComponent<EnemyHealthScript>().AddHealth(healthToAdd);
                colObj.GetComponent<EnemyHealthScript>().SetOurTarget();
                healthThingy = Instantiate(healthGainedEffectPrefab, colObj.GetComponentInChildren<UserControlAI>().transform.position, colObj.GetComponentInChildren<UserControlAI>().transform.rotation, colObj.GetComponentInChildren<UserControlAI>().transform);
                destroy = true;
            }
        }
        if(destroy)
        {
            timer = 0f;
            waiting = true;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
            source.PlayOneShot(healthPickup, 1f);
            orb.SetActive(false);
            if(healthThingy != null)
            {
                Destroy(healthThingy, RemoveHealthEffect);
            }
        }
    }

    public void ChangeStartingPosition(Vector3 pos)
    {
        startingPos = pos;
    }
}
