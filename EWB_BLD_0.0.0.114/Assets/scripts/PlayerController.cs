using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float fallMultiplier = 8f;
    public float moveSpeed = 5f;
    public float fallBuffer = .5f;

    public bool onSlope = false;

    private float recoverTimer = .5f;
    private float hurtTimer;


    private Rigidbody2D rigidBody;
    private Bounds playerBounding;

    public Collider2D footCollider;
    public Bounds feetBounding;


    private Collider2D[] colliders;
    public Animator animator;

    private int swordDamager = 1;
    private int health = 100;
    private bool isHurt = false;


    public Character character;
    public GameObject torso;
    public GameObject climbing;

    public GameObject talisman;
    public GameObject sword;
    public SpriteRenderer helmet;

    public GameObject platform;


    private bool movingToTarget = false;
    public string State;


    public bool MovingToTarget
    {
        get
        {
            return movingToTarget;
        }
        set
        {
            movingToTarget = value;
        }
    }

    public void Awake()
    {
        if (SceneManager.GetActiveScene().name == GameConstants.PROTOTYPE)
            sword.SetActive(GameManager.instance.eventsManager.CheckEvent("has_sword"));

        talisman.SetActive(GameManager.instance.eventsManager.CheckEvent("give_talisman"));
    }

    public void EquipTalisman()
    {
        talisman.SetActive(true);
    }

    public void EquipSword()
    {
        AttackController attackController = FindObjectOfType<AttackController>();
        sword.SetActive(true);

        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_POWER_SWORD))
        {
            attackController.spriteRenderer.sprite = attackController.powerSword;
        }
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_ULT_POWER_SWORD))
        {
            attackController.spriteRenderer.sprite = attackController.ultPowerSword;
        }
    }


    /*
     * USED IN COLLISION TESTING
     */
    public Bounds GetFeetBounding()
    {
        return feetBounding;
    }

    public Bounds GetPlayerBounding()
    {
        return playerBounding;
    }

    /*
     * PLAYER STATE CONTROLLER
     */
    public string GetState()
    {
        return State;
    }
    
    private void SetState(string state)
    {
        State = state;
        animator.SetBool("IsClimbing", false);


        if (State == GameConstants.WALKING)
        {
            EnablePlatforms();
            GameManager.instance.RelayerEnemies();
            animator.SetTrigger("HitGround");
        }
        else
        {
            animator.SetBool("IsWalking", false);

            if (State == GameConstants.JUMPING)
            {
                animator.SetTrigger("IsJumping");
                EnablePlatforms(false);
            }
            else if (State == GameConstants.CLIMBING)
            {
                animator.SetBool("IsClimbing", true);
                EnablePlatforms(false);
            }
            else if (State == GameConstants.FALLING)
            {
                animator.SetTrigger("IsFalling");
                rigidBody.gravityScale = fallMultiplier;
                EnablePlatforms(true, true);
            }
        }
    }
    
    /*
     * SHOULD LIVE IN SCENE MANAGER- TOGGLES PLATFORMS THE PLAYER CAN INTERACT WITH
     */
    private void EnablePlatforms(bool enable = true, bool enableSlopes = false)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!enable)
            {
                Physics2D.IgnoreCollision(footCollider, colliders[i]);
            }

            else
            {
                if (colliders[i].transform.position.y + colliders[i].bounds.extents.y  < (transform.position.y - feetBounding.extents.y))
                {
                    Physics2D.IgnoreCollision(footCollider, colliders[i], false);
                }
                else
                {
                    Physics2D.IgnoreCollision(footCollider, colliders[i]);
                }

                if(colliders[i].gameObject.name.Contains("slope"))
                {
                    if (enableSlopes && colliders[i].transform.position.y < transform.position.y - feetBounding.extents.y + fallBuffer)
                    {
                        Physics2D.IgnoreCollision(footCollider, colliders[i], false);
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(footCollider, colliders[i]);
                    }
                }
            }
        }
    }
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        feetBounding = footCollider.bounds;
        playerBounding = GetComponent<Collider2D>().bounds;
        rigidBody = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        GameObject[] movingPlatforms = GameObject.FindGameObjectsWithTag("MovingPlatform");

        colliders = new Collider2D[platforms.Length + movingPlatforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i].GetComponent<Collider2D>())
            {
                colliders[i] = platforms[i].GetComponent<Collider2D>();
            }
        }
        for(int j = 0; j < movingPlatforms.Length; j++)
        {
            if (movingPlatforms[j].GetComponent<Collider2D>())
            {
                colliders[j + platforms.Length] = movingPlatforms[j].GetComponent<Collider2D>();
            }
        }
    }

    /*
     * INTERACTING WITH ENEMIES
     */
    public int GetAttackDamage()
    {
        return swordDamager;
    }

    public void TakeDamage(GameObject enemy)
    {
        isHurt = true;
        hurtTimer = 0;
        
        Vector2 force = enemy.transform.position.x >= transform.position.x ? new Vector2(-5f, 4f) : new Vector2(5f, 4f);

        rigidBody.AddForce(force, ForceMode2D.Impulse);
        animator.SetTrigger("IsHurt");
        GameManager.instance.HurtPlayer(10);
    }
    
    /*
     * MOVEMENT INPUT, ADDRESSES DIRECTION FACING
     */
    private void FixedUpdate()
    {
        if (!GameManager.instance.eventsManager.CheckEvent("game_ended"))
        {
            if (GameManager.instance.lockedInput || character.target != Vector2.zero)
                return;

            if (isHurt)
            {
                hurtTimer += Time.deltaTime;

                if (hurtTimer >= recoverTimer)
                {
                    isHurt = false;
                    hurtTimer = 0;
                }
            }
            else
            {
                if (Input.GetAxis("Horizontal") != 0)
                {
                    animator.SetBool("IsWalking", true);
                    var x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
                    transform.Translate(x, 0, 0);
                }
                else
                {
                    animator.SetBool("IsWalking", false);
                }

                if (Input.GetAxis("Horizontal") != 0)
                {
                    SendMessage("FlipCharacterDirection", new Vector2(transform.position.x + Input.GetAxis("Horizontal"), transform.position.y));
                }

                if (transform.position.y <= -2f && !GameManager.instance.resetting)
                {
                    Destroy(this);

                    GameManager.instance.eventsManager.SaveEvent("dead");
                    GameManager.instance.GameOver();
                }
            }
        }
    }
}