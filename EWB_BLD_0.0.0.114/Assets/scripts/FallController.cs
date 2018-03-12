using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallController : MonoBehaviour {
    
    public float groundedSkin = 1.1f;
    public LayerMask mask;
    
    private Rigidbody2D rigidBody;
    [System.NonSerialized]
    private PlayerController player;
    private Animator animator;
    private Collider2D collider2D;

    public GameObject Torso;
    public GameObject climbing;

    
    private void Awake ()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerController>();
        collider2D = GetComponent<Collider2D>();
    }

    /*
     * HACKY WAY TO CHECK IF THE PLAYER HAS LANDED ON A PLATFORM
     *  STORES THE PLATFORM IN THE PLAYERCONTROLLER FOR LATER COLLISION CHECKS
     * 
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player.GetState() == GameConstants.FALLING && (collision.collider.tag == "Platform" || collision.collider.tag == "MovingPlatform" || collision.collider.tag == "Ground") && !collision.collider.name.Contains("slope"))
        {
            animator.SetTrigger("HitGround");
            SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);

            player.platform = collision.gameObject;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (player.GetState() == GameConstants.FALLING && (collision.collider.tag == "Platform" || collision.collider.tag == "Ground") && !collision.collider.name.Contains("slope"))
        {
            animator.SetTrigger("HitGround");
            SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);

            player.platform = collision.gameObject;
        }
    }

    // USED IF YOU WALK OFF A PLATFORM 
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (player.GetState() == GameConstants.WALKING)
        {
            SendMessage("SetState", GameConstants.FALLING, SendMessageOptions.RequireReceiver);

            player.platform = null;
        }
    }


    // Update is called once per frame
    private void FixedUpdate ()
    {
        if (player.GetState() == GameConstants.FALLING)
        {
            Vector2 rayStart = new Vector2(transform.position.x, transform.position.y - (collider2D.bounds.size.y * .5f) - (player.GetFeetBounding().extents.y));
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, groundedSkin, mask);
            
            if (hit && !hit.collider.name.Contains("slope"))
            {
                animator.SetTrigger("HitGround");

                player.platform = hit.collider.gameObject;
                SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);

                if (!hit.collider.tag.Contains("MovingPlatform"))
                {
                    transform.position = new Vector3(transform.position.x, hit.transform.position.y + hit.collider.bounds.extents.y + player.GetPlayerBounding().extents.y, transform.position.z);
                }

                rigidBody.velocity = Vector3.zero;
                rigidBody.gravityScale = 1;
            }
        }
    }

    private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;
        return Vector3.Cross(side1, side2).normalized;
    }
}
