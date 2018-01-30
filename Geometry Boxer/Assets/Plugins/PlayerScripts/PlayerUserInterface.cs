using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI
{
    public class PlayerUserInterface : MonoBehaviour
    {
        public Text enemyCounter;
        public Text PlayerSpecialTimer;
        public float coolDownTime;



        [Header("Health images")]
        public Texture healthLow;
        public Texture hitLow;
        public int hitLowNumber = 20;
        public Texture hitHigh;
        public int hitHighNumber = 60;

        private GameObject player;
        private GameObject enemies;
        private int numEnemiesAlive;
        private bool cooldown = false;
        private float playerCoolDownTimer;
        private float fullHealth;
        private float PlayersHealth = 15000f;
        private bool hit;
        private int hitCount;
        private float hitTimer;
        

        // Use this for initialization
        void Start()
        {
            enemies = GameObject.FindGameObjectWithTag("EnemyContainer");
            player = GameObject.FindGameObjectWithTag("Player");
            numEnemiesAlive = enemies.transform.childCount;
            enemyCounter.text = numEnemiesAlive.ToString();

            PlayerSpecialTimer.text = playerCoolDownTimer.ToString();
            hitTimer = 0;
            PlayersHealth = 15000f;
        }

        // Update is called once per frame
        void Update()
        {
            // displays enemy count
            enemyCounter.text = numEnemiesAlive.ToString();

            // displays cool down
            if (cooldown)
            {
                coolDownTime -= Time.deltaTime;
            }
            if (coolDownTime <= 0)
            {
                cooldown = false;
                coolDownTime = playerCoolDownTimer;
            }

            PlayerSpecialTimer.text = coolDownTime.ToString("n2");
            Debug.Log("PlayersHealth: " + PlayersHealth.ToString());
            Debug.Log("maxHealth: " + fullHealth.ToString());
            //Debug.Log("hitTimer: " + hitTimer.ToString());

            // if player is not hit for 1 sec, hit UI will turn off
            if (hitTimer >= 0)
            {
                hitTimer -= Time.deltaTime;
            }
            
            if(hitTimer <= 0)
            {
                hit = false;
                hitCount = 0;
            }
        }

        /// <summary>
        /// UI GUI for low health and when being hit by enemyies
        /// if you are hit 200 times hit UI will activate 
        /// </summary>
        private void OnGUI()
        {
            if (PlayersHealth < (fullHealth / 4) && !hit)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), healthLow);
            }

            if(hit && hitCount > hitLowNumber && hitCount < hitHighNumber)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitLow);
            }
            else if (hit && hitCount > hitHighNumber)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitHigh);
            }
            else if(hit && hitCount < hitLowNumber && PlayersHealth < (fullHealth / 4))
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), healthLow);
            }

        }

        // functions called from other classes
        public void enemyIsKilled()
        {
            numEnemiesAlive--;
        }

        public void UsedSpecialAttack()
        {
            cooldown = true;
        }

        public void SetCoolDownTime(float time)
        {
            coolDownTime = time;
        }

        public void SetDefultcoolDownTime(float time)
        {
            playerCoolDownTimer = time;
        }

        public void SetMaxHealth(float health)
        {
            fullHealth = health;
        }

        public void SetHealth(float health)
        {
            PlayersHealth = health;
        }

        public void setHitUIimage(bool h, int count)
        {
            hitCount += count;
            hit = h;
            hitTimer = 1f;
        }
    }
}
