using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDialog : MonoBehaviour
{
    public Character character;
    public float triggerRange;
    public bool randomDialog = false;

    public bool firstDialog = true;

    private float currentTime = 0f;
    private float waitTime = 5f;

    /*
     * CALLED FOR RANDOMLY FIRED NPC DIALOG AND CANCLES DIALOG IF PLAYER LEAVES THE GENERAL AREA OF THE NPC
     */
    void Start () {
        character = GetComponent<Character>();
        if(triggerRange == 0)
        {
            triggerRange = 4;
        }
	}
    
    void FixedUpdate()
    {
        if (!GameManager.instance.eventsManager.CheckEvent("game_ended"))
        {
            if (GameManager.instance.dialogController.currentSpeaker == null)
            {
                if (Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x) <= triggerRange)
                {
                    // DECIDE IF THE LAST SPEAKER IS DONE
                    if (!character.IsSpeaking)
                    {
                        if (randomDialog)
                        {
                            if (firstDialog)
                            {
                                SendMessage("RunDialog", "randomDialog", SendMessageOptions.RequireReceiver);

                                firstDialog = false;
                            }
                            else
                            {
                                currentTime += Time.deltaTime;

                                if (currentTime >= waitTime)
                                {
                                    SendMessage("RunDialog", "randomDialog", SendMessageOptions.RequireReceiver);
                                    currentTime = 0;
                                }
                            }
                        }
                    }
                }
            }
            else if (Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x) >= triggerRange && character.IsSpeaking)
            {
                character.IsSpeaking = false;
                GameManager.instance.dialogController.HideDialogBubble(true);

                currentTime = 0;
                firstDialog = true;
            }
        }
    }
}
