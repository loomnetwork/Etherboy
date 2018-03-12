using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeController : MonoBehaviour {

    PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    /*
     * CONTROLS WHEN PLAYER HITS SLOPE-  ORIGINALLY HAD IT CHANGE THE NORMAL OF THE PLAYER, BUT WE WANT HIM TO STAND UPRIGHT AND NOT AT AN ANGLE SO THAT WAS REMOVED
     *  SINCE HE BOUNCES INTO FALL STATE WHEN HE MOVES THERE IS SOME HACKY LOGIC TO MAINTAIN/RESET HIS WALK STATE AS OFTEN AS POSSIBLE
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.Contains("slope") && player.GetState() == GameConstants.FALLING)
        {
            player.State = GameConstants.WALKING;
            player.animator.SetTrigger("HitGround");
            player.onSlope = true;
            player.platform = collision.gameObject;

            GameManager.instance.RelayerEnemies();
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.name.Contains("slope") && player.GetState() == GameConstants.FALLING)
        {
            player.State = GameConstants.WALKING;
            player.animator.SetTrigger("HitGround");
            player.onSlope = true;
            player.platform = collision.gameObject;

            GameManager.instance.RelayerEnemies();
        }
    }

    // LEFT A SLOPE
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (player.onSlope)
        {
            player.onSlope = false;
            player.platform = null;
        }
    }
}
