using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollider : MonoBehaviour
{

    public bool onPlatform = false;
    public Transform characterContainer;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "MovingPlatform")
        {
            onPlatform = true;

            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (onPlatform)
        {
            onPlatform = false;

            transform.SetParent(characterContainer);
        }
    }
}
