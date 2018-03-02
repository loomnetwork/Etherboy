using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public List<string> events;

    public void TriggerEvent(string triggerEvent)
    {

        events.Add(triggerEvent);

        Debug.Log("GOT A NEW EVENT : " + triggerEvent);

        if(triggerEvent == "god_ray")
        {
            GameManager.instance.GodRayAppears();
        }
        else if(triggerEvent == "go_on_quest")
        {
            GameManager.instance.player.SendMessage("RunDialog", SendMessageOptions.RequireReceiver);
        }
        else if(triggerEvent == "place_token")
        {
            GameManager.instance.GodRayAppears();
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
}
