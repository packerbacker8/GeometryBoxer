using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagTrigger : MonoBehaviour {

    public List<GameObject> weaponStands;
    public float activateWeaponStandThreshold;
    
    private float damageTaken;
    private bool weaponStandsUp = false;

	// Use this for initialization
	void Start ()
    {
        damageTaken = 0f;
	}

    // Update is called once per frame
    void OnCollisionEnter(Collision col)
    {
        damageTaken += col.impulse.magnitude;
        if (!weaponStandsUp && damageTaken > activateWeaponStandThreshold)
        {
            for(int i = 0; i < weaponStands.Count; i++)
            {
                weaponStands[i].BroadcastMessage("Rise", SendMessageOptions.RequireReceiver);
            }
            weaponStandsUp = true;
        }
	}
}
