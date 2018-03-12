using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerController player;

    public Sprite powerSword;
    public Sprite ultPowerSword;

    private bool runAttack = false;

    private void Awake()
    {
        player = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
        animator = player.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /*
     * UPDATES SWORD ART BASED ON EVENTS
     */
    private void Start()
    {
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_POWER_SWORD))
        {
            spriteRenderer.sprite = powerSword;
        }
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_ULT_POWER_SWORD))
        {
            spriteRenderer.sprite = ultPowerSword;
        }
    }

    /*
     * ATTACHED TO THE SWORD GAME OBJECT
     *   FIRED WITH SPACEBAR AND TRIGGERS ON COLLISION ENTER
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Enemy") && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            collision.SendMessage("TakeDamage", player.GetAttackDamage(), SendMessageOptions.RequireReceiver);
        }
    }

    void Update()
    {
        if (!GameManager.instance.lockedInput && GameManager.instance.eventsManager.CheckEvent("has_sword") && Input.GetButtonDown("Fire1"))
        {
            runAttack = true;
        }
    }

    private void FixedUpdate()
    {
        if (runAttack == true)
        {
            animator.SetTrigger("IsAttacking");
            runAttack = false;
        }
    }
}
