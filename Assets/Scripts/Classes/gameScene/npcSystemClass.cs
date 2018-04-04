using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

[System.Serializable]
public class automaticDialogueClass {
	public string[] dialogues;
	public float[] timeInBetween;
	public int currentDialogue;
	public string[] flagsAfterDialogue;
	public float timer;
	public string[] forOther;
}

[System.Serializable]
public class triggeredDialogueClass {
	public string[] dialogues;
	public float[] timeInBetween;
	public int currentDialogue;
	public string[] flagsAfterDialogue;
	public float timer;
	public string[] forOther;
}

[System.Serializable]
public class dialogueSystemClass {
	public bool hasAutomaticDialogue;
	public bool hasTriggeredDialogue;

	public automaticDialogueClass automaticDialogue;
	public triggeredDialogueClass triggeredDialogue;
}

public class npcSystemClass : MonoBehaviour {
	public string[] dialogueFiles;

	private int questSet;
	private dialogueSystemClass currentDialogue;

	private GameObject bubble;
	private TextMeshPro text;

	private bool startAutomatic;
	private bool readyForTriggered;
	private bool startTriggered;

	private GameObject character;

	// Use this for initialization
	void Start () {
		bubble = transform.GetChild (0).gameObject;
		text = bubble.transform.GetChild (0).GetComponent<TextMeshPro> ();
		text.text = "";
		questSet = globalScript.currentQuest;
		loadDialogueData ();
	}
	
	// Update is called once per frame
	void Update () {
		if (questSet != globalScript.currentQuest) {
			questSet = globalScript.currentQuest;
			loadDialogueData ();
		}

		if (startTriggered) {
			if (bubble.activeSelf == false) {
				bubble.SetActive (true);
			}

			text.text = currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue];
			currentDialogue.triggeredDialogue.timer += Time.deltaTime;

			GameObject other = null;
			Transform bubbleOther = null;
			if (currentDialogue.triggeredDialogue.forOther.Length >= currentDialogue.triggeredDialogue.currentDialogue && currentDialogue.triggeredDialogue.forOther [currentDialogue.triggeredDialogue.currentDialogue] != "") {
				text.text = "";
				other = GameObject.Find (currentDialogue.triggeredDialogue.forOther [currentDialogue.triggeredDialogue.currentDialogue]);

				if (other != null) {
					bubbleOther = other.transform.Find ("bubble");
					bubbleOther.gameObject.SetActive (true);
					bubbleOther.GetChild (0).GetComponent<TextMeshPro> ().text = currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue];

					npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
					if (otherNPC != null) {
						otherNPC.enabled = false;
					}
				}
			}

			if (text.text == "") {
				bubble.SetActive (false);
			}

