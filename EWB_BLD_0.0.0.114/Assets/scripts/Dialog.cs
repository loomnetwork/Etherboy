using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog
{
    public string eventTrigger;
    public string speaker;
    public string dialogText;
    public string eventTriggered;
    public bool isRandom;
    public bool saveEvent;

    /*
     * DIALOG OBJECT CLASS USED FOR PASSING COMPLEX INSTRUCTIONS
     */
    public Dialog()
    {
        eventTrigger = "";
        speaker = "";
        dialogText = "";
        eventTrigger = "";
        isRandom = false;
        saveEvent = true;
    }

    public Dialog(string _eventTrigger, string _speaker, string _dialogText, string _eventTriggered, bool _isRandom, bool _saveEvent)
    {
        eventTrigger = _eventTrigger;
        speaker = _speaker;
        dialogText = _dialogText;
        eventTriggered = _eventTriggered;
        isRandom = _isRandom;
        saveEvent = _saveEvent;
    }
}

