using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A type of SpawnSet. 
public class RowSpawnFormat : SpawnSet
{
    /// <summary>
    /// Finds a transform out of object that goes through three children of the initial parent object.
    /// </summary>
    /// <returns>Returns transform of third layer child through random number.</returns>
    public override Transform getRandomSpawnTransform3D()
    {
        GameObject randomRow = this.gameObject.transform.GetChild(Random.Range(0, this.gameObject.transform.childCount)).gameObject;
        int randInd = Random.Range(0, randomRow.transform.childCount);
        GameObject randomColumn = randomRow.transform.GetChild(randInd).gameObject;
        Transform currentTransform = randomColumn.transform.GetChild(Random.Range(0, randomColumn.transform.childCount));

        return currentTransform;
    }

    /// <summary>
    /// Finds a transform out of object that goes through two children of the initial parent object.
    /// </summary>
    /// <returns>Returns transform of second layer child through random number.</returns>
    public override Transform getRandomSpawnTransform2D()
    {
        GameObject randomRow = this.gameObject.transform.GetChild(Random.Range(0, this.gameObject.transform.childCount)).gameObject;
        Transform currentTransform = randomRow.transform.GetChild(Random.Range(0, randomRow.transform.childCount));

        return currentTransform;
    }
}
