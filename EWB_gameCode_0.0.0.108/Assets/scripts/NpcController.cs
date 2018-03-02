using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour {

    public float moveSpeed = 3f;
    private Collider2D collider2D;
    private PlayerController player;

    private void OnMouseDown()
    {
        if (!GameManager.instance.lockedInput)
        {
            GameManager.instance.SendMessageUpwards("ToggleLockInput");

            if (Mathf.Abs(player.transform.position.x - transform.position.x) <= (1f + collider2D.bounds.extents.x))
            {
                SendMessage("FlipCharacterDirection", new Vector2(player.transform.position.x, player.transform.position.y));
                player.SendMessage("FlipCharacterDirection", new Vector2(transform.position.x, transform.position.y));

                SendMessage("RunDialog");
                //  TurnToPlayer();
            }
            else
            {
                MoveDirections directions = new MoveDirections(player.gameObject, gameObject, gameObject, "RunDialog");

                player.gameObject.SendMessage("MoveToObject", directions, SendMessageOptions.RequireReceiver);
            }
            Debug.Log("clicked on the npc");
        }
    }

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        collider2D = GetComponent<Collider2D>();
    }
}
