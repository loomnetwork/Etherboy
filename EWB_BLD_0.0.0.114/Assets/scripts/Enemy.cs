using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float moveSpeed = 3f;
    public Vector2 minPosition;
    public Vector2 maxPosition;
    public GameObject platform;
    
    private bool movingLeft;
    private bool attacking = false;
    private bool dead = false;
    public int health = 1;

    public float attackCooldown = 2;
    public float waitTime = 0;
    

    /*
     * ENEMY CLASS - STOPS MOVEMENT WHEN ATTACKING.  IF NOT ATTACKING, MOVES GAME OBJECT BASED ON POSITION VALUES
     *   SHOULD BE SWITCHED TO USE RIGIDBODIES ON THE ENEMIES
     */
    private void Attacking()
    {
        attacking = true;
    }

    public void TakeDamage(int damageValue)
    {
        health -= damageValue;

        if(health <= 0 && !dead)
        {
            dead = true;
            Destroy(gameObject);
            GameManager.instance.SpawnCoin(transform);
        }
    }
    
    private void FixedUpdate()
    {
        if (!attacking)
        {
            if (maxPosition != new Vector2(0, 0))
            {
                var deltaX = moveSpeed * Time.deltaTime;
                deltaX = movingLeft ? -deltaX : deltaX;

                if (movingLeft)
                {
                    movingLeft = transform.position.x >= minPosition.x;
                    if (!movingLeft)
                    {
                        SendMessage("FlipCharacterDirection", maxPosition, SendMessageOptions.RequireReceiver);
                    }
                }
                else
                {
                    movingLeft = transform.position.x >= maxPosition.x;
                    if (movingLeft)
                    {
                        SendMessage("FlipCharacterDirection", minPosition, SendMessageOptions.RequireReceiver);
                    }
                }

                transform.Translate(deltaX, 0, 0);
            }
        }
        else
        {
            waitTime += Time.deltaTime;
            if(waitTime >= attackCooldown)
            {
                waitTime = 0;
                attacking = false;
            }
        }
    }
}
