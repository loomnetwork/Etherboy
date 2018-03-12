using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    private bool speaking = false;
    private float speed;
    public Vector2 target = Vector2.zero;
    public bool facingRight = true;
    public bool facesPlayer = true;

    public Transform targetTransform;
    public Transform originTransform;
    private MoveDirections moveDirections;

    private Animator animator;
    
    public bool IsSpeaking
    {
        get
        {
            return speaking;
        }
        set
        {
            speaking = value;
        }
    }

    /*
     * USED TO CALL DIALOG IF STANDING OVER THE NPC COLLIDER.  NEEDS TO FIND THE MOST RECENT EVENT OR IT FIRES THE FIRST EMPTY trigger DIALOG
     */ 
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>() && Input.GetButtonDown("Fire2"))
        {
            Character character = collision.GetComponent<Character>();

            character.FlipCharacterDirection(transform.position);
            character.RunDialog();
        }
    }

    // CHARACTER MOVEMENT FUNCTION
    public void MoveToObject(MoveDirections directions)
    {
        targetTransform = directions.targetObject.transform;
        originTransform = directions.originObject.transform;
        moveDirections = directions;

        if (animator)
        {
            animator.SetBool(GameConstants.FORCE_WALK, true);
            animator.SetBool(GameConstants.IS_WALKING, false);
        }

        FlipCharacterDirection(new Vector2(targetTransform.position.x, targetTransform.position.y));

        Collider2D targetCollider2D = targetTransform.GetComponent<Collider2D>();
        Collider2D collider2D = transform.GetComponent<Collider2D>();

        float offset = targetTransform.GetComponent<Character>() ? targetCollider2D.bounds.extents.x + collider2D.bounds.extents.x : 0f;
        
        if (targetTransform.position.x < transform.position.x)
        {
            target = new Vector2(targetTransform.position.x + offset, transform.position.y);
        }
        else
        {
            target = new Vector2(targetTransform.position.x - offset, transform.position.y);
        }
    }

    /*
     * CALLED WHEN PLAYER CANCELS MOVEMENT
     */ 
    public void CancelMove()
    {
        targetTransform = null;
        originTransform = null;
        moveDirections = null;

        if (animator)
        {
            animator.SetBool(GameConstants.FORCE_WALK, false);
        }


        target = Vector2.zero;
    }


    void Start()
    {
        if (GetComponent<PlayerController>())
        { 
            speed = GetComponent<PlayerController>().moveSpeed;
            animator = GetComponent<Animator>();
        }
        else if(GetComponent<NpcController>())
        {
            speed = GetComponent<NpcController>().moveSpeed;
        }
        else if(GetComponent<Enemy>())
        {
            speed = GetComponent<Enemy>().moveSpeed;
        }
    }
    
    void FixedUpdate()
    {
        if (target != Vector2.zero)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            
            if(Mathf.Abs(transform.position.x - target.x) <= .5f)
            {
                FlipCharacterDirection(target);

                target = Vector2.zero;
                GameManager.instance.ToggleLockInput(false);

                if (animator)
                {
                    animator.SetBool(GameConstants.FORCE_WALK, false);
                }

                if (targetTransform.GetComponent<Character>() != null)
                {
                    targetTransform.SendMessage("FlipCharacterDirection", new Vector2(originTransform.position.x, originTransform.position.y));
                }

                if (moveDirections.callbackObject != null && moveDirections.callbackFunction != null)
                {
                    if (moveDirections.parameters != null)
                    {
                        moveDirections.callbackObject.SendMessage(moveDirections.callbackFunction, moveDirections.parameters);
                    }
                    else
                    {
                        moveDirections.callbackObject.SendMessage(moveDirections.callbackFunction, "", SendMessageOptions.RequireReceiver);
                    }
                }
            }
            else
            {
                // JUST TO MAKE SURE CHARACTER ISN'T RUNNING BACKWARDS EVER
                FlipCharacterDirection(target);

            }
        }
    }

    /*
     * MAINTAINS THEIR DIRECTION BASED ON MOVEMENT
     */ 
    public void FlipCharacterDirection(Vector2 target)
    {
        if (facesPlayer && (facingRight && transform.position.x > target.x || !facingRight && transform.position.x < target.x))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }
    }
    
    /*
     * CALLS DIALOG FROM THE SCENE XML
     */
    public void RunDialog(string dialogName = null)
    {
        foreach(Character character in GameManager.instance.sceneCharacters)
        {
            character.IsSpeaking = false;
        }
        
        IsSpeaking = true;
        Dialog dialog = null;
        
        if (dialogName == "randomDialog")
        {
            dialog = GameManager.instance.dialogController.GetDialog(dialogName);
        }
        else
        {
            //  RUN A SPECIFIC DIALOG HERE

           // Debug.Log("parameters is on override function null");
        }
        
        GameManager.instance.SetPlayerToIdle();
        GameManager.instance.dialogController.MoveDialogBubble(this, dialog);
    }
}
