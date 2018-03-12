using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    public List<string> events;


    /* BETTER WAY TO HANDLE THIS
    public Dictionary<string, UnityEvent> eventDictionary;

    private static EventsManager eventsManager;

    public static EventsManager instance
    {
        get
        {
            if (!eventsManager)
            {
                eventsManager = FindObjectOfType(typeof (EventsManager)) as EventsManager;

                if (!eventsManager)
                {
                    Debug.LogError("There needs to be 1 active Events Manager script on a game object in your scene.");
                }
                else
                {
                    eventsManager.Init(); 
                }
            }

            return eventsManager;
        }
    }

    private void Init()
    {
        if(eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }


    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);

            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventsManager == null)
            return;

        UnityEvent thisEvent = null;
        if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    } */


    /*
     *  EVENT MANAGER HELPER CLASS.  STORES AND MAINTAINS EVENTS BETWEEN SCENES
     */    
    public void SaveEvent(string eventName)
    {
        if (eventName == "" || events.Contains(eventName))
            return;

        events.Add(eventName);
    }


    public void TriggerEvent(string triggerEvent, bool saveEvent)
    { 
        if (triggerEvent == "" || events.Contains(triggerEvent))
        {
            GameManager.instance.ToggleLockInput(false);
            return;
        }

        if (saveEvent){
            events.Add(triggerEvent);
        }

        Dialog dialog = GameManager.instance.dialogController.EventTriggeredDialog(triggerEvent);
        if (dialog != null)
        {
            foreach(Character character in GameManager.instance.sceneCharacters)
            {
                if(character!= null && character.name == dialog.speaker)
                {
                    GameManager.instance.dialogController.MoveDialogBubble(character, dialog);

                    if (dialog.speaker == "Player")
                    {
                        GameManager.instance.ToggleLockInput(true);
                    }
                }
            }
        }
        
        if(triggerEvent == "run_outside")
        {
            GameManager.instance.ToggleLockInput(false);
            GameManager.instance.EquipArmor();
        }
        else if(triggerEvent == "show_talisman")
        {
            GameManager.instance.ToggleLockInput(false);
            GameManager.instance.mentor.GetComponent<MentorController>().EquipTalisman();
        }
        else if(triggerEvent == "has_talisman")
        {
            GameManager.instance.mentor.GetComponent<MentorController>().GiveTalisman();
            GameManager.instance.player.GetComponent<PlayerController>().EquipTalisman();
        }
        else if (triggerEvent == "show_sword")
        {
            GameManager.instance.mentor.GetComponent<MentorController>().EquipSword();
        }
        else if (triggerEvent == "has_sword")
        {
            GameManager.instance.mentor.GetComponent<MentorController>().GiveSword();
            GameManager.instance.player.GetComponent<PlayerController>().EquipSword();
        }
        else if (triggerEvent == "get_loom_token")
        {
            GameManager.instance.GetToken();
        }
        else if(triggerEvent == "god_ray")
        {
            GameManager.instance.GodRayAppears();
        }
        else if(triggerEvent == "open_shop_menu")
        {
            Debug.Log("OPEN SHOP MENU");
            GameManager.instance.levelArt.shopController.gameObject.SetActive(true);
        }
        else if(triggerEvent == "place_token")
        {
            GameManager.instance.GodRayAppears();
        }
        else if(triggerEvent == GameConstants.GOT_POWER_ARMOR || triggerEvent == GameConstants.GOT_ULT_POWER_ARMOR)
        {
            GameManager.instance.EquipArmor();
        }
        else if(triggerEvent == GameConstants.GOT_POWER_SWORD || triggerEvent == GameConstants.GOT_ULT_POWER_SWORD)
        {
            GameManager.instance.player.GetComponent<PlayerController>().EquipSword();
        }
    }



    public bool CheckEvent(string triggerEvent)
    {
        foreach (string eventToCheck in events)
        {
            if (eventToCheck == triggerEvent)
            {
                return true;
            }
        }
        
        return false;
    }


    public void RemoveEvent(string eventName)
    {
        events.Remove(eventName);
    }


    /*
     * GAME RESET CODE- STORES IMPORTANT EVENTS LIKE WEAPONS AND ARMOR, DELETES EVERYTHING AFTER THE CHECKPOINT EVENT AND RESAVES ANY IMPORTANT ONES
     */
    public void ClearAfterEvent(string lastEvent)
    {
        bool foundYou = false;
        bool theOne = false;
        bool gotPowerSword = false;
        bool gotUltPowerSword = false;
        bool gotPowerArmor = false;
        bool gotUltPowerArmor = false;
        bool reloaded = false;


        if (CheckEvent("found_you"))
        {
            foundYou = true;
        }
        if (CheckEvent("the_one"))
        {
            theOne = true;
        }
        if (CheckEvent(GameConstants.RELOADED))
        {
            Debug.Log("FOUND RELOADED IN THE EVENT CLEAR FUNCTION...");
            reloaded = true;
        }
        if (CheckEvent(GameConstants.GOT_POWER_SWORD))
        {
            gotPowerSword = true;
        }
        if (CheckEvent(GameConstants.GOT_POWER_ARMOR))
        {
            gotPowerArmor = true;
        }
        if (CheckEvent(GameConstants.GOT_ULT_POWER_SWORD))
        {
            gotUltPowerSword = true;
        }
        if (CheckEvent(GameConstants.GOT_ULT_POWER_ARMOR))
        {
            gotUltPowerArmor = true;
        }

        int removeIndex = 0;

        foreach(string eventToCheck in events)
        {
            removeIndex++;
            if(eventToCheck == lastEvent)
                break;
        }

        events.RemoveRange(removeIndex, events.Count - 1 - removeIndex);

        if (foundYou)
        {
            SaveEvent("found_you");
        }
        if (theOne)
        {
            SaveEvent("the_one");
        }
        if (gotPowerSword)
        {
            SaveEvent(GameConstants.GOT_POWER_SWORD);
        }
        if (gotPowerArmor)
        {
            SaveEvent(GameConstants.GOT_POWER_ARMOR);
        }
        if (gotUltPowerSword)
        {
            SaveEvent(GameConstants.GOT_ULT_POWER_SWORD);
        }
        if (gotUltPowerArmor)
        {
            SaveEvent(GameConstants.GOT_ULT_POWER_ARMOR);
        }
        if (reloaded)
        {
            SaveEvent(GameConstants.RELOADED);
        }
    }
}
