using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernScene : MonoBehaviour
{ 
    public CameraFollow cameraFollow;
    public Character character;
    public GameObject armorOnFloor;

    /*
     * SCENE LOGIC CLASS FOR THE TAVERN-  COULD MOVE THE SCENE SETUP CODE TO HERE
     *   SHOULD INHERIT FROM A MORE GENERAL SCENE CLASS
     */
	void Start ()
    {
        character = GetComponent<Character>();


        if (!GameManager.instance.eventsManager.CheckEvent("game_started"))
        {
            GameManager.instance.player.transform.position = new Vector2(5.27f, 2f);

            GameManager.instance.ToggleLockInput(true);
            cameraFollow.ShakeCamera(.1f, 3);

            GameManager.instance.eventsManager.SaveEvent("game_started");

            foreach (Character character in GameManager.instance.sceneCharacters)
            {
                if(character.name != "Player")
                {
                    character.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            armorOnFloor.SetActive(false);
        }
	}
	
    private void EndShake()
    {
        GameManager.instance.dialogController.MoveDialogBubble(GetComponent<Character>(), GameManager.instance.dialogController.GetDialog("end_shake"));
        GameManager.instance.eventsManager.SaveEvent("end_shake");
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.eventsManager.CheckEvent("run_outside"))
        {
            if (GameManager.instance.eventsManager.CheckEvent("end_shake") && !character.IsSpeaking)
            {
                armorOnFloor.SetActive(false);
                GameManager.instance.eventsManager.TriggerEvent(GameConstants.GOT_ARMOR, true);
                GameManager.instance.eventsManager.TriggerEvent("run_outside", true);

                GameObject door = GameObject.FindObjectOfType<DoorController>().gameObject;
                MoveDirections directions = new MoveDirections(gameObject, door, door, "UseDoor");
                
                GetComponent<Character>().MoveToObject(directions);
                GetComponent<Animator>().SetBool("ForceWalk", true);
            }
        }
    }
}
