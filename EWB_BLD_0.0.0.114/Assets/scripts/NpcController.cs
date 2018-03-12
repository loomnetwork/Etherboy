using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour {

    public float moveSpeed = 3f;
    private Collider2D collider2D;
    private Character character;
    
    public Vector2 minPosition;
    public Vector2 maxPosition;
    private bool movingLeft = false;

    /*
     * USED TO TRIGGER NPC INTERACTIONS- MOVING TOWARDS THEM AND RUNNING DIALOG OR ANY OTHER CALLBACK FUNCTION PASSED IN THE MOVEDIRECTIONS OBJECT
     */
    private void OnMouseDown()
    {
        if (!GameManager.instance.lockedInput && !character.IsSpeaking)
        {
            GameManager.instance.player.MovingToTarget = true;
            GameManager.instance.ToggleLockInput(true);

            if (Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x) <= (1f + collider2D.bounds.extents.x))
            {
                SendMessage("FlipCharacterDirection", new Vector2(GameManager.instance.player.transform.position.x, GameManager.instance.player.transform.position.y));
                GameManager.instance.player.SendMessage("FlipCharacterDirection", new Vector2(transform.position.x, transform.position.y));
                
                character.RunDialog();
            }
            else
            {
                MoveDirections directions = new MoveDirections(GameManager.instance.player.gameObject, gameObject, gameObject, "RunDialog");
                GameManager.instance.player.SendMessage("MoveToObject", directions, SendMessageOptions.RequireReceiver);
            }
            Debug.Log("clicked on the npc");
        }
        else
        {
            // NEED NEW DIRECTIONS
            if(Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x) <= (1f + collider2D.bounds.extents.x))
            {
                SendMessage("FlipCharacterDirection", new Vector2(GameManager.instance.player.transform.position.x, GameManager.instance.player.transform.position.y));
                GameManager.instance.player.SendMessage("FlipCharacterDirection", new Vector2(transform.position.x, transform.position.y));
            }
            else
            {
                MoveDirections directions = new MoveDirections(GameManager.instance.player.gameObject, gameObject, gameObject, "RunDialog");
                GameManager.instance.player.SendMessage("MoveToObject", directions, SendMessageOptions.RequireReceiver);
            }
            Debug.Log("clicked on an NPC while he was talking and/or the game was locked");
        }
    }

    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        character = GetComponent<Character>();
    }

    public void FixedUpdate()
    {
        if (!character.IsSpeaking)
        {
            if (maxPosition != new Vector2(0, 0))
            {
                var deltaX = moveSpeed * Time.deltaTime;
                deltaX = movingLeft ? -deltaX : deltaX;

                if (movingLeft)
                {
                    movingLeft = transform.position.x >= minPosition.x;
                    if (character.facingRight)
                    {
                        SendMessage("FlipCharacterDirection", minPosition, SendMessageOptions.RequireReceiver);
                    }

                    if (!movingLeft)
                    {
                        SendMessage("FlipCharacterDirection", maxPosition, SendMessageOptions.RequireReceiver);
                    }
                }
                else
                {
                    movingLeft = transform.position.x >= maxPosition.x;
                    if (!character.facingRight)
                    {
                        SendMessage("FlipCharacterDirection", maxPosition, SendMessageOptions.RequireReceiver);
                    }


                    if (movingLeft)
                    {
                        SendMessage("FlipCharacterDirection", minPosition, SendMessageOptions.RequireReceiver);
                    }
                }

                transform.Translate(deltaX, 0, 0);
            }
        }
    }
}
