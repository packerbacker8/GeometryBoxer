using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClose : MonoBehaviour {
    public List<GameObject> enemyStands;
    public List<GameObject> walls;
    public List<GameObject> weaponStands;
    public List<GameObject> lights;
    public AudioClip doorShut0;
    public AudioClip doorShut1;
    public AudioClip wallGrow;
    public AudioClip wallSlide;
    public AudioClip tone;
    public AudioClip cageMatchSong; 
    public GameObject Ceiling;

    public float moveX = 1f;
    public float moveY = 1f;
    public float moveZ = 1f;
    public float speed = 10f;
    public float maxScale = 10f;

    private bool playedTone = false;
    private bool playedSong = false;
    private bool closing = false;
    private bool closed = false;
    private bool wallSlided = false;
    private bool growWalls = false;
    private bool growCeiling = false;
    private bool doorShut0Played = false;
    private bool doorShut1Played = false;
    private bool wallGrowPlayed = false;
    private bool ceilingGrowPlayed = false;
    private float scale;
    private Vector3 targetLocation;
    private AudioSource source;
    private AudioSource toneSource;
    private float originalLightIntensity;
    // Use this for initialization
    void Start () {
        originalLightIntensity = RenderSettings.ambientIntensity;
        source = gameObject.AddComponent<AudioSource>();
        toneSource = gameObject.AddComponent<AudioSource>();
        targetLocation = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z + moveZ);
    }
	
	// Update is called once per frame
	void Update () {
        
		if(closing && !closed)
        {
            
            if (!wallSlided)
            {
                PlayWallSlide();
                wallSlided = true;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);
            if (transform.position == targetLocation)
            {
                if (!playedTone)
                {
                    toneSource.PlayOneShot(tone,0.5f);
                    playedTone = true;
                }
                if (!doorShut0Played)
                {
                    PlayDoorShut0();
                }
                closing = false;
                closed = true;
                growWalls = true;
            }
        }
        if(closed && growWalls)
        {
            if(!wallGrowPlayed)
            {
                PlayWallGrow();
                wallGrowPlayed = true;
            }
            
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
                    if(source.isPlaying)
                    {
                        source.Stop();
                    }
                }
            }
        }
        bool done = false;
        if(growCeiling)
        {
            if(!ceilingGrowPlayed)
            {
                PlayCeilingGrow();
                ceilingGrowPlayed = true;
            }
            RenderSettings.ambientIntensity -= 0.1f * Time.deltaTime;
            Ceiling.transform.localScale = new Vector3(Ceiling.transform.localScale.x, 4f, Ceiling.transform.localScale.z + Time.deltaTime * 10f);
            if(Ceiling.transform.localScale.z > 90f)
            {
                growCeiling = false;
                for(int i = 0; i < lights.Count; i++)
                {
                    RenderSettings.ambientIntensity = originalLightIntensity;
                    lights[i].SetActive(true);
                }
                if(source.isPlaying)
                {
                    source.Stop();
                    done = true;
                }
            }
        }
        if(done && !doorShut1Played)
        {
            PlayDoorShut1();
            doorShut1Played = true;
            for (int i = 0; i < enemyStands.Count; i++)
            {
                enemyStands[i].transform.GetComponent<WeaponStand>().Rise();
            }
            if(!playedSong)
            {
                toneSource.Stop();
                toneSource.loop = true;
                toneSource.PlayOneShot(cageMatchSong,1f);
            }
        }
	}
    void CloseWall()
    {
        closing = true;
    }
    void PlayDoorShut0()
    {
        source.PlayOneShot(doorShut0, 1f);
    }
    void PlayDoorShut1()
    {
        source.PlayOneShot(doorShut1, 1f);
    }
    void PlayWallGrow()
    {
        source.PlayOneShot(wallGrow, 1f);
    }
    void PlayCeilingGrow()
    {
        source.PlayOneShot(wallGrow, 1f);
    }
    void PlayWallSlide()
    {
        source.PlayOneShot(wallSlide, 1f);
    }
}
