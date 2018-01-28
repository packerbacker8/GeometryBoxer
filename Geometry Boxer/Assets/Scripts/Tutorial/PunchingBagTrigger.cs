using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchingBagTrigger : MonoBehaviour {

    public GameObject wall;
    public List<GameObject> weaponStands;
    public float activateWeaponStandThreshold;
    public Canvas canvas;
    public AudioClip ping;
    public AudioClip up;
    public AudioClip open;

    private KeyCode jab;
    private KeyCode hook;
    private KeyCode combo;
    private KeyCode kick;
    
    private Text text;
    private KeyCode lastAttack; 
    private float damageTaken;
    private float prevDamage;
    private float wallSpeed;
    private float meleeDamageStart;
    private float meleeDamageEnd;
    private bool weaponStandsUp = false;
    private CubeAttackScript punchScript;
    private Vector3 wallTargetLocation;
    private AudioSource pinger;

    private bool jabbed = false;
    private bool hooked = false;
    private bool comboed = false;
    private bool kicked = false;
    private bool lastPing = false;

    // Use this for initialization
    void Start ()
    {
        damageTaken = 0f;
        prevDamage = 0f;
        wallSpeed = 3f;
        text = canvas.transform.GetChild(0).GetComponent<Text>();
        punchScript = GameObject.FindGameObjectWithTag("Player").transform.root.gameObject.GetComponent<CubeAttackScript>();
        pinger = gameObject.AddComponent<AudioSource>();

        jab = punchScript.rightJabKey;
        combo = punchScript.leftJabKey;
        hook = punchScript.rightUppercutKey;
        kick = punchScript.hiKickKey;

        wallTargetLocation = new Vector3(wall.transform.position.x, wall.transform.position.y - 20f, wall.transform.position.z);
	}
    public void setJabText()
    {
        text.text = "Hit the punching bag with " + jab.ToString();
        pinger.PlayOneShot(ping, 0.5f);
    }
    void Update()
    {
        if(Input.GetKeyDown(jab) && !jabbed && damageTaken > prevDamage)
        {
            jabbed = true;
            text.text = "Good! Hook the bag with " + hook.ToString();
            pinger.PlayOneShot(ping, 0.5f);
        }
        else if (Input.GetKeyDown(hook) && !hooked && jabbed && damageTaken > prevDamage)
        {
            hooked = true;
            text.text = "Nice! Combo hit the punching bag with " + combo.ToString();
            pinger.PlayOneShot(ping, 0.5f);
        }
        else if(Input.GetKeyDown(combo) && !comboed && hooked && jabbed && damageTaken > prevDamage)
        {
            comboed = true;
            text.text = "Ouch! Kick the bag with " + kick.ToString();
            pinger.PlayOneShot(ping, 0.5f);
        }
        else if(Input.GetKeyDown(kick) && !kicked && jabbed && hooked && comboed && damageTaken > prevDamage)
        {
            kicked = true;
            if (!weaponStandsUp)
            {
                for (int i = 0; i < weaponStands.Count; i++)
                {
                    weaponStands[i].BroadcastMessage("Rise", SendMessageOptions.RequireReceiver);
                }
                weaponStandsUp = true;
                pinger.PlayOneShot(up, 0.25f);
            }
            text.text = "Grab a melee weapon from behind you!\nThe same buttons swing it around. Press X to drop items. \nWhack the punching bag some more.";
            pinger.PlayOneShot(ping, 0.5f);
            meleeDamageStart = damageTaken;
            meleeDamageEnd = meleeDamageStart + 50f;
        }
        else if(damageTaken > meleeDamageEnd && kicked)
        {
            text.text = "That's enough. Head into the arena.";
            wall.transform.position = Vector3.MoveTowards(wall.transform.position, wallTargetLocation, wallSpeed * Time.deltaTime);
            if(!lastPing)
            {
                pinger.PlayOneShot(ping, 0.5f);
                lastPing = true;
                pinger.PlayOneShot(open, 0.5f);
            }
        }

    }

    // Update is called once per frame
    void OnCollisionEnter(Collision col)
    {
        if(col.transform.root.tag == "Player")
        {
            prevDamage = damageTaken;
            damageTaken += col.impulse.magnitude;
        }
	}
}
