using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallController : MonoBehaviour {
    
    public float groundedSkin = 1.1f;
    public LayerMask mask;
    
    private Rigidbody2D rigidBody;
    [System.NonSerialized]
    private PlayerController playerController;
    private Animator animator;

    public GameObject Torso;
    public GameObject climbing;

    
    private void Awake ()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (playerController.GetState() == GameConstants.WALKING)
        {
            SendMessage("SetState", GameConstants.FALLING, SendMessageOptions.RequireReceiver);
        }
    }


    // Update is called once per frame
    private void FixedUpdate ()
    {
        if (playerController.GetState() == GameConstants.FALLING)
        {
            Vector2 rayStart = (Vector2)transform.position + (Vector2.down * playerController.GetFeetBounding().extents.y);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, groundedSkin, mask);
            
            if (hit)
            {
                animator.SetTrigger("HitGround");
                
                SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);

                if (!hit.collider.tag.Contains("MovingPlatform"))
                {
                    transform.position = new Vector3(transform.position.x, hit.transform.position.y + hit.collider.bounds.extents.y + playerController.GetPlayerBounding().extents.y, transform.position.z);
                }

                rigidBody.velocity = Vector3.zero;
                rigidBody.gravityScale = 1;
            }
        }
    }
}
