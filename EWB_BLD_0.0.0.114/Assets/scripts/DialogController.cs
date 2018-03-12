using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public GameObject dialogBubble;
    public Character currentSpeaker = null;
    public Text text;

    public TextAsset xmlRawFile;
    public Dialog[] dialogs;

    public Dialog currentDialog = null;

    public int currentLine;
    public int lastLine;


    public float waitTime = 0f;
    public float currentTime = 0f;

    public bool waiting = false;
    public bool resetDialogBubble = false;
    
    /*
     * MANAGER CLASS USED TO CONTROL DIALOGS
     */
    public void SetSceneDialog (TextAsset sceneDialog)
    {
        xmlRawFile = sceneDialog;
        string data = xmlRawFile.text;
        parseXmlFile(data);
    }

    /*
     * PARSE SCENE XML BASED ON WHAT IS STORED IN THE LEVEL-ART GAME OBJECT CLASS (*which should be renamed and restructured to be more generic for multiple scenes)
     */
    void parseXmlFile(string xmlData)
    {
        Dialog dialog;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlData));
        string xmlPathPattern = "//dialogs/dialog";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);
        
        dialogs = new Dialog[myNodeList.Count];
        int i = 0;

        foreach (XmlNode node in myNodeList)
        {
            string dialogInstance = node["name"].InnerText;
            string speaker = node["speaker"].InnerText;
            string text = node["text"].InnerText;
            string eventTriggered = node["eventTriggered"].InnerText;


            bool isRandom = node["isRandom"] == null ? true : Convert.ToBoolean(node["isRandom"].InnerText);
            bool saveEvent = node["saveEvent"] == null ? true : Convert.ToBoolean(node["saveEvent"].InnerText);

            dialog = new Dialog(dialogInstance, speaker, text, eventTriggered, isRandom, saveEvent);
            
            dialogs[i] = dialog;
            i++;
        }
    }

    public Dialog GetDialog(string dialogTrigger)
    {
        Dialog dialog = new Dialog();

        foreach (Dialog _dialog in dialogs)
        {
            if (_dialog.eventTrigger == dialogTrigger)
            {
                
                dialog = _dialog;
            }
        }

        return dialog;
    }
    
    private void FixedUpdate()
    {
        if (waiting)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= waitTime)
            {
                waiting = false;
                HideDialogBubble();
            }
        }
        else if (resetDialogBubble)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= waitTime)
            {
                resetDialogBubble = false;
                currentTime = 0f;
                waitTime = 0f;

                if (currentDialog != null)
                {
                    GameManager.instance.eventsManager.TriggerEvent(currentDialog.eventTriggered, currentDialog.saveEvent);
                }
            }
        }
    }

    /*
     * SHOULD BE EXPANDED TO INSTANTIATE A NEW DIALOG BUBBLE IN SCENE SPACE AND NOT JUST RECYCLE THE ONE BUBBLE STORED IN THE INSPECTOR
     */
    public void MoveDialogBubble(Character targetObject, Dialog dialog = null)
    {
        if (dialog == null)
        {
            dialog = GetDialogText(targetObject.name);
        }
        else
        {
            currentDialog = dialog;
        }

        Collider2D collider2D = targetObject.GetComponent<Collider2D>();

        currentSpeaker = targetObject;
        currentSpeaker.IsSpeaking = true;
        
        if (targetObject.name == "Player")
        {
            dialogBubble.transform.position = new Vector2(targetObject.transform.position.x - 4.5f * collider2D.bounds.size.x, targetObject.transform.position.y + 1.2f * collider2D.bounds.size.y);
        }
        else if(targetObject.name == "UltimateWarrior")
        {
            dialogBubble.transform.position = new Vector2(targetObject.transform.position.x - collider2D.bounds.size.x, targetObject.transform.position.y + .5f + collider2D.bounds.size.y);
        }
        else
        {
            dialogBubble.transform.position = new Vector2(targetObject.transform.position.x - 2.5f * collider2D.bounds.size.x, targetObject.transform.position.y + 1.5f * collider2D.bounds.size.y);
        }
        text.text = dialog.dialogText;

        currentTime = 0;
        waitTime = text.text.Length * .1f;
        waitTime = waitTime > 5f ? 5f : waitTime;

        waiting = true;
    }



    public Dialog EventTriggeredDialog(string eventTrigger)
    {
        foreach (Dialog dialog in dialogs)
        {
            if (dialog.eventTrigger == eventTrigger)
            {
                currentDialog = dialog;
                return dialog;
            }
        }

        return null;
    }


    private Dialog GetDialogText(string characterName, string dialogName = "")
    {
        Dialog dialog = new Dialog();


        /// LETS GIVE YOU THE OPTION TO PROVIDE A DIALOG NAME
        foreach (Dialog _dialog in dialogs)
        {
            if (_dialog.speaker == characterName)
            {
                if (_dialog.eventTrigger == "" || GameManager.instance.eventsManager.CheckEvent(_dialog.eventTrigger))
                {
                    dialog = _dialog;
                }
            }
        }

        currentDialog = dialog;
        return dialog;
    }


    public void HideDialogBubble(bool walkedAway = false)
    {
        dialogBubble.transform.position = new Vector2(-12, -12);

        if (currentSpeaker)
        {
            currentSpeaker.IsSpeaking = false;
            currentSpeaker = null;
        }

        if (walkedAway)
        {
            currentDialog = null;
        }

        waitTime = .2f;
        currentTime = 0f;
        waiting = false;
        resetDialogBubble = true;
    }
}
