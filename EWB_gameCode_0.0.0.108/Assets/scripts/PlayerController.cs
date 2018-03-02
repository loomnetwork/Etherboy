using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float fallMultiplier = 8f;

    public float moveSpeed = 5f;
    public float fallBuffer = .5f;
    
    private Rigidbody2D rigidBody;
    private Bounds feetBounding;
    private Bounds playerBounding;
    public Collider2D footCollider;
    private Collider2D[] colliders;
    private Animator animator;

    private int swordDamager = 1;

    private bool stopMotion = false;
    private int health = 100;
    private bool takeDamage = false;


    public GameObject torso;
    public GameObject climbing;

    
    public string State;


    public Bounds GetFeetBounding()
    {
        return feetBounding;
    }

    public Bounds GetPlayerBounding()
    {
        return playerBounding;
    }

    public string GetState()
    {
        return State;
    }
    
    private void SetState(string state)
    {
        State = state;
        
        if (State == GameConstants.WALKING)
        {
            EnablePlatforms();
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

    private void GotHit(int damage)
    {
            rigidBody.AddForce(new Vector2(-20f, -10f), ForceMode2D.Impulse);
            animator.SetTrigger("IsHurt");
    }

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
                if (colliders[i].transform.position.y + (colliders[i].bounds.size.y * .5f) < (transform.position.y - feetBounding.extents.y + fallBuffer))
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
        
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        
        colliders = new Collider2D[platforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i].GetComponent<Collider2D>())
            {
                colliders[i] = platforms[i].GetComponent<Collider2D>();
            }
        }
    }

    public void StopMotion(bool _stopMotion = true)
    {
        stopMotion = _stopMotion;
    }
    
    public int GetAttackDamage()
    {
        return swordDamager;
    }

    public void TakeDamage()
    {
        animator.SetTrigger("IsHurt");
    }

    private void FixedUpdate()
    {
        if (stopMotion)
        {
            return;
        }


        if (GameManager.instance.lockedInput)
        {
            return;
        }
        
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

        if (transform.position.y <= -2f)
        {
            GameManager.instance.GameOver();
        }
    }
}