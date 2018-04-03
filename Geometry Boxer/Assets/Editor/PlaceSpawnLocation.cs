using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlaceSpawnLocation : ScriptableWizard
{
    [Tooltip("0 is the lowest level, 2 is the highest, always")]
    [SerializeField]
    public Levels HeightLevelToSpawnOn = Levels.Low;

    private GameObject spawnPlacer;
    private GameObject rootSpawn;
    public enum Levels : int
    {
        Low = 0,
        Medium,
        High
    }
    [MenuItem("Geometry Boxer Tools/Add Spawn Points")]

    static void SelectAllOfTagWizard()
    {
        ScriptableWizard.DisplayWizard<PlaceSpawnLocation>("Add Spawn Points", "Finished", "Add Spawn Point");
    }

    private void Awake()
    {
        spawnPlacer = new GameObject("SpawnPlacer");
        spawnPlacer.AddComponent<BoxCollider>();
        spawnPlacer.AddComponent<Light>();
        spawnPlacer.GetComponent<Light>().color = Color.red;
        spawnPlacer.GetComponent<Light>().intensity = 10f;
        rootSpawn = GameObject.FindGameObjectWithTag("RootSpawn");
        if (rootSpawn == null)
        {
            rootSpawn = new GameObject("RootSpawn");
            rootSpawn.tag = "RootSpawn";
            rootSpawn.AddComponent<RowSpawnFormat>();
            GameObject level1 = new GameObject("LowLevelSpawn");
            level1.transform.parent = rootSpawn.transform;
            GameObject level2 = new GameObject("MediumLevelSpawn");
            level2.transform.parent = rootSpawn.transform;
            GameObject level3 = new GameObject("HighLevelSpawn");
            level3.transform.parent = rootSpawn.transform;
        }
    }

    void OnWizardCreate()
    {
        //DestroyImmediate(spawnPlacer);
    }

    private void OnDestroy()
    {
        DestroyImmediate(spawnPlacer);
    }

    private void OnWizardOtherButton()
    {
        GameObject spawnToPlace = new GameObject("SpawnPoint");
        spawnToPlace.AddComponent<BoxCollider>();
        spawnToPlace.GetComponent<BoxCollider>().isTrigger = true;
        spawnToPlace.transform.position = spawnPlacer.transform.position;
        spawnToPlace.transform.parent = rootSpawn.transform.GetChild((int)HeightLevelToSpawnOn);
    }
}
