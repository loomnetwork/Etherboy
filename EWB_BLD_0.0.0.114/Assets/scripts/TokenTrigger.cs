using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenTrigger : MonoBehaviour
{
    /*
     * CURRENTLY NOT IN USE, WAS GOING TO BE A BOX COLLIDER AROUND THE PEDASTAL TO TRIGGER EVENTS IF PLAYER CLICKS ON IT OR HITS "e"/"up arrow"
     */
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>() && Input.GetButtonDown("Fire2"))
        {
            GameManager.instance.SetPlayerToIdle();
            if (GameManager.instance.eventsManager.CheckEvent("has_sword"))
            {
                GameManager.instance.eventsManager.TriggerEvent("pick_up_token", true);
            }

            else
            {
                GameManager.instance.eventsManager.TriggerEvent("no_loom_token", false);
            }
        }
    }


    private void OnMouseDown()
    {
        if(Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x) <= 3f && Mathf.Abs(GameManager.instance.player.transform.position.y - transform.position.y) <= 3f)
        {
            GameManager.instance.SetPlayerToIdle();
            if (GameManager.instance.eventsManager.CheckEvent("has_sword"))
            {
                GameManager.instance.eventsManager.TriggerEvent("pick_up_token", true);
            }

            else
            {
                GameManager.instance.eventsManager.TriggerEvent("no_loom_token", false);
            }
            
        }
    }
}
