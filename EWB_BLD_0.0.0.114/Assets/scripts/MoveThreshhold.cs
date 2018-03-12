using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThreshhold : MonoBehaviour
{
    Character randomCharacter;
    private void Start()
    {
        randomCharacter = GameManager.instance.randomGuy.GetComponent<Character>();
    }

    /*
     * LIVES ON THE RANDOM GUY IN THE TOWN SCENE.  SHOULD LIVE IN A TOWN SCENE CLASS
     *  IDEALLY WOULD BE A HELPER FUNCTION FOR TRIGGERING THRESHOLD EVENTS
     */
    private void FixedUpdate()
    {
        if(GameManager.instance.eventsManager.CheckEvent("has_sword") && !GameManager.instance.eventsManager.CheckEvent("found_you"))
        {
            if (transform.position.x - 1.5f > randomCharacter.transform.position.x)
            {
                GameManager.instance.eventsManager.SaveEvent("found_you");
                GameManager.instance.ToggleLockInput(true);
                GameManager.instance.SetPlayerToIdle();

                if(!randomCharacter)
                    randomCharacter = GameManager.instance.randomGuy.GetComponent<Character>();

                randomCharacter.SendMessage("FlipCharacterDirection", new Vector2(transform.position.x, transform.position.y), SendMessageOptions.RequireReceiver);

                Dialog dialog = GameManager.instance.dialogController.GetDialog("passing_random_guy");
                GameManager.instance.dialogController.currentDialog = dialog;

                GameManager.instance.dialogController.MoveDialogBubble(randomCharacter, dialog);
            }
        }
        else if (GameManager.instance.eventsManager.CheckEvent("found_you") && !GameManager.instance.eventsManager.CheckEvent("the_one"))
        {
            if (!randomCharacter.IsSpeaking)
            {
                GameManager.instance.ToggleLockInput(false);
                GameManager.instance.eventsManager.SaveEvent("the_one");
            }
        }
    }
}
