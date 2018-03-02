using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog
{
    public string eventTrigger;
    public string speaker;
    public string dialog;
    public string eventTriggered;

    public Dialog(string _eventTrigger, string _speaker, string _dialog, string _eventTriggered)
    {
        eventTrigger = _eventTrigger;
        speaker = _speaker;
        dialog = _dialog;
        eventTriggered = _eventTriggered;
    }
}

