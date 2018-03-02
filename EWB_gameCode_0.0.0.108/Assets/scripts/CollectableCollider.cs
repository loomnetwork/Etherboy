using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCollider : MonoBehaviour {

    public bool GotLoomToken = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("loom"))
        {
            if (GameManager.instance.eventsManager.CheckEvent("go_on_quest"))
            {
                GotLoomToken = true;
                GameManager.instance.GetToken();

                DestroyObject(collision.gameObject);
            }
        }

        else if (collision.name.Contains("coin"))
        {
            DestroyObject(collision.gameObject);
            GameManager.instance.AddMoney();
        }
    }
}
