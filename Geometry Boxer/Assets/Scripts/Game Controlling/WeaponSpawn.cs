using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
    public GameObject WeaponSpawnLocationPrefab;
    public List<GameObject> WeaponTypes;

    private GameObject WeaponContainer;
    private int[] typeCount;
    private List<Vector3> WeaponSpawnLocations;
    private static System.Random rand;
    // Use this for initialization
    void Start()
    {
        rand = new System.Random();
        rand.Next();
        //find weapons
        WeaponContainer = GameObject.FindGameObjectWithTag("MeleeWeapons");
        typeCount = new int[WeaponTypes.Count];
        for (int i = 0; i < typeCount.Length; i++)
        {
            typeCount[i] = 0;
        }
        WeaponSpawnLocations = new List<Vector3>();
        for (int i = 0; i < WeaponContainer.transform.childCount; i++)
        {
            //add weapons to this list so we can spawn the correct weapon if it goes missing
            for (int j = 0; j < WeaponTypes.Count; j++)
            {
                if(WeaponTypes[j].name.Contains(WeaponContainer.transform.GetChild(i).gameObject.name))
                {
                    typeCount[j]++;
                    break;
                }
            }
            //Instantiate(WeaponSpawnLocationPrefab, WeaponsInWorld[i].transform.position, WeaponsInWorld[i].transform.rotation, this.transform);
            //add locations to this list
            WeaponSpawnLocations.Add(WeaponContainer.transform.GetChild(i).position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int weaponCount = GameObject.FindGameObjectsWithTag("Interactable").Length;
        if(weaponCount < WeaponSpawnLocations.Count)
        {
            SpawnNewWeapon();
        }
    }

    /// <summary>
    /// Spawn a weapon that has gone missing.
    /// </summary>
    private void SpawnNewWeapon()
    {
        int weaponType = CountAndCompareTypes();
        if (weaponType == -1)
        {
            Debug.Log("Did not find weapon type.");
            return;
        }
        Instantiate(WeaponTypes[weaponType], WeaponSpawnLocations[rand.Next(WeaponSpawnLocations.Count)], Quaternion.identity, WeaponContainer.transform);
    }

    private int CountAndCompareTypes()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Interactable");
        int[] currentTypeCount = new int[typeCount.Length];
        foreach(GameObject obj in objs)
        {
            for (int j = 0; j < WeaponTypes.Count; j++)
            {
                if (obj.name.Contains(WeaponTypes[j].name))
                {
                    currentTypeCount[j]++;
                    break;
                }
            }
        }
        for (int i = 0; i < currentTypeCount.Length; i++)
        {
            if(currentTypeCount[i] < typeCount[i])
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Get the type of the weapon to spawn.
    /// </summary>
    /// <returns>0 to 6 is weapon type.</returns>
    private int GetWeaponType(GameObject objDeleted)
    {
        for (int i = 0; i < WeaponTypes.Count; i++)
        {
            if (WeaponTypes[i].name.Contains(objDeleted.name))
            {
                return i;
            }
        }
        return -1;
    }
}
