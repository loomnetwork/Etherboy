using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float moveSpeed = 3f;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    public bool isTouching = true;
    public bool jumping = false;
    private float jumpVelocity = 5f;

    private bool movingLeft;
    private bool attacking = false;
    public int health = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (transform.tag == "Enemy" && collision.name == "Player")
        {
            GameManager.instance.TakeDamage(10);
        }
        
    }

    public void TakeDamage(int damageValue)
    {
        health -= damageValue;

        if(health <= 0)
        {
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

            else
            {
                if (!jumping)
                {
                    jumping = true;
                }
            }
        }
    }
}
