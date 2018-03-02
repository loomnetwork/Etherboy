using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidBody;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            SendMessage("GotHit", 10, SendMessageOptions.RequireReceiver);
        }
    }


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
}
