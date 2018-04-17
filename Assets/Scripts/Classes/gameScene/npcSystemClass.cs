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
	public string[] newPosition;
	public string[] playAnimation;
	public bool[] skippable;
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
	public bool skipLast;

	private int questSet;
	private dialogueSystemClass currentDialogue;

	private GameObject bubble;
	private TextMeshPro text;

	private bool startAutomatic;
	private bool readyForTriggered;
	[HideInInspector]
	public bool activateTriggeredManually;
	private bool startTriggered;

	private GameObject character;
	private GameObject activePopup;

	private bool firstIterationDialog;

	// Use this for initialization
	void Start () {
		bubble = transform.Find ("bubble").gameObject;
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

		if (startTriggered && (activePopup == null || activePopup.activeSelf == false)) {
			if (bubble.activeSelf == false) {
				bubble.SetActive (true);
			}
				
			text.text = currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue];
			currentDialogue.triggeredDialogue.timer += Time.deltaTime;

			if (text.text == "<bowSwordSelect>") {
				text.text = "";
				GameObject popup = GameObject.Find ("bowSwordSelect");
				popup = popup.transform.GetChild (0).gameObject;
				popup.SetActive (true);
				activePopup = popup;

				currentDialogue.triggeredDialogue.timer = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue];
			}

			GameObject other = null;
			Transform bubbleOther = null;
			if (currentDialogue.triggeredDialogue.forOther.Length >= currentDialogue.triggeredDialogue.currentDialogue && currentDialogue.triggeredDialogue.forOther [currentDialogue.triggeredDialogue.currentDialogue] != "") {
				text.text = "";
				other = GameObject.Find (currentDialogue.triggeredDialogue.forOther [currentDialogue.triggeredDialogue.currentDialogue]);

				if (other != null) {
					if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] != "") {
						bubbleOther = other.transform.Find ("bubble");
						bubbleOther.gameObject.SetActive (true);
						bubbleOther.GetChild (0).GetComponent<TextMeshPro> ().text = currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue];
					}

					if (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue] != "") {
						GameObject glblCtrl = other.transform.Find ("Global_CTRL").gameObject;
						Animator otherAnim = glblCtrl.GetComponent<Animator> ();
						if (!otherAnim.GetCurrentAnimatorStateInfo (0).IsName (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue])) {
							otherAnim.Play (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue], -1, 0f);
						}
					}
						
					if (firstIterationDialog && currentDialogue.triggeredDialogue.newPosition [currentDialogue.triggeredDialogue.currentDialogue] != "") {
						firstIterationDialog = false;

						string[] splitter = currentDialogue.triggeredDialogue.newPosition [currentDialogue.triggeredDialogue.currentDialogue].Split (new string[] { "/separate/" }, StringSplitOptions.None);

						Vector2 movePos = other.transform.parent.InverseTransformPoint (transform.position);

						bool moveX = false;
						bool moveY = false;
						if (splitter.Length > 1) {
							float valueX = float.Parse (splitter [0]);
							float valueY = float.Parse (splitter [1]);

							if (splitter [0] == "0") {
								movePos.x = other.transform.localPosition.x;
							} else {
								movePos.x = movePos.x + valueX;
								moveX = true;
							}

							if (splitter [1] == "0") {
								movePos.y = other.transform.localPosition.y;
							} else {
								movePos.y = movePos.y + valueY;
								moveY = moveX;
							}
						} else {
							movePos = other.transform.localPosition;
						}

						if (moveX) {
							LeanTween.moveLocalX (other, movePos.x, currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
						}

						if (moveY) {
							LeanTween.moveLocalX (other, movePos.y, currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
						}
					}

					npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
					if (otherNPC != null) {
						otherNPC.enabled = false;
					}
				}
			} else {
				if (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue] != "") {
					GameObject glblCtrl = transform.Find ("Global_CTRL").gameObject;
					Animator thisAnim = glblCtrl.GetComponent<Animator> ();
					if (!thisAnim.GetCurrentAnimatorStateInfo (0).IsName (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue])) {
						thisAnim.Play (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue], -1, 0f);
					}
				}
			}

			if (text.text == "") {
				bubble.SetActive (false);
			} else {
				if (text.text.Contains ("<otherWeapon>")) {
					string otherWeapon = "sword";
					if (globalScript.currentWeapon == "sword") {
						otherWeapon = "bow";
					}
					text.text = text.text.Replace ("<otherWeapon>", otherWeapon);
				}
			}

			if (currentDialogue.triggeredDialogue.timer >= currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]) {
				currentDialogue.triggeredDialogue.timer = 0;
				currentDialogue.triggeredDialogue.currentDialogue++;
				firstIterationDialog = true;
				if (other != null) {
					if (bubbleOther != null) {
						bubbleOther.gameObject.SetActive (false);
					}

					npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
					if (otherNPC != null) {
						otherNPC.enabled = true;
					}
				}
				if (currentDialogue.triggeredDialogue.currentDialogue >= currentDialogue.triggeredDialogue.dialogues.Length) {
					firstIterationDialog = false;
					startTriggered = false;
					globalScript.gameState = "Normal";
					character.GetComponent<characterClass> ().state = "normal";
					if (startAutomatic == false && bubble.activeSelf == true) {
						bubble.SetActive (false);
					}

					if (currentDialogue.triggeredDialogue.flagsAfterDialogue.Length >= 1) {
						if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "mentor1Talked") {
							globalScript.currentQuest = 1;
							globalScript.currentGold += 100;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "hiro2Talked") {
							globalScript.currentQuest = 2;
							globalScript.currentGold += 100;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "steveTalkedWithMentor1") {
							globalScript.currentQuest = 3;
							globalScript.currentGold += 100;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "shop1Talked") {
							globalScript.currentQuest = 4;
							globalScript.currentGold += 100;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "doneBeforeForest1") {
							GetComponent<npcSystemClass> ().enabled = false;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "reachedForest1") {
							globalScript.currentQuest = 5;
							globalScript.currentGold += 100;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "gotEarthOrb1") {
							globalScript.currentQuest = 6;
							globalScript.currentGold += 200;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "steveTalkedWithMentor2") {
							globalScript.currentQuest = 7;
							globalScript.currentGold += 200;
						}
					}
				}
			}
		} else if (startAutomatic && (activePopup == null || activePopup.activeSelf == false)) {
			automaticDialogueStep ();
		} else {
			if (bubble.activeSelf == true) {
				bubble.SetActive (false);
			}
		}

		if (readyForTriggered || activateTriggeredManually || startTriggered) {
			bool pressedTalk = Input.GetButtonDown ("Fire2");

			if (pressedTalk || activateTriggeredManually) {
				if (activateTriggeredManually) {
					character = GameObject.Find ("etherBoy");
					activateTriggeredManually = false;
				}
				if (!startTriggered) {
					if (character != null) {
						if (character.GetComponent<characterClass> () != null) {
							if (startAutomatic) {
								currentDialogue.automaticDialogue.timer = 999;
								automaticDialogueStep ();
							}
							globalScript.gameState = "isTalking";
							character.GetComponent<characterClass> ().state = "talking";
							startTriggered = true;
							firstIterationDialog = true;
							currentDialogue.triggeredDialogue.timer = 0;
							currentDialogue.triggeredDialogue.currentDialogue = 0;
						}
					}
				} else {
					if (currentDialogue.triggeredDialogue.skippable [currentDialogue.triggeredDialogue.currentDialogue]) {
						currentDialogue.triggeredDialogue.timer = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue];
					}
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

		if (!skipLast) {
			if (value >= dialogueFiles.Length) {
				value = dialogueFiles.Length - 1;
			}
		}

		print (transform.name);

		if (dialogueFiles.Length > 0 && dialogueFiles [value] != "") {
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
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (globalScript.gameState == "isTalking") {
			return;
		}
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Character") && collider.gameObject.name == "etherBoy") {
			if (currentDialogue.hasAutomaticDialogue) {
				startAutomatic = true;
			}

			if (currentDialogue.hasTriggeredDialogue) {
				readyForTriggered = true;
				character = collider.gameObject;

				if (transform.name == "earth_sphere") {
					GetComponent<Collider2D> ().enabled = false;
					activateTriggeredManually = true;
				}
			}
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (globalScript.gameState == "isTalking") {
			return;
		}
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
