using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour {

    public string sceneName;
    public float spawnPoint;

    private Collider2D collider2D;
    private PlayerController player;
    private bool enteredDoor = false;

    /*
     * USED TO SWITCH SCENES WHEN USING DOORS
     */
    private void OnTriggerStay2D(Collider2D other)
    {
        if(Input.GetAxis("Vertical") > 0 && !enteredDoor)
        {
            enteredDoor = true;
            UseDoor();
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.lockedInput)
            return;

        player.MovingToTarget = true;

        GameManager.instance.ToggleLockInput();
        GameManager.instance.animator.SetBool("IsWalking", true);

        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= (1f + collider2D.bounds.extents.x))
        {
            UseDoor();
            player.SendMessage("FlipCharacterDirection", new Vector2(transform.position.x, transform.position.y));
        }
        else
        {
            MoveDirections directions = new MoveDirections(player.gameObject, gameObject, gameObject, "UseDoor");

            player.SendMessage("MoveToObject", directions, SendMessageOptions.RequireReceiver);
        }
    }
    
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        collider2D = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (player.MovingToTarget && Input.anyKeyDown)
        {
            GameManager.instance.ToggleLockInput(false);

            player.MovingToTarget = false;
            player.SendMessage("CancelMove", SendMessageOptions.RequireReceiver);
        }
    }


    private void UseDoor()
    {
        SceneManager.LoadScene(sceneName);
    }
}
