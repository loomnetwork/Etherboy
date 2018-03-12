using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCollider : MonoBehaviour {
    
    /*
     * IF PLAYER COLLIDES WITH A COLLECTABLE OBJECT
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GameConstants.COLLECTABLE)
        {
            if (collision.name.Contains("loom"))
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

            else if (collision.name.Contains("coin"))
            {
                DestroyObject(collision.gameObject);
                GameManager.instance.AddMoney();
            }
        }
    }
}
