using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI
{
    public class PlayerUserInterface : MonoBehaviour
    {
        public Text enemyCounter;
        public Text HealthNumber;
        public Text CooldownPercent;
        public Transform specialAttackBar;
        public Transform InnerLoop;
        public Text percentSign;
        public Transform specialAttackFullCube;
        public Transform specialAttackFullOcta;
        public Sprite glowingBar;
        public Transform healthBar;


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
        private float coolDownTime;
        private float cooldownPercent;
        private Color Yellow = new Color(255 / 255.0F, 203 / 255.0F, 0, 1);
        private Color Blue = new Color(115 / 255.0F, 189 / 255.0F, 234 / 255.0F, 1);
        private Color Red = new Color(255 / 255.0F, 25 / 255.0F, 25 / 255.0F, 1);
        private Color Green = new Color(0 / 255.0F, 255 / 255.0F, 72 / 255.0F, 1);
        private bool PlayerIsCube;
        private bool PlayerIsOcta;
        private bool usingSpecialAttack;
        private string SpecialAttackButton = "B";


        // Use this for initialization
        void Awake()
        {
            enemies = GameObject.FindGameObjectWithTag("EnemyContainer");
            player = GameObject.FindGameObjectWithTag("Player");

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
            {
                numEnemiesAlive = 5;
                enemyCounter.text = "Tutorial";
            }
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("MainMenu"))
            {
                numEnemiesAlive = 1;
                enemyCounter.text = "";
            }
            else
            {
                numEnemiesAlive = enemies.transform.childCount;
                enemyCounter.text = numEnemiesAlive.ToString();
            }

            hitTimer = 0;
            // PlayersHealth = 15000f;
        }

        // Update is called once per frame
        void Update()
        {
            // displays enemy count
            enemyCounter.text = numEnemiesAlive.ToString();

            // display health
            HealthNumber.text = ((int)PlayersHealth / 100).ToString() + " / " + ((int)fullHealth / 100).ToString();
            if (PlayersHealth < (fullHealth / 2.5) && PlayersHealth > (fullHealth / 4))
            {
                healthBar.GetComponent<Image>().color = Yellow;
            }
            else if (PlayersHealth < (fullHealth / 3.5))
            {
                healthBar.GetComponent<Image>().color = Red;
            }
            else
            {
                healthBar.GetComponent<Image>().color = Green;
            }

            // displays cool down
            if (cooldown)
            {
                coolDownTime -= Time.deltaTime;
            }
            if (coolDownTime <= 0)
            {
                cooldown = false;
                usingSpecialAttack = false;
                coolDownTime = playerCoolDownTimer;
                cooldownPercent = 0f;
            }

            // Special Attack loading bar
            if (cooldownPercent <= .99 && cooldown)
            {
                percentSign.gameObject.SetActive(true);
                specialAttackBar.GetComponent<Image>().color = Yellow;
                InnerLoop.GetComponent<Image>().color = Yellow;

                cooldownPercent = (playerCoolDownTimer - coolDownTime) * .1f;
                float percentNumber = cooldownPercent * 100f;
                CooldownPercent.text = ((int)percentNumber).ToString();

                if (PlayerIsCube)
                {
                    specialAttackFullCube.gameObject.SetActive(false);
                }
                else if (PlayerIsOcta)
                {
                    specialAttackFullOcta.gameObject.SetActive(false);
                }
            }
            else
            {
                percentSign.gameObject.SetActive(false);

                if (usingSpecialAttack)
                {
                    CooldownPercent.text = " ";
                }
                else
                {
                    CooldownPercent.text = SpecialAttackButton;
                }
                InnerLoop.GetComponent<Image>().color = Color.white;
                cooldownPercent = 1;
                if (PlayerIsCube)
                {
                    specialAttackFullCube.gameObject.SetActive(true);
                    specialAttackBar.GetComponent<Image>().color = Blue;
                }
                else if (PlayerIsOcta)
                {
                    specialAttackFullOcta.gameObject.SetActive(true);
                    specialAttackBar.GetComponent<Image>().color = Red;
                }
            }

            specialAttackBar.GetComponent<Image>().fillAmount = cooldownPercent;

            //PlayerSpecialTimer.text = coolDownTime.ToString("n2");
            //Debug.Log("PlayersHealth: " + PlayersHealth.ToString());
            //Debug.Log("maxHealth: " + fullHealth.ToString());
            //Debug.Log("hitTimer: " + hitTimer.ToString());

            // if player is not hit for 1 sec, hit UI will turn off
            if (hitTimer >= 0)
            {
                hitTimer -= Time.deltaTime;
            }

            if (hitTimer <= 0)
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

            if (hit && hitCount > hitLowNumber && hitCount < hitHighNumber)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitLow);
            }
            else if (hit && hitCount > hitHighNumber)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitHigh);
            }
            else if (hit && hitCount < hitLowNumber && PlayersHealth < (fullHealth / 4))
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), healthLow);
            }

        }

        // functions called from other classes
        public void EnemiesLeft(int amount)
        {
            numEnemiesAlive = amount;
        }

        public void UsingSpecialAttack()
        {
            usingSpecialAttack = true;
        }

        public void UsedSpecialAttack()
        {
            cooldown = true;
            cooldownPercent = 0f;
            usingSpecialAttack = false;
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

        /// <summary>
        /// set player type, 1 = cube  and 2 = otra
        /// </summary>
        /// <param name="type"></param>
        public void SetPlayerType(int type)
        {
            if (type == 1)
            {
                PlayerIsCube = true;
                PlayerIsOcta = false;
            }

            if (type == 2)
            {
                PlayerIsOcta = true;
                PlayerIsCube = false;
            }
        }

        public void SetSpecialAttackButton(string button)
        {
            SpecialAttackButton = button;
        }
    }
}
