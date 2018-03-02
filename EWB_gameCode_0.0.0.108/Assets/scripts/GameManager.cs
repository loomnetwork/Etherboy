using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public bool lockedInput = false;
    public float health = 100f;
    public int coins = 0;
    public PlayerController player;

    public GameObject coinObject;
    public Animator animator;

    public GameObject gameOverScreen;

    private bool dead = false;


    public EventsManager eventsManager;



    public GameObject lightLevel;
    public GameObject darkLevel;

    public GameObject lightEnemiesContainer;
    public GameObject darkEnemiesContainer;

    public GameObject godRay;
    public bool godRayActive = false;
    public bool sendPeopleUp = false;

    public GameObject loomToken2;


    public GameObject warrior;
    public GameObject girl;
    public GameObject girl2;
    public GameObject mentor;
    public GameObject shopKeeper;
    public GameObject randomGuy;

    public bool warriorUp = false;
    public bool girl1Up = false;
    public bool girl2Up = false;
    public bool randomGuyUp = false;
    public bool shopKeeperUp = false;
    public bool mentorUp = false;
    public bool playerUp = false;

    public bool toggles = false;

    public float deadTime = 0f;


    public Sprite darkPlatform;


    public void TakeDamage(int damage)
    {
        player.TakeDamage();
        health -= damage;
    }

    private void OnEnable()
    {
        // With the exception of web!!!!!
    }

    private void OnDisable()
    {

    }
    
    public void SetPlayerToIdle()
    {
        animator.SetBool("IsWalking", false);
    }

    public bool HasEvent(string eventName)
    {
        bool triggered = false;

        foreach(string eventTrigger in eventsManager.events)
        {
            if(eventName == eventTrigger)
            {
                return true;
            }
        }
        return triggered;
    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name == "Prototype" || SceneManager.GetActiveScene().name == "Tavern")
        {
            GUIStyle healthStyle = new GUIStyle();
            healthStyle.fontSize = 20;
            healthStyle.fontStyle = FontStyle.Bold;
            healthStyle.normal.textColor = Color.red;

            GUIStyle coinStyle = new GUIStyle(healthStyle);
            coinStyle.normal.textColor = new Color(255, 140, 0, 1);

            GUI.Box(new Rect(0, 0, 150, 70), "");
            GUI.Label(new Rect(10, 10, 130, 30), "Health: " + health, healthStyle);
            GUI.Label(new Rect(10, 40, 130, 30), "Coins: " + coins, coinStyle);
            GUI.backgroundColor = new Color(0, 0, 0, .5f);
        }
    }

    public void AddMoney()
    {
        coins += 1;
    }

    private void ToggleLockInput()
    {
        lockedInput = !lockedInput;
    }
    
    private void MovePlayerToTargetObject(MoveDirections directions)
    {
        lockedInput = true;
        directions.originObject.SendMessage("MoveToCharacter", directions, SendMessageOptions.RequireReceiver);
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SpawnCoin(Transform transform)
    {
        GameObject instance = Instantiate(coinObject, transform.position, transform.rotation);
    }

    public void TriggerEvent(string eventName)
    {
        eventsManager.TriggerEvent(eventName);
    }

    public void Update()
    {
        if(health <= 0 && !dead)
        {
            animator.SetTrigger("IsDead");
            ToggleLockInput();
            dead = true;

            player.GetComponent<BoxCollider2D>().enabled = false;
            player.GetComponent<EnemyCollider>().enabled = false;
            player.GetComponent<PlayerController>().enabled = false;
        }


        if (dead)
        {
            deadTime += Time.deltaTime;

            if (deadTime >= 2f)
            {
                GameOver();
            }
        }

        if (animator == null)
        {
            if (SceneManager.GetActiveScene().name == "Prototype")
            {
                GetReferences();
            }
            else if(SceneManager.GetActiveScene().name == "Tavern")
            {
                player = FindObjectOfType<PlayerController>();
                animator = player.GetComponent<Animator>();
            }
        }
    }


    public void FixedUpdate()
    {
        if (sendPeopleUp)
        {
            if (!warriorUp)
            {
                if (warrior.transform.position.x > 41.51017f) 
                {
                    warrior.transform.position = new Vector2(warrior.transform.position.x - Time.deltaTime * 6, warrior.transform.position.y);
                }
                else if(warrior.transform.position.y < 20.50746)
                {
                    warrior.transform.position = new Vector2(warrior.transform.position.x, warrior.transform.position.y + Time.deltaTime * 12);
                }
                else
                {
                    warriorUp = true;
                    Destroy(warrior);
                }

            }else if (!girl1Up)
            {
                if (girl.transform.position.x > 41.51017f)
                {
                    girl.transform.position = new Vector2(girl.transform.position.x - Time.deltaTime * 5, girl.transform.position.y);
                }
                else if (girl.transform.position.y < 20.50746)
                {
                    girl.transform.position = new Vector2(girl.transform.position.x, girl.transform.position.y + Time.deltaTime * 12);
                }
                else
                {
                    girl1Up = true;
                    Destroy(girl);
                }
            }
            else if(!girl2Up)
            {
                if (girl2.transform.position.x > 41.51017f)
                {
                    girl2.transform.position = new Vector2(girl2.transform.position.x - Time.deltaTime * 4, girl2.transform.position.y);
                }
                else if (girl2.transform.position.y < 20.50746)
                {
                    girl2.transform.position = new Vector2(girl2.transform.position.x, girl2.transform.position.y + Time.deltaTime * 12);
                }
                else
                {
                    girl2Up = true;
                    Destroy(girl2);
                }
            }
            else if (!randomGuyUp)
            {
                if (randomGuy.transform.position.x > 41.51017f)
                {
                    randomGuy.transform.position = new Vector2(randomGuy.transform.position.x - Time.deltaTime * 6, randomGuy.transform.position.y);
                }
                else if (randomGuy.transform.position.y < 20.50746)
                {
                    randomGuy.transform.position = new Vector2(randomGuy.transform.position.x, randomGuy.transform.position.y + Time.deltaTime * 12);
                }
                else
                {
                    randomGuyUp = true;
                    Destroy(randomGuy);
                }
            }
            else if (!shopKeeperUp)
            {
                if (shopKeeper.transform.position.x < 41.51017f)
                {
                    shopKeeper.transform.position = new Vector2(shopKeeper.transform.position.x + Time.deltaTime * 6, shopKeeper.transform.position.y);
                }
                else if (shopKeeper.transform.position.y < 20.50746)
                {
                    shopKeeper.transform.position = new Vector2(shopKeeper.transform.position.x, shopKeeper.transform.position.y + Time.deltaTime * 12);
                }
                else
                {
                    shopKeeperUp = true;
                    Destroy(shopKeeper);
                }

            }
            else if (!mentorUp)
            {
                if(mentor.transform.position.x > 41.51017f)
                {
                    mentor.transform.position = new Vector2(mentor.transform.position.x - Time.deltaTime * 6, mentor.transform.position.y);
                }
                else if (mentor.transform.position.y < 20.50746)
                {
                    mentor.transform.position = new Vector2(mentor.transform.position.x, mentor.transform.position.y + Time.deltaTime * 12);
                }
                else
                {
                    mentorUp = true;
                    Destroy(mentor);
                }
            }
            else if (!playerUp)
            {
                if (player.transform.position.x > 41.51017f)
                {
                    player.transform.position = new Vector2(player.transform.position.x - Time.deltaTime * 6, player.transform.position.y);
                }
                else if (player.transform.position.y < 20.50746)
                {
                    player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + Time.deltaTime * 12);

                    CameraFollow camera = FindObjectOfType<CameraFollow>();
                    camera.ToggleOff();
                }
                else
                {
                    playerUp = true;
                    GameOver();
                }
            }
        }
        else if (godRayActive)
        {
            lockedInput = true;


            if (player.transform.position.x < 39f)
            {
                player.transform.position = new Vector2(player.transform.position.x + Time.deltaTime * 6, player.transform.position.y);
            }
            else if(player.transform.position.x > 40f)
            {
                player.transform.position = new Vector2(player.transform.position.x - Time.deltaTime * 6, player.transform.position.y);
            }
            else
            {
                if (!player.GetComponent<Character>().facingRight)
                {
                    player.GetComponent<Character>().FlipCharacterDirection(new Vector2(player.transform.position.x + 10f, player.transform.position.y));
                    loomToken2.SetActive(true);
                }


                
                Vector2 scale;
                if (godRay.transform.localScale.y < 1)
                {
                    scale = new Vector2(godRay.transform.localScale.x, godRay.transform.localScale.y + Time.deltaTime);
                    godRay.transform.localScale = scale;
                }
                else if (godRay.transform.localScale.x < 1)
                {
                    scale = new Vector2(godRay.transform.localScale.x + Time.deltaTime, godRay.transform.localScale.y);
                    godRay.transform.localScale = scale;

                }
                else
                {
                    sendPeopleUp = true;
                    SpriteRenderer spriteRenderer = shopKeeper.GetComponent<SpriteRenderer>();
                    spriteRenderer.sortingLayerName = "npc";

                    if (!toggles)
                    {
                        player.GetComponent<Rigidbody2D>().gravityScale = 0;
                        player.GetComponent<Rigidbody2D>().isKinematic = true;
                        shopKeeper.transform.localScale = new Vector2(-1f, 1f);

                        randomGuy.transform.position = new Vector2(52.91f, randomGuy.transform.position.y);

                        toggles = true;

                        warrior.SetActive(true);
                        girl.SetActive(true);
                        girl2.SetActive(true);
                    }
                }
            }

        }
    }

    public void GetReferences()
    {
        player = FindObjectOfType<PlayerController>();
        animator = player.GetComponent<Animator>();


        if (SceneManager.GetActiveScene().name == "Prototype")
        {
            LevelArt levelArt = FindObjectOfType<LevelArt>();

            lightLevel = levelArt.lightLevel;
            darkLevel = levelArt.darkLevel;
            godRay = levelArt.godRay;

            CollidersContainer collidersContainer = FindObjectOfType<CollidersContainer>();

            lightEnemiesContainer = collidersContainer.lightCreatures;
            darkEnemiesContainer = collidersContainer.darkCreatures;

            if (eventsManager.CheckEvent("got_loom_token"))
            {
                SwitchToDarkLevel();
            }

            warrior = levelArt.warrior;
            girl = levelArt.girl;
            girl2 = levelArt.girl2;
            mentor = levelArt.mentor;
            shopKeeper = levelArt.shopKeeper;
            randomGuy = levelArt.randomGuy;

            loomToken2 = levelArt.loomToken;

            gameOverScreen = levelArt.gameOverScreen;
        }
    }

    public void GetToken()
    {
        eventsManager.TriggerEvent("got_loom_token");
        SwitchToDarkLevel();
    }

    public void SwitchToDarkLevel()
    {
        lightLevel.SetActive(false);
        lightEnemiesContainer.SetActive(false);

        swapPlatformArt();

        darkLevel.SetActive(true);
        darkEnemiesContainer.SetActive(true);
    }


    public void swapPlatformArt()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        SpriteRenderer renderer;
        foreach(GameObject platform in platforms)
        {
            renderer = platform.GetComponent<SpriteRenderer>();

            renderer.sprite = darkPlatform;
        }
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void BlackOut()
    {
        godRayActive = true;
    }

    public void GodRayAppears()
    {
        godRayActive = true;
    }
}
