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

    private void OnTriggerStay2D(Collider2D other)
    {
        if(Input.GetAxis("Vertical") > 0 && !enteredDoor)
        {
         //   enteredDoor = true;
            UseDoor(false);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.lockedInput)
        {
            return;
        }

        GameManager.instance.SendMessageUpwards("ToggleLockInput");

        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= (1f + collider2D.bounds.extents.x))
        {
            UseDoor();
            player.SendMessage("FlipCharacterDirection", new Vector2(transform.position.x, transform.position.y));
        }
        else
        {
            MoveDirections directions = new MoveDirections(player.gameObject, gameObject, gameObject, "UseDoor");

            player.gameObject.SendMessage("MoveToObject", directions, SendMessageOptions.RequireReceiver);
        }
    }

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        collider2D = GetComponent<Collider2D>();
    }


    private void UseDoor(bool lockInput = true)
    {
        if (lockInput)
        {
            GameManager.instance.SendMessage("ToggleLockInput", SendMessageOptions.RequireReceiver);
        }

        SceneManager.LoadScene(sceneName);
    }
}
