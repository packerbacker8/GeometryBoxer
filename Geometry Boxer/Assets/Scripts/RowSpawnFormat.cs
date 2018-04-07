using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A type of SpawnSet. 
public class RowSpawnFormat : SpawnSet
{
    private static System.Random randNum = new System.Random();
    private void Start()
    {
        randNum.Next();
    }
    /// <summary>
    /// Finds a transform out of object that goes through three children of the initial parent object.
    /// </summary>
    /// <returns>Returns transform of third layer child through random number.</returns>
    public override Transform getRandomSpawnTransform3D()
    {
        GameObject randomRow = this.gameObject.transform.GetChild(randNum.Next(0, this.gameObject.transform.childCount)).gameObject;
        int randInd = randNum.Next(0, randomRow.transform.childCount);
        GameObject randomColumn = randomRow.transform.GetChild(randInd).gameObject;
        Transform currentTransform = randomColumn.transform.GetChild(randNum.Next(0, randomColumn.transform.childCount));

        return currentTransform;
    }

    /// <summary>
    /// Finds a transform out of object that goes through two children of the initial parent object.
    /// PLEASE MAKE SURE THERE ARE MORE OPTIONS THAN THERE ARE OBJECTS LOOKING FOR A SPOT TO SPAWN.
    /// </summary>
    /// <returns>Returns transform of second layer child through random number.</returns>
    public override Transform getRandomSpawnTransform2D()
    {
        GameObject randomRow = this.gameObject.transform.GetChild(randNum.Next(0, this.gameObject.transform.childCount)).gameObject;
        if(randomRow.transform.childCount == 0)
        {
            Debug.Log("The row selected had no children, please add a child object to this row.");
            return this.transform;
        }
        Transform currentTransform = randomRow.transform.GetChild(randNum.Next(0, randomRow.transform.childCount));

        return currentTransform;
    }
}
