using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollider : MonoBehaviour
{

    public bool onPlatform = false;
    public Transform characterContainer;
    public Rigidbody2D rigidBody;
    public PlayerController player;

    /*
     * COLLIDER CLASS TO SET PLAYER ANIMATION, STATE, AND PLATFORM WHEN INTERACTING WITH MOVING PLATFORMS
     */
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerController>();
    }

    public void SetCharacterContainer(Transform containerTransform)
    {
        characterContainer = containerTransform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MovingPlatform movingPlatform = collision.transform.GetComponent<MovingPlatform>();

        if (collision.collider.tag == "MovingPlatform" && transform.position.y + player.GetPlayerBounding().extents.y > collision.transform.position.y + collision.collider.bounds.size.y)
        {
            onPlatform = true;
            SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);

            transform.SetParent(collision.transform);

            player.platform = collision.gameObject;
            transform.position = new Vector3(transform.position.x, collision.transform.position.y + collision.collider.bounds.size.y + .1f + player.GetPlayerBounding().extents.y, transform.position.z);
            
            rigidBody.velocity = Vector3.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (onPlatform)
        {
            onPlatform = false;
            player.platform = null;
            transform.SetParent(characterContainer);

            if (player.GetState() != GameConstants.JUMPING)
            {
                SendMessage("SetState", GameConstants.FALLING, SendMessageOptions.RequireReceiver);
            }
        }
    }
}
