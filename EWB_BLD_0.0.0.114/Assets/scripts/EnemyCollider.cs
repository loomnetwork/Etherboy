using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private PlayerController player;
    
    /*
     * PLAYER CLASS TO TELL IF HE COLLIDED WITH AN ENEMY
     *  TRACKS THEIR CURRENT PLATFORM IN THE PLAYERCONTROLLER CLASS AND ENEMIES PLATFORM IS CURRENTLY SET IN INSPECTOR
     *  WON'T FIRE UNLESS PLAYER IS ON THE SAME PLATFORM AS ENEMIES
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (collision.transform.tag == "Enemy" && (player.platform == null || enemy.platform.name == player.platform.name))
        {
            collision.SendMessage("Attacking", SendMessageOptions.RequireReceiver);

            player.TakeDamage(collision.gameObject);
        }
    }


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerController>();
    }
}
