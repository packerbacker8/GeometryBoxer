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
        public Sprite EnemyCubePortrait;
        public Sprite EnemyOctahedronPotrait;


        [Header("Health images")]
        public Material healthLow;
        public Material hitLow;
        public int hitLowNumber = 20;
        public Material hitHigh;
        public int hitHighNumber = 60;

        private GameObject player;
        private GameObject enemies;
        private Camera playerCam;
        private Image img;
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
            //player = GameObject.FindGameObjectWithTag("Player");

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
            img = this.GetComponent<Image>();
            // PlayersHealth = 15000f;
        }

        private void Start()
        {
            this.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            this.GetComponent<Canvas>().worldCamera = playerCam;
            this.GetComponent<Canvas>().sortingLayerName = "UI";
            this.GetComponent<Canvas>().planeDistance = 1;
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

            if (playerCam != null)
            {
                Rect camRect = playerCam.rect;
                if (PlayersHealth < (fullHealth / 4) && !hit)
                {
                    img.color = new Color(1f, 1f, 1f, 1f);
                    img.material = healthLow;
                    //GUI.DrawTexture(camRect, healthLow);
                }

                if (hit && hitCount > hitLowNumber && hitCount < hitHighNumber)
                {
                    img.color = new Color(1f, 1f, 1f, 1f);
                    img.material = hitLow;
                    //GUI.DrawTexture(camRect, hitLow);
                }
                else if (hit && hitCount > hitHighNumber)
                {
                    img.color = new Color(1f, 1f, 1f, 1f);
                    img.material = hitHigh;
                    //GUI.DrawTexture(camRect, hitHigh);
                }
                else if (hit && hitCount < hitLowNumber && PlayersHealth < (fullHealth / 4))
                {
                    img.color = new Color(1f, 1f, 1f, 1f);
                    img.material = healthLow;
                    //GUI.DrawTexture(camRect, healthLow);
                }
            }
            if (!hit)
            {
                img.color = new Color(1f, 1f, 1f, 0f);
                img.material = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void SetPlayer(GameObject p)
        {
            player = p;
            playerCam = player.GetComponentInChildren<Camera>();
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

        public void setHitUIimage(bool h)
        {
            hitCount++;
            hit = h;
            hitTimer = 1f;
        }

        /// <summary>
        /// set player type, 1 = cube  and 2 = otra
        /// </summary>
        /// <param name="type"></param>
        public void SetPlayerType(int type)
        {
            GameObject enemyImg = this.transform.GetChild(3).GetChild(1).gameObject;
            if (type == 1)
            {
                PlayerIsCube = true;
                PlayerIsOcta = false;
                enemyImg.GetComponent<Image>().sprite = EnemyOctahedronPotrait;
            }

            if (type == 2)
            {
                PlayerIsOcta = true;
                PlayerIsCube = false;
                enemyImg.GetComponent<Image>().sprite = EnemyCubePortrait;
            }
        }

        public void SetSpecialAttackButton(string button)
        {
            SpecialAttackButton = button;
        }

        public void reinitializeUI(int enemyCount)
        {
            numEnemiesAlive = enemyCount;
            hitLowNumber = 20;
            hitHighNumber = 60;
            //PlayersHealth = 15000f;
            //Yellow = new Color(255 / 255.0F, 203 / 255.0F, 0, 1);
            //Blue = new Color(115 / 255.0F, 189 / 255.0F, 234 / 255.0F, 1);
            //Red = new Color(255 / 255.0F, 25 / 255.0F, 25 / 255.0F, 1);
            //Green = new Color(0 / 255.0F, 255 / 255.0F, 72 / 255.0F, 1);
        }
    }
}
