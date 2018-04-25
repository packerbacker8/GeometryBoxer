using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Reflection;
using System;

public class AutoGenerateSpawnPoints : ScriptableWizard
{
    [Tooltip("Density of how many points there will be to spawn.")]
    [SerializeField]
    public Density DensityOfSpawnPoints = Density.Medium;
    public NavMeshData nav;
    public GameObject sp;

    private GameObject rootSpawn;

    public enum Density : int
    {
        VeryLow,
        Low,
        MediumLow,
        Medium,
        MediumHigh,
        High,
        VeryHigh,
        PleaseNo
    }
    [MenuItem("Geometry Boxer Tools/Auto Generate Spawn Points")]

    static void AutoGenerateSpawnWizard()
    {
        ScriptableWizard.DisplayWizard<AutoGenerateSpawnPoints>("Auto Generate Spawn Points", "Generate!", "Clear Points");
    }

    //first launch
    private void Awake()
    {
        rootSpawn = GameObject.FindGameObjectWithTag("RootSpawn");
        if(rootSpawn == null)
        {
            rootSpawn = new GameObject("RootSpawn")
            {
                tag = "RootSpawn"
            };
            rootSpawn.AddComponent<RowSpawnFormat>();
        }
    }

    void OnWizardCreate()
    {
        float densityFactor = 1;
        switch (DensityOfSpawnPoints)
        {
            case Density.VeryLow:
                densityFactor = 15;
                break;
            case Density.Low:
                densityFactor = 10;
                break;
            case Density.MediumLow:
                densityFactor = 7.5f;
                break;
            case Density.Medium:
                densityFactor = 5;
                break;
            case Density.MediumHigh:
                densityFactor = 3.5f;
                break;
            case Density.High:
                densityFactor = 2;
                break;
            case Density.VeryHigh:
                densityFactor = 1;
                break;
            case Density.PleaseNo:
                densityFactor = 0.1f;
                break;
            default:
                densityFactor = 1;
                break;
        }

        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();
        Vector3[] pts = navMeshTriangulation.vertices;
        float maxDist = -1.0f;
        Vector3 beginPt = Vector3.zero;
        Vector3 finalPt = Vector3.one;
        for (int i = 0; i < pts.Length - 1; i++)
        {

            for (int j = i + 1; j < pts.Length; j++)
            {
                if (maxDist < Mathf.Abs(Vector3.Distance(pts[i], pts[j])))
                {
                    maxDist = Mathf.Abs(Vector3.Distance(pts[i], pts[j]));
                    beginPt = pts[i];
                    finalPt = pts[j];
                }
            }
        }
        Vector3 mid = new Vector3((beginPt.x + finalPt.x) / 2f, (beginPt.y + finalPt.y) / 2f, (beginPt.z + finalPt.z) / 2f);
        Bounds navBounds = new Bounds(mid, Vector3.one);
        foreach (Vector3 pt in pts)
        {
            navBounds.Encapsulate(pt);
        }

        Vector3 startPt = navBounds.min;
        Vector3 endPt = navBounds.max;
        Vector3 navSize = navBounds.size;
        for (float x = startPt.x; x < endPt.x; x += densityFactor)
        {
            GameObject spawnLayerX = new GameObject("SpawnLayerX" + (int)x);
            spawnLayerX.transform.parent = rootSpawn.transform;
            for (float z = startPt.z; z < endPt.z; z += densityFactor)
            {
                GameObject spawnLayerZ = new GameObject("SpawnLayerZ" + (int)z);
                spawnLayerZ.transform.parent = spawnLayerX.transform;
                for (float y = startPt.y; y < endPt.y + 0.5f; y++) //not sure if should be y++ or y+= densityFactor
                {
                    NavMeshHit hit;
                    Vector3 pos = new Vector3(x, y, z);
                    float distToCheck = 0.5f; // not sure
                    if (NavMesh.SamplePosition(pos, out hit, distToCheck, NavMesh.AllAreas))
                    {
                        GameObject spawnPoint;
                        if (sp != null)
                        {
                            spawnPoint = Instantiate(sp);
                            spawnPoint.GetComponent<ParticleSystem>().Stop();
                        }
                        else
                        {
                            spawnPoint = new GameObject("SpawnPoint");
                            spawnPoint.AddComponent<BoxCollider>();
                            spawnPoint.GetComponent<BoxCollider>().isTrigger = true;
                        }
                        spawnPoint.transform.parent = spawnLayerZ.transform;
                        spawnPoint.transform.position = new Vector3(pos.x, pos.y + 1.0f, pos.z);
                    }
                }
                if(spawnLayerZ.transform.childCount == 0)
                {
                    DestroyImmediate(spawnLayerZ);
                }
            }
        }
    }

    private void OnDestroy()
    {

    }

    private void OnWizardOtherButton()
    {
        int childrenInObject = rootSpawn.transform.childCount;
        for (int i = childrenInObject - 1; i > -1; i--)
        {
            DestroyImmediate(rootSpawn.transform.GetChild(i).gameObject);
        }
    }
}