			if (currentDialogue.triggeredDialogue.timer >= currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]) {
				currentDialogue.triggeredDialogue.timer = 0;
				currentDialogue.triggeredDialogue.currentDialogue++;
				if (other != null) {
					bubbleOther.gameObject.SetActive (false);

					npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
					if (otherNPC != null) {
						otherNPC.enabled = true;
					}
				}
				if (currentDialogue.triggeredDialogue.currentDialogue >= currentDialogue.triggeredDialogue.dialogues.Length) {
					startTriggered = false;
					character.GetComponent<characterClass> ().state = "normal";
					if (startAutomatic == false && bubble.activeSelf == true) {
						bubble.SetActive (false);
					}

					if (currentDialogue.triggeredDialogue.flagsAfterDialogue.Length >= 1) {
						if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "mentor1Talked") {
							globalScript.currentQuest = 1;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "hiro2Talked") {
							globalScript.currentQuest = 2;
						}
					}
				}
			}
		} else if (startAutomatic) {
			automaticDialogueStep ();
		} else {
			if (bubble.activeSelf == true) {
				bubble.SetActive (false);
			}
		}

		if (readyForTriggered) {
			bool pressedTalk = Input.GetButtonDown ("Fire2");

			if (pressedTalk) {
				if (!startTriggered) {
					if (character != null) {
						if (character.GetComponent<characterClass> () != null) {
							if (startAutomatic) {
								currentDialogue.automaticDialogue.timer = 999;
								automaticDialogueStep ();
							}
							character.GetComponent<characterClass> ().state = "talking";
							startTriggered = true;
							currentDialogue.triggeredDialogue.timer = 0;
							currentDialogue.triggeredDialogue.currentDialogue = 0;
						}
					}
				} else {
					currentDialogue.triggeredDialogue.timer = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue];
				}
			}
		}
	}

	void automaticDialogueStep () {
		if (bubble.activeSelf == false) {
			bubble.SetActive (true);
		}

		if (currentDialogue.automaticDialogue.dialogues [currentDialogue.automaticDialogue.currentDialogue] == "") {
			if (bubble.activeSelf == true) {
				bubble.SetActive (false);
			}
		}

		text.text = currentDialogue.automaticDialogue.dialogues [currentDialogue.automaticDialogue.currentDialogue];
		currentDialogue.automaticDialogue.timer += Time.deltaTime;

		GameObject other = null;
		Transform bubbleOther = null;
		if (currentDialogue.automaticDialogue.forOther.Length >= currentDialogue.automaticDialogue.currentDialogue && currentDialogue.automaticDialogue.forOther [currentDialogue.automaticDialogue.currentDialogue] != "") {
			text.text = "";
			other = GameObject.Find (currentDialogue.automaticDialogue.forOther [currentDialogue.automaticDialogue.currentDialogue]);

			if (other != null) {
				bubbleOther = other.transform.Find ("bubble");
				bubbleOther.gameObject.SetActive (true);
				bubbleOther.GetChild (0).GetComponent<TextMeshPro> ().text = currentDialogue.automaticDialogue.dialogues [currentDialogue.automaticDialogue.currentDialogue];

				npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
				if (otherNPC != null) {
					otherNPC.enabled = false;
				}
			}
		}

		if (text.text == "") {
			bubble.SetActive (false);
		}

		if (currentDialogue.automaticDialogue.timer >= currentDialogue.automaticDialogue.timeInBetween [currentDialogue.automaticDialogue.currentDialogue]) {
			currentDialogue.automaticDialogue.timer = 0;
			currentDialogue.automaticDialogue.currentDialogue++;

			if (other != null) {
				bubbleOther.gameObject.SetActive (false);

				npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
				if (otherNPC != null) {
					otherNPC.enabled = true;
				}
			}

			if (currentDialogue.automaticDialogue.currentDialogue >= currentDialogue.automaticDialogue.dialogues.Length) {
				currentDialogue.automaticDialogue.currentDialogue = 0;
			}
		}
	}

	void loadDialogueData () {
		int value = questSet;

		if (value >= dialogueFiles.Length) {
			value = dialogueFiles.Length - 1;
		}

		print (dialogueFiles.Length);
		if (dialogueFiles.Length > 0) {
			TextAsset targetFile = Resources.Load<TextAsset> (dialogueFiles [value]);

			currentDialogue = JsonUtility.FromJson<dialogueSystemClass> (targetFile.text);

			if (startAutomatic) {
				if (!currentDialogue.hasAutomaticDialogue) {
					startAutomatic = false;
				}
			}

			if (readyForTriggered) {
				if (!currentDialogue.hasTriggeredDialogue) {
					readyForTriggered = false;
				}
			}
		} else {
			currentDialogue = new dialogueSystemClass ();
			currentDialogue.hasAutomaticDialogue = false;
			currentDialogue.hasTriggeredDialogue = false;

			print ("DONE");
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Character") && collider.gameObject.name == "etherBoy") {
			if (currentDialogue.hasAutomaticDialogue) {
				startAutomatic = true;
			}

			if (currentDialogue.hasTriggeredDialogue) {
				readyForTriggered = true;
				character = collider.gameObject;
			}
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Character") && collider.gameObject.name == "etherBoy") {
			if (currentDialogue.hasAutomaticDialogue) {
				if (startAutomatic) {
					currentDialogue.automaticDialogue.timer = 999;
					automaticDialogueStep ();
				}
				currentDialogue.automaticDialogue.timer = 0;
				currentDialogue.automaticDialogue.currentDialogue = 0;
				startAutomatic = false;
			}

			if (currentDialogue.hasTriggeredDialogue) {
				readyForTriggered = false;
			}
		}
	}
}
