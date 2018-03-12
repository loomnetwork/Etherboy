using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public bool lockedInput = false;
    public float health;
    public int coins;
    public PlayerController player;

    public GameObject coinObject;
    public Animator animator;

    public GameObject gameOverScreen;
    public GameObject uploadedScreen;

    public EventsManager eventsManager;
    public DialogController dialogController;

    

    public GameObject godRay;
    public bool godRayActive = false;
    public bool sendPeopleUp = false;

    public GameObject loomToken;
    public GameObject loomToken2;
    public LevelArt levelArt;


    public Collider2D[] platformColliders;


    // NEED TO ADDRESS THIS -have it contained in something better, don't need multiple references
    public GameObject lightLevel;
    public GameObject darkLevel;

    public GameObject lightEnemiesContainer;
    public GameObject darkEnemiesContainer;

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
    public bool resetting = false;
    public bool blackingOut = false;
    public float blackOutTimer = 0f;

    public Image darknessCover;

    public Sprite darkPlatform;

    public Character[] sceneCharacters;
    public SpriteRenderer[] sceneEnemySprites;
    public Collider2D[] sceneEnemyColliders;
    public Enemy[] sceneEnemies;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            health = GameConstants.START_HEALTH;
            coins = GameConstants.START_COINS;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /*
     * SETS UP SCENE BASED ON EVENTS
     */
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ToggleLockInput(false);
        resetting = false;

        // GET ALL CHARACTERS
        sceneCharacters = GameObject.FindObjectsOfType<Character>();

        foreach(Character character in sceneCharacters)
        {
            if(character.name == GameConstants.PLAYER)
            {
                player = character.GetComponent<PlayerController>();
                animator = player.GetComponent<Animator>();
            }
        }


        // GET THE ENEMY SPRITES FOR RELAYING, AND COLLIDERS FOR IGNORING ENEMIES NOT ON YOUR LEVEL
        sceneEnemies = GameObject.FindObjectsOfType<Enemy>();
        sceneEnemySprites = new SpriteRenderer[sceneEnemies.Length];
        sceneEnemyColliders = new Collider2D[sceneEnemies.Length];
        

        for(int i = 0; i < sceneEnemies.Length; i++)
        {
            sceneEnemySprites[i] = sceneEnemies[i].GetComponent<SpriteRenderer>();
            sceneEnemyColliders[i] = sceneEnemies[i].GetComponent<Collider2D>();
        }



        // GET THE DIALOG CONTROLLER
        if (scene.name == GameConstants.PROTOTYPE || scene.name == GameConstants.TAVERN)
        {
            GetReferences();

            if (scene.name == GameConstants.PROTOTYPE)
            {
                dialogController.transform.localScale = new Vector3(2, 2, 2);
                eventsManager.SaveEvent("got_outside");
            }
            else
            {
                dialogController.transform.localScale = new Vector3(1, 1, 1);
                eventsManager.RemoveEvent(GameConstants.RELOADED);
            }
        }

        // GET PLATFORM COLLIDERS
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        GameObject[] movingPlatforms = GameObject.FindGameObjectsWithTag("MovingPlatform");

        platformColliders = new Collider2D[platforms.Length + movingPlatforms.Length];

        for (int j = 0; j < platforms.Length; j++)
        {
            if (platforms[j].GetComponent<Collider2D>())
            {
                platformColliders[j] = platforms[j].GetComponent<Collider2D>();
            }
        }
        for (int k = 0; k < movingPlatforms.Length; k++)
        {
            if (movingPlatforms[k].GetComponent<Collider2D>())
            {
                platformColliders[k + platforms.Length] = movingPlatforms[k].GetComponent<Collider2D>();
            }
        }
    }


    /*
     * STORES REFERENCES TO THE SCENE OBJECTS
     */
    public void GetReferences()
    {
        levelArt = FindObjectOfType<LevelArt>();

        if (SceneManager.GetActiveScene().name == GameConstants.PROTOTYPE)
        {
            lightLevel = levelArt.lightLevel;
            darkLevel = levelArt.darkLevel;
            godRay = levelArt.godRay;

            CollidersContainer collidersContainer = FindObjectOfType<CollidersContainer>();

            lightEnemiesContainer = collidersContainer.lightCreatures;
            darkEnemiesContainer = collidersContainer.darkCreatures;

            warrior = levelArt.warrior;
            girl = levelArt.girl;
            girl2 = levelArt.girl2;
            mentor = levelArt.mentor;
            shopKeeper = levelArt.shopKeeper;
            randomGuy = levelArt.randomGuy;

            loomToken = levelArt.loomToken;
            loomToken2 = levelArt.loomToken2;

            gameOverScreen = levelArt.gameOverScreen;
            uploadedScreen = levelArt.uploadedScreen;
            darknessCover = levelArt.darknessCover.GetComponentInChildren<Image>();

            player.GetComponent<MovingPlatformCollider>().SetCharacterContainer(collidersContainer.transform);

            if (eventsManager.CheckEvent(GameConstants.GOT_LOOM_TOKEN))
            {
                loomToken.SetActive(false);
                player.talisman.SetActive(false);
                SwitchToDarkLevel();

                if (eventsManager.CheckEvent(GameConstants.RELOADED))
                {
                    player.transform.position = new Vector3(180, 4.3f, 1);

                    Camera camera = FindObjectOfType<Camera>();
                    camera.transform.position = new Vector3(180.1071f, 8.962399f, -3);
                }
            }
            else
            {
                levelArt.darkLevel.SetActive(false);
                darkEnemiesContainer.SetActive(false);
            }

            MentorController mentorController = mentor.GetComponent<MentorController>();

            if (eventsManager.CheckEvent("show_talisman") && !eventsManager.CheckEvent("has_talisman"))
            {
                mentorController.EquipTalisman();
            }
            if (eventsManager.CheckEvent("show_sword") && !eventsManager.CheckEvent("has_sword"))
            {
                mentorController.EquipSword();
            }
        }

        dialogController.SetSceneDialog(levelArt.sceneDialogs);

        // EQUIP HELMET
        if (eventsManager.CheckEvent("run_outside"))
        {
            player.helmet.sprite = levelArt.armor;

            if (eventsManager.CheckEvent(GameConstants.GOT_POWER_ARMOR))
            {
                player.helmet.sprite = levelArt.powerArmor;
            }

            if (eventsManager.CheckEvent(GameConstants.GOT_ULT_POWER_ARMOR))
            {
                player.helmet.sprite = levelArt.ultPowerArmor;
            }
        }
    }

    public void EquipArmor()
    {
        if (eventsManager.CheckEvent(GameConstants.GOT_ARMOR))
        {
            player.helmet.sprite = levelArt.armor;
        }
        if (eventsManager.CheckEvent(GameConstants.GOT_POWER_ARMOR))
        {
            player.helmet.sprite = levelArt.powerArmor;
        }
        if (eventsManager.CheckEvent(GameConstants.GOT_ULT_POWER_ARMOR))
        {
            player.helmet.sprite = levelArt.ultPowerArmor;
        }
    }




    /*
     * HP / COINS HUD
     */
    void OnGUI()
    {
        if (SceneManager.GetActiveScene().name == GameConstants.PROTOTYPE || SceneManager.GetActiveScene().name == GameConstants.TAVERN)
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

    /*
     * STOPS PLAYER WALK ANIMATIONS
     */
    public void SetPlayerToIdle()
    {
        animator.SetBool(GameConstants.IS_WALKING, false);
        animator.SetBool(GameConstants.FORCE_WALK, false);
    }
    

    /*
     * SWAPS LAYERS OF ENEMIES AND TOGGLES COLLISIONS BASED ON PLAYER vs ENEMY HEIGHT
     */
    public void RelayerEnemies()
    {        
        string sortingLayer;
        SpriteRenderer enemySprite;
        Collider2D enemyCollider;
        Enemy enemy;

        for(int i = 0; i < sceneEnemySprites.Length; i ++)
        {
            enemy = sceneEnemies[i];
            enemySprite = sceneEnemySprites[i];
            enemyCollider = sceneEnemyColliders[i];

            if (enemyCollider != null && enemyCollider.transform.parent.gameObject.activeSelf)
            {
                if (player.platform && player.platform.name == enemy.platform.name)
                {
                    sortingLayer = "NPC";
                    
                    Physics2D.IgnoreCollision(player.footCollider, enemyCollider, false);
                }
                else
                {
                    sortingLayer = "foregroundNPC";
                    Physics2D.IgnoreCollision(player.footCollider, enemyCollider, true);
                }

                enemySprite.sortingLayerName = sortingLayer;
            }
        }
    }



    /*
     * EVENTS TO SET ANIMATIONS / INSTANTIATE ITEMS
     */
    public void HurtPlayer(int damage)
    {
        health -= damage;
    }

    public void SpawnCoin(Transform transform)
    {
        GameObject instance = Instantiate(coinObject, transform.position, transform.rotation);
    }

    public void AddMoney()
    {
        coins += 1;
    }

    public int GetMoney()
    {
        return coins;
    }

    /*
     * CONTROL PLAYER INPUT/LOCK
     */
    public void ToggleLockInput()
    { 
        lockedInput = !lockedInput;
    }
    public void ToggleLockInput(bool lockState)
    {
        lockedInput = lockState;
    }
    

    public void Update()
    {
        if (!resetting)
        {
            if (!eventsManager.CheckEvent("game_ended") && animator != null)
            {
                if (health <= 0 && !eventsManager.CheckEvent("dead"))
                {
                    animator.SetTrigger(GameConstants.IS_DEAD);
                    ToggleLockInput();
                    eventsManager.SaveEvent("dead");

                    player.GetComponent<BoxCollider2D>().enabled = false;
                    player.GetComponent<EnemyCollider>().enabled = false;
                    player.GetComponent<PlayerController>().enabled = false;
                }


                if (eventsManager.CheckEvent("dead"))
                {
                    deadTime += Time.deltaTime;

                    if (deadTime >= 2f)
                    {
                        resetting = true;
                        GameOver();
                    }
                }
            }
        }
    }

    /*
     * ALMOST ALL OF THIS SHOULD BE MOVED TO THE TOWN SCENE CLASS- HACKY WAY OF RUNNING SCENE LOGIC
     */
    public void FixedUpdate()
    {
        if (blackingOut)
        {
            blackOutTimer += Time.deltaTime;
            if (blackOutTimer < 1f)
            {
                darknessCover.color = new Color(0.0f, 0.0f, 0.0f, Mathf.Lerp(darknessCover.color.a, 235 / 255f, Time.deltaTime * 1.1f));
            }
            else if (blackOutTimer < 1.5f)
            {
                SwitchToDarkLevel();
            }
            else if (blackOutTimer < 3f)
            {
                darknessCover.color = new Color(0.0f, 0.0f, 0.0f, Mathf.Lerp(darknessCover.color.a, 0f, Time.deltaTime * 1.1f));
            }
            else
            {
                ToggleLockInput(false);
                blackingOut = false;

                levelArt.darknessCover.SetActive(false);
            }
        }

        if (sendPeopleUp)
        {
            if (!warriorUp)
            {
                if (warrior.transform.position.x > 41.51017f) 
                {
                    warrior.transform.position = new Vector2(warrior.transform.position.x - Time.deltaTime * 10, warrior.transform.position.y);
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
                    girl.transform.position = new Vector2(girl.transform.position.x - Time.deltaTime * 7.5f, girl.transform.position.y);
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
                    girl2.transform.position = new Vector2(girl2.transform.position.x - Time.deltaTime * 7, girl2.transform.position.y);
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
                    randomGuy.transform.position = new Vector2(randomGuy.transform.position.x - Time.deltaTime * 8, randomGuy.transform.position.y);
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
                    shopKeeper.transform.position = new Vector2(shopKeeper.transform.position.x + Time.deltaTime * 8, shopKeeper.transform.position.y);
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
                    mentor.transform.position = new Vector2(mentor.transform.position.x - Time.deltaTime * 8, mentor.transform.position.y);
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
                if (player.transform.position.x < 41.51017f)
                {
                    player.transform.position = new Vector2(player.transform.position.x + Time.deltaTime * 8, player.transform.position.y);
                    animator.SetBool(GameConstants.FORCE_WALK, true);
                }
                else if (player.transform.position.y < 20.50746)
                {
                    player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + Time.deltaTime * 12);
                    animator.SetBool(GameConstants.FORCE_WALK, false);

                    CameraFollow camera = FindObjectOfType<CameraFollow>();
                    camera.ToggleOff();
                }
                else
                {
                    playerUp = true;
                    Uploaded();
                }
            }
        }
        else if (godRayActive)
        {
            lockedInput = true;

            if (player.transform.position.x < 39f)
            {
                player.transform.position = new Vector2(player.transform.position.x + Time.deltaTime * 6, player.transform.position.y);
                animator.SetBool(GameConstants.FORCE_WALK, true);
                player.GetComponent<Character>().FlipCharacterDirection(new Vector2(player.transform.position.x + 10f, player.transform.position.y));
            }
            else if(player.transform.position.x > 40f)
            {
                player.transform.position = new Vector2(player.transform.position.x - Time.deltaTime * 6, player.transform.position.y);
                animator.SetBool(GameConstants.FORCE_WALK, true);
                player.GetComponent<Character>().FlipCharacterDirection(new Vector2(player.transform.position.x - 10f, player.transform.position.y));
            }
            else
            {
                animator.SetBool(GameConstants.FORCE_WALK, false);
                if (!player.GetComponent<Character>().facingRight)
                {
                    player.GetComponent<Character>().FlipCharacterDirection(new Vector2(player.transform.position.x + 10f, player.transform.position.y));
                }
                if(!loomToken2.activeSelf)
                    loomToken2.SetActive(true);


                mentor.GetComponent<Character>().FlipCharacterDirection(new Vector2(mentor.transform.position.x - 10f, mentor.transform.position.y));

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
                    spriteRenderer.sortingLayerName = "NPC";

                    if (!toggles)
                    {
                        player.GetComponent<Rigidbody2D>().gravityScale = 0;
                        player.GetComponent<Rigidbody2D>().isKinematic = true;
                        shopKeeper.transform.localScale = new Vector2(-1f, 1f);

                        randomGuy.transform.position = new Vector2(55.84f, randomGuy.transform.position.y);
                        randomGuy.GetComponent<Character>().FlipCharacterDirection(new Vector2(randomGuy.transform.position.x - 10f, randomGuy.transform.position.y));
                        randomGuy.GetComponent<ProximityDialog>().enabled = false;
                        randomGuy.GetComponent<NpcController>().enabled = false;

                        
                        toggles = true;

                        warrior.SetActive(true);
                        girl.SetActive(true);
                        girl2.SetActive(true);
                    }
                }
            }
        }
    }

   
    public void GetToken()
    {
        loomToken.SetActive(false);
        player.talisman.SetActive(false);
        eventsManager.SaveEvent(GameConstants.GOT_LOOM_TOKEN);

        ToggleLockInput(true);

        blackingOut = true;
        levelArt.darknessCover.SetActive(true);
    }


    public void GodRayAppears()
    {
        godRayActive = true;
    }
    
    public void SwitchToDarkLevel()
    {
        levelArt.lightLevel.SetActive(false);
        lightEnemiesContainer.SetActive(false);

        SwapPlatformArt();

        levelArt.darkLevel.SetActive(true);
        darkEnemiesContainer.SetActive(true);
    }

    public void SwapPlatformArt()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        SpriteRenderer renderer;
        foreach(GameObject platform in platforms)
        {
            renderer = platform.GetComponent<SpriteRenderer>();

            renderer.sprite = darkPlatform;
        }
    }
    

    /*
     * SHOULD BE MOVED TO A SCENEMANAGER CLASS
     */
    public void GameOver()
    {
        eventsManager.SaveEvent("game_ended");
        dialogController.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        player.enabled = false;
    }


    public void Uploaded()
    {
        dialogController.gameObject.SetActive(false);
        uploadedScreen.SetActive(true);
    }

    // NEED TO UPDATE THIS TO KEEP TRACK OF MONEY AND EVENTS
    public void LoadNewGame()
    {
        health = GameConstants.START_HEALTH;

        ToggleLockInput(false);
        dialogController.gameObject.SetActive(true);
        eventsManager.SaveEvent(GameConstants.RELOADED);

        if (eventsManager.CheckEvent(GameConstants.GOT_LOOM_TOKEN))
        {
            eventsManager.ClearAfterEvent(GameConstants.GOT_LOOM_TOKEN);
            SceneManager.LoadScene(GameConstants.PROTOTYPE);
        }
        else if (eventsManager.CheckEvent(GameConstants.HAS_SWORD))
        {
            eventsManager.ClearAfterEvent(GameConstants.HAS_SWORD);
            SceneManager.LoadScene(GameConstants.PROTOTYPE);
        }
        else
        {
            eventsManager.events = new List<string>();
            SceneManager.LoadScene(GameConstants.TAVERN);
            coins = GameConstants.START_COINS;
        }
    }
}
