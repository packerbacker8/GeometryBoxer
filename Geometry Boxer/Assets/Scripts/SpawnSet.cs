using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents an object that can provide the user valid spawn locations.
public abstract class SpawnSet : MonoBehaviour
{
    public abstract Transform getRandomSpawnTransform();
}
