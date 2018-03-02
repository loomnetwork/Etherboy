using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    private float speed;
    private Vector2 target = Vector2.zero;
    public bool facingRight = true;
    public bool facesPlayer = true;

    Transform targetTransform;
    Transform originTransform;
    private MoveDirections moveDirections;

    public DialogController dialogController;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>() && Input.GetButtonDown("Fire2"))
        {
            GameManager.instance.lockedInput = true;
            Character character = collision.GetComponent<Character>();

            character.FlipCharacterDirection(transform.position);
            character.RunDialog();
        }
    }

    // NPC to move to
    private void MoveToObject(MoveDirections directions)
    {
        targetTransform = directions.targetObject.transform;
        originTransform = directions.originObject.transform;
        moveDirections = directions;

        FlipCharacterDirection(new Vector2(targetTransform.position.x, targetTransform.position.y));

        Collider2D targetCollider2D = targetTransform.GetComponent<Collider2D>();
        Collider2D collider2D = transform.GetComponent<Collider2D>();

        float offset = targetTransform.GetComponent<Character>() ? targetCollider2D.bounds.extents.x + collider2D.bounds.extents.x : 0f;

        Debug.Log("TargetTransform: " + targetTransform);
        if (targetTransform.position.x < transform.position.x)
        {
            target = new Vector2(targetTransform.position.x + offset, transform.position.y);
        }
        else
        {
            target = new Vector2(targetTransform.position.x - offset, transform.position.y);
        }
    }

    void Start()
    {
        if (GetComponent<PlayerController>())
        { 
            speed = GetComponent<PlayerController>().moveSpeed;
        }
        else if(GetComponent<NpcController>())
        {
            speed = GetComponent<NpcController>().moveSpeed;
        }
        else if(GetComponent<Enemy>())
        {
            speed = GetComponent<Enemy>().moveSpeed;
        }

        dialogController = GameObject.Find("DialogContainer").GetComponent<DialogController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != Vector2.zero)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if(Mathf.Abs(transform.position.x - target.x) <= .5f)
            {
                Debug.Log("Reached character : " + moveDirections.callbackObject);
                target = Vector2.zero;

                if (targetTransform.GetComponent<Character>() != null)
                {
                    targetTransform.SendMessage("FlipCharacterDirection", new Vector2(originTransform.position.x, originTransform.position.y));
                }

                if (moveDirections.callbackObject != null && moveDirections.callbackFunction != null)
                {
                    moveDirections.callbackObject.SendMessage(moveDirections.callbackFunction, true);
                    Debug.Log("Run the callback function");
                }
            }
        }
    }


    public void FlipCharacterDirection(Vector2 target)
    {
        if (facesPlayer && (facingRight && transform.position.x > target.x || !facingRight && transform.position.x < target.x))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }
    }

    private void RunDialog()
    {
        GameManager.instance.SetPlayerToIdle();
        dialogController.MoveDialogBubble(gameObject);
    }
}
