using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

    private Animator animator;
    private Animation anime;
    public PlayerController player;

    private bool checkTarget = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        animator = player.GetComponent<Animator>();
        anime = animator.GetComponent<Animation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Enemy") && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            collision.SendMessage("TakeDamage", player.GetAttackDamage(), SendMessageOptions.RequireReceiver);
        }
    }

    void Update()
    {
        if (!GameManager.instance.lockedInput && Input.GetButtonDown("Fire1"))
        {
            checkTarget = true;
        }

    }

    private void FixedUpdate()
    {
        if (checkTarget == true)
        {
            animator.SetTrigger("IsAttacking");
            checkTarget = false;
        }
    }
}
