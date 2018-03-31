using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A type of SpawnSet. 
public class RowSpawnFormat : SpawnSet {

	
	void Start () {
		
	}
	
	
	void Update () {
		
	}

    public override Transform getRandomSpawnTransform()
    {
        GameObject randomRow = this.gameObject.transform.GetChild(Random.Range(0, this.gameObject.transform.childCount)).gameObject;

        Transform currentTransform = randomRow.transform.GetChild(Random.Range(0, randomRow.transform.childCount));

        return currentTransform;
    }


}
