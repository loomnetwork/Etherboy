using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    [Range(1, 10)]
    public float climbVelocity = 5f;
    
    private bool climbRequest = false;
    private Rigidbody2D rigidBody;


    private bool canClimb = false;

    [System.NonSerialized]
    PlayerController playerController;

    // Use this for initialization
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        rigidBody = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Climb")
        {
            print("Climb TriggerEntered");
            canClimb = true;
        }
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Climb")
        {
            print("Climb TriggerEntered");
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Climb")
        {
            print("TriggerLeft");
            canClimb = false;

            if (playerController != null)
            {
                if (playerController.GetState() == GameConstants.CLIMBING)
                {
                    SendMessage("SetState", GameConstants.FALLING, SendMessageOptions.RequireReceiver);
                }
            }
        }
    }




    private void Update()
    {
        if (Input.GetAxis("Vertical") != 0 && canClimb && playerController.GetState() != GameConstants.CLIMBING)
        {
            climbRequest = true;
        }
    }

    private void FixedUpdate()
    {
        if (climbRequest)
        {
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector3.zero;
            //transform.parent.SendMessage("SetState", GameConstants.CLIMBING, SendMessageOptions.RequireReceiver);
            SendMessage("SetState", GameConstants.CLIMBING, SendMessageOptions.RequireReceiver);

            climbRequest = false;
        }
        else if (playerController.GetState() == GameConstants.CLIMBING)
        {
            var y = Input.GetAxis("Vertical") * Time.deltaTime * 8.0f;
            transform.Translate(0, y, 0);
        }
    }
}

