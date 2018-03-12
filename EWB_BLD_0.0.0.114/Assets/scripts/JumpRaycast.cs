using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRaycast : MonoBehaviour {

    [Range(1, 10)]
    public float jumpVelocity = 6f;

    public float jumpGravity = 3f;
    public float lowJumpMultiplier = 10f;
    private bool jumpRequest = false;

    private Rigidbody2D rigidBody;
    private Animator animator;
    [System.NonSerialized]
    private PlayerController player;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        player= GetComponent<PlayerController>();
    }
    
    /*
     * TOGGLES JUMP MECHANIC, RUNS ANIMATION, SETS PLAYER STATE AND SETS HIS PLATFORM TO NULL
     */
    void Update()
    {
        if (GameManager.instance.lockedInput)
        {
            return;
        }

        if (Input.GetButtonDown("Jump") && player.GetState() == GameConstants.WALKING && !jumpRequest)
        {
            jumpRequest = true;
            animator.SetTrigger("JumpPrep");
        }
    }

    void FixedUpdate()
    {
        if (jumpRequest)
        {
                rigidBody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
                SendMessage("SetState", GameConstants.JUMPING, SendMessageOptions.RequireReceiver);
            
                player.platform = null;
                jumpRequest = false;
        }

        else if(player.GetState() == GameConstants.JUMPING)
        {
            if (rigidBody.velocity.y <= 0)
            {
                SendMessage("SetState", GameConstants.FALLING, SendMessageOptions.RequireReceiver);
            }
            else
            {
                rigidBody.gravityScale = jumpGravity;
            }
        }
    }
}
