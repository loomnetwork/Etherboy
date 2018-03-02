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
    public Text text;

    public TextAsset xmlRawFile;
    public Dialog[] dialogs;

    public Dialog currentDialog = null;

    public int currentLine;
    public int lastLine;


    public float waitTime = 0f;
    public bool waiting;
    public float currentTime = 0f;

	// Use this for initialization
	void Start ()
    {
        string data = xmlRawFile.text;
        parseXmlFile(data);
    }

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
            
            XmlNode dialogInstance = node.FirstChild;
            XmlNode speaker = dialogInstance.NextSibling;
            XmlNode text = dialogInstance.NextSibling.NextSibling;
            XmlNode eventTriggered = dialogInstance.NextSibling.NextSibling.NextSibling;

            dialog = new Dialog(dialogInstance.InnerText, speaker.InnerText, text.InnerText, eventTriggered.InnerText);
            
            dialogs[i] = dialog;
            i++;
        }
    }
    

    private void FixedUpdate()
    {
        if (waiting)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= waitTime)
            {
                GameManager.instance.TriggerEvent(currentDialog.eventTriggered);
                HideDialogBubble();
                waiting = false;
            }
        }
    }

    public void MoveDialogBubble(GameObject targetObject)
    {
        Debug.Log("Target Object name: " + targetObject.name);
        Dialog dialog = GetDialogText(targetObject.name);
        Collider2D collider2D = targetObject.GetComponent<Collider2D>();
        
       // RectTransform rectTransform = dialogBubble.GetComponent<RectTransform>();

        if (targetObject.name != "UltimateWarrior")
        {
            dialogBubble.transform.position = new Vector2(targetObject.transform.position.x - 2.5f * collider2D.bounds.size.x, targetObject.transform.position.y + 1.5f * collider2D.bounds.size.y);
        }
        else
        {
            dialogBubble.transform.position = new Vector2(targetObject.transform.position.x - collider2D.bounds.size.x, targetObject.transform.position.y + collider2D.bounds.size.y);
        }
        text.text = dialog.dialog;

        waitTime = text.text.Length * .1f;
        waitTime = waitTime > 5f ? 5f : waitTime;

        waiting = true;
    }


    private Dialog GetDialogText(string characterName)
    {
        Dialog dialog = new Dialog("","","didnt find the damn npc","");
        foreach (Dialog _dialog in dialogs)
        {
            if (_dialog.speaker == characterName)
            {
                if (_dialog.eventTrigger == "")
                {
                    dialog = _dialog;
                }

                else if (GameManager.instance.HasEvent(_dialog.eventTrigger))
                {
                    dialog = _dialog;
                }
            }
        }

        currentDialog = dialog;
        return dialog;
    }


    private void HideDialogBubble()
    {
        dialogBubble.transform.position = new Vector2(-2, -2);
        waiting = false;
        GameManager.instance.SendMessage("ToggleLockInput");

        waitTime = 0f;
        currentTime = 0f;
    }
}
