using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeController : MonoBehaviour {
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.Contains("slope"))
        {
            SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.name.Contains("slope"))
        {
         //   SendMessage("SetState", GameConstants.WALKING, SendMessageOptions.RequireReceiver);

         //   transform.rotation = Quaternion.identity;
        }
    }
}
