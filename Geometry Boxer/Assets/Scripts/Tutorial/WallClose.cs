using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClose : MonoBehaviour {
    public List<GameObject> enemyStands;
    public List<GameObject> walls;
    public List<GameObject> weaponStands;
    public List<GameObject> lights;
    public GameObject Ceiling;

    public float moveX = 1f;
    public float moveY = 1f;
    public float moveZ = 1f;
    public float speed = 10f;
    public float maxScale = 10f;

    private bool closing = false;
    private bool closed = false;
    private bool growWalls = false;
    private bool growCeiling = false;
    private float scale;
    private Vector3 targetLocation;
	// Use this for initialization
	void Start () {
		targetLocation = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z + moveZ);
    }
	
	// Update is called once per frame
	void Update () {
		if(closing && !closed)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);
            if (transform.position == targetLocation)
            {
                closing = false;
                closed = true;
                growWalls = true;
                for(int i = 0; i < enemyStands.Count; i++)
                {
                    enemyStands[i].transform.GetComponent<WeaponStand>().Rise();
                }
            }
        }
        if(closed && growWalls)
        {
            for(int i = 0; i < walls.Count; i++)
            {
                walls[i].transform.localScale = new Vector3(walls[i].transform.localScale.x, walls[i].transform.localScale.y + Time.deltaTime, walls[i].transform.localScale.z);
                walls[i].transform.position = new Vector3(walls[i].transform.position.x, walls[i].transform.localScale.y / 2f, walls[i].transform.position.z);
                if (walls[i].transform.localScale.y > maxScale)
                {
                    growWalls = false;
                    growCeiling = true;
                    Ceiling.transform.localScale = new Vector3(Ceiling.transform.localScale.x, 4f, 0f);
                    break;
                }
            }
        }
        if(growCeiling)
        {
            Ceiling.transform.localScale = Ceiling.transform.localScale = new Vector3(Ceiling.transform.localScale.x, 4f, Ceiling.transform.localScale.z + Time.deltaTime * 10f);
            if(Ceiling.transform.localScale.z > 100f)
            {
                growCeiling = false;
                for(int i = 0; i < lights.Count; i++)
                {
                    lights[i].SetActive(true);
                }
            }
        }
	}
    void CloseWall()
    {
        closing = true;
    }
}
