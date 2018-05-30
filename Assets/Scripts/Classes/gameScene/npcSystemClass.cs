using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using TMPro;
using Puppet2D;

[System.Serializable]
public class automaticDialogueClass {
	public string[] dialogues;
	public float[] timeInBetween;
	public int currentDialogue;
	public string[] flagsAfterDialogue;
	public float timer;
	public string[] forOther;
	public string[] bubbleDirection;
	public string[] bubbleType;
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
	public int[] faceDirection;
	public bool[] skippable;
	public string[] bubbleDirection;
	public string[] bubbleType;
	public string[] playSounds;
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

	private Sprite shoutBubble;
	private Sprite whisperBubble;
	private Sprite normalBubble;

	private GameObject eSkipObj;

	private AudioSource[] scriptSFX;

	private bool firstIterationDialog;

	// Use this for initialization
	void Start () {
		Transform bubbleTrans = transform.Find ("bubbleGroup");
		if (bubbleTrans != null) {
			bubble = bubbleTrans.gameObject;
		}

		if (bubble != null) {
			text = bubble.transform.GetChild (0).transform.GetChild (0).GetComponent<TextMeshPro> ();
			text.text = "";
			text = bubble.transform.GetChild (1).transform.GetChild (0).GetComponent<TextMeshPro> ();
			text.text = "";

			normalBubble = bubble.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite;
			whisperBubble = Resources.Load<Sprite> ("whisperBubble");
			shoutBubble = Resources.Load<Sprite> ("shoutBubble");
		}
		questSet = globalScript.currentQuest;
		loadDialogueData ();

		eSkipObj = GameObject.Find ("actionTab");
		if (eSkipObj != null) {
			eSkipObj = eSkipObj.transform.GetChild (0).gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (questSet != globalScript.currentQuest) {
			questSet = globalScript.currentQuest;
			loadDialogueData ();
		}

		if (startTriggered && (activePopup == null || activePopup.activeSelf == false)) {
			GameObject subBubble = null;
			if (bubble != null && bubble.activeSelf == false) {
				bubble.SetActive (true);

				if (currentDialogue.triggeredDialogue.bubbleDirection != null && currentDialogue.triggeredDialogue.currentDialogue < currentDialogue.triggeredDialogue.bubbleDirection.Length
				    && currentDialogue.triggeredDialogue.bubbleDirection [currentDialogue.triggeredDialogue.currentDialogue] == "right") {
					subBubble = bubble.transform.GetChild (1).gameObject;
					subBubble.SetActive (true);
					bubble.transform.GetChild (0).gameObject.SetActive (false);
				} else {
					subBubble = bubble.transform.GetChild (0).gameObject;
					subBubble.SetActive (true);
					bubble.transform.GetChild (1).gameObject.SetActive (false);
				}

				if (currentDialogue.triggeredDialogue.bubbleType != null && currentDialogue.triggeredDialogue.bubbleType.Length > currentDialogue.triggeredDialogue.currentDialogue) {
					if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "normal") {
						subBubble.GetComponent<SpriteRenderer> ().sprite = normalBubble;
					} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "shout") {
						subBubble.GetComponent<SpriteRenderer> ().sprite = shoutBubble;
					} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "whisper") {
						subBubble.GetComponent<SpriteRenderer> ().sprite = whisperBubble;
					}
				}
			} 

			if (subBubble != null) {
				text = subBubble.transform.GetChild (0).GetComponent<TextMeshPro> ();;
			}

			if (text != null) {
				text.text = currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue];
			}

			#if UNITY_ANDROID
				currentDialogue.triggeredDialogue.timer += (1f / 60f);
			#else
				currentDialogue.triggeredDialogue.timer += Time.deltaTime;
			#endif

			if (currentDialogue.triggeredDialogue.playSounds != null && currentDialogue.triggeredDialogue.currentDialogue < currentDialogue.triggeredDialogue.playSounds.Length) {
				if (currentDialogue.triggeredDialogue.playSounds [currentDialogue.triggeredDialogue.currentDialogue] != "") {
					currentDialogue.triggeredDialogue.playSounds [currentDialogue.triggeredDialogue.currentDialogue] = "";
					if (scriptSFX != null) {
						if (currentDialogue.triggeredDialogue.currentDialogue < scriptSFX.Length) {
							if (scriptSFX [currentDialogue.triggeredDialogue.currentDialogue] != null) {
								scriptSFX [currentDialogue.triggeredDialogue.currentDialogue].Play ();
							}
						}
					}
				}
			}

			if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<bowSwordSelect>") {
				if (text != null) {
					text.text = "";
				}
				GameObject popup = GameObject.Find ("bowSwordSelect");
				popup = popup.transform.GetChild (0).gameObject;
				popup.SetActive (true);
				activePopup = popup;

				currentDialogue.triggeredDialogue.timer = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue];
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<openShop>") {
				if (text != null) {
					text.text = "";
				}
				GameObject popup = GameObject.Find ("shopGroup");
				popup = popup.transform.GetChild (0).gameObject;
				popup.SetActive (true);
				activePopup = popup;

				currentDialogue.triggeredDialogue.timer = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue];
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<magicBook>") {
				if (text != null) {
					text.text = "";
				}
				GameObject popup = GameObject.Find ("magicGroup");
				popup = popup.transform.GetChild (0).gameObject;
				popup.SetActive (true);
				activePopup = popup;

				currentDialogue.triggeredDialogue.timer = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue];
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<shakeScreen>") {
				if (text != null) {
					text.text = "";
				}
				if (firstIterationDialog && globalScript.shakeScreenTime <= 0) {
					firstIterationDialog = false;
					globalScript.groupToShake = GameObject.Find ("gameGroup");
					globalScript.basePositionGroupToShake = globalScript.groupToShake.transform.position;
					globalScript.shakeScreenTime = currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue] - 0.1f;
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<fadeToBlack>") {
				if (text != null) {
					text.text = "";
				}
				if (firstIterationDialog) {
					firstIterationDialog = false;
					globalScript.fadeToBlack (currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<deactivateRenderer>") {
				if (text != null) {
					text.text = "";
				}
				if (firstIterationDialog) {
					firstIterationDialog = false;
					GetComponent<Renderer> ().enabled = false;
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<deactivateGlobalCtrl>") {
				if (text != null) {
					text.text = "";
				}
				if (firstIterationDialog) {
					firstIterationDialog = false;
					transform.GetChild (1).gameObject.SetActive (false);
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<changeToDarkLevel3>") {
				if (firstIterationDialog) {
					firstIterationDialog = false;
					globalScript.changeScene ("darkForestLevel3Scene");
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<changeToDarkLevel2>") {
				if (firstIterationDialog) {
					firstIterationDialog = false;
					globalScript.changeScene ("darkForestLevel2Scene");
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<changeToDarkLevel1>") {
				if (firstIterationDialog) {
					firstIterationDialog = false;
					globalScript.changeScene ("darkForestLevel1Scene");
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<changeToLevel1>") {
				if (firstIterationDialog) {
					firstIterationDialog = false;
					globalScript.changeScene ("forestLevel1Scene");
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<youAreUploadedScene>") {
				if (firstIterationDialog) {
					firstIterationDialog = false;
					globalScript.changeScene ("uploadedScene");
					gameObject.SetActive (false);
					return;
				}
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<swapWeapon>") {
				if (globalScript.currentWeapon == "bow") {
					globalScript.currentWeapon = "sword";
				} else {
					globalScript.currentWeapon = "bow";
				}
				text.text = "";
			} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<startThumbWar>") {
				transform.GetChild (1).gameObject.SetActive (false);
				GameObject.Find ("Chaos").transform.GetChild (1).gameObject.SetActive (false);
				GameObject.Find ("ChaosHiro").transform.GetChild (1).gameObject.SetActive (true);
				text.text = "";
			}

			GameObject other = null;
			Transform bubbleOther = null;
			if (currentDialogue.triggeredDialogue.forOther.Length >= currentDialogue.triggeredDialogue.currentDialogue && currentDialogue.triggeredDialogue.forOther [currentDialogue.triggeredDialogue.currentDialogue] != "") {
				if (text != null) {
					text.text = "";
				}

				other = GameObject.Find (currentDialogue.triggeredDialogue.forOther [currentDialogue.triggeredDialogue.currentDialogue]);

				if (other != null) {
					if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<activateGlobalCtrl>") {
						currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] = "";
						if (text != null) {
							text.text = "";
						}
						if (firstIterationDialog) {
							other.transform.GetChild(1).gameObject.SetActive (true);
						}
					} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<activateRenderer>") {
						currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] = "";
						if (text != null) {
							text.text = "";
						}
						if (firstIterationDialog) {
							other.GetComponent<Renderer> ().enabled = true;
						}
					} else if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] == "<deactivateGravity>") {
						currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] = "";
						if (text != null) {
							text.text = "";
						}
						if (firstIterationDialog) {
							other.GetComponent<Rigidbody2D> ().gravityScale = 0;
						}
					}

					if (currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue] != "") {
						bubbleOther = other.transform.Find ("bubbleGroup");
						bubbleOther.gameObject.SetActive (true);
						GameObject subBubbleOther = null;
						if (currentDialogue.triggeredDialogue.bubbleDirection != null && currentDialogue.triggeredDialogue.currentDialogue < currentDialogue.triggeredDialogue.bubbleDirection.Length
						    && currentDialogue.triggeredDialogue.bubbleDirection [currentDialogue.triggeredDialogue.currentDialogue] == "right") { 
							bubbleOther.GetChild (0).gameObject.SetActive (false);
							subBubbleOther = bubbleOther.GetChild (1).gameObject;
							subBubbleOther.SetActive (true);
						} else {
							bubbleOther.GetChild (1).gameObject.SetActive (false);
							subBubbleOther = bubbleOther.GetChild (0).gameObject;
							subBubbleOther.SetActive (true);
						}

						if (currentDialogue.triggeredDialogue.bubbleType != null && currentDialogue.triggeredDialogue.bubbleType.Length > currentDialogue.triggeredDialogue.currentDialogue) {
							if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "normal") {
								subBubbleOther.GetComponent<SpriteRenderer> ().sprite = normalBubble;
							} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "shout") {
								subBubbleOther.GetComponent<SpriteRenderer> ().sprite = shoutBubble;
							} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "whisper") {
								subBubbleOther.GetComponent<SpriteRenderer> ().sprite = whisperBubble;
							}
						}

						subBubbleOther.transform.GetChild (0).GetComponent<TextMeshPro> ().text = currentDialogue.triggeredDialogue.dialogues [currentDialogue.triggeredDialogue.currentDialogue];
					}

					if (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue] != "") {
						GameObject glblCtrl = other.transform.Find ("Global_CTRL").gameObject;
						Animator otherAnim = glblCtrl.GetComponent<Animator> ();
						if (!otherAnim.GetCurrentAnimatorStateInfo (0).IsName (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue])) {
							otherAnim.Play (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue], -1, 0f);
						}
					}

					if (currentDialogue.triggeredDialogue.faceDirection != null && currentDialogue.triggeredDialogue.faceDirection.Length > 0
					    && currentDialogue.triggeredDialogue.currentDialogue < currentDialogue.triggeredDialogue.faceDirection.Length) {
						Transform globalCtrlTransform = other.transform.Find ("Global_CTRL");
						if (globalCtrlTransform != null) {
							GameObject glblCtrl = globalCtrlTransform.gameObject;
							if (currentDialogue.triggeredDialogue.faceDirection [currentDialogue.triggeredDialogue.currentDialogue] > 0) {
								glblCtrl.GetComponent<Puppet2D_GlobalControl> ().flip = true;
							} else if (currentDialogue.triggeredDialogue.faceDirection [currentDialogue.triggeredDialogue.currentDialogue] < 0) {
								glblCtrl.GetComponent<Puppet2D_GlobalControl> ().flip = false;
							}
						}
					}
						
					if (firstIterationDialog && currentDialogue.triggeredDialogue.newPosition [currentDialogue.triggeredDialogue.currentDialogue] != "") {
						

						string[] splitter = currentDialogue.triggeredDialogue.newPosition [currentDialogue.triggeredDialogue.currentDialogue].Split (new string[] { "/separate/" }, StringSplitOptions.None);

						Vector2 movePos = other.transform.parent.InverseTransformPoint (transform.position);

						bool moveX = false;
						bool moveY = false;
						if (splitter.Length > 1) {
							float valueX = float.Parse (splitter [0], CultureInfo.InvariantCulture);
							float valueY = float.Parse (splitter [1], CultureInfo.InvariantCulture);

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
								moveY = true;
							}
						} else {
							movePos = other.transform.localPosition;
						}

						if (moveX) {
							LeanTween.moveLocalX (other, movePos.x, currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
						}

						if (moveY) {
							LeanTween.moveLocalY (other, movePos.y, currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
						}
					}

					npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
					if (otherNPC != null) {
						otherNPC.enabled = false;
					}

					firstIterationDialog = false; 
				}
			} else {
				if (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue] != "") {
					GameObject glblCtrl = transform.Find ("Global_CTRL").gameObject;
					Animator thisAnim = glblCtrl.GetComponent<Animator> ();
					if (!thisAnim.GetCurrentAnimatorStateInfo (0).IsName (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue])) {
						thisAnim.Play (currentDialogue.triggeredDialogue.playAnimation [currentDialogue.triggeredDialogue.currentDialogue], -1, 0f);
					}
				}

				if (currentDialogue.triggeredDialogue.faceDirection != null && currentDialogue.triggeredDialogue.faceDirection.Length > 0
					&& currentDialogue.triggeredDialogue.currentDialogue < currentDialogue.triggeredDialogue.faceDirection.Length) {
					Transform globalCtrlTransform = transform.Find ("Global_CTRL");
					if (globalCtrlTransform != null) {
						GameObject glblCtrl = globalCtrlTransform.gameObject;
						if (currentDialogue.triggeredDialogue.faceDirection [currentDialogue.triggeredDialogue.currentDialogue] > 0) {
							glblCtrl.GetComponent<Puppet2D_GlobalControl> ().flip = true;
						} else if (currentDialogue.triggeredDialogue.faceDirection [currentDialogue.triggeredDialogue.currentDialogue] < 0) {
							glblCtrl.GetComponent<Puppet2D_GlobalControl> ().flip = false;
						}
					}
				}

				if (firstIterationDialog && currentDialogue.triggeredDialogue.newPosition [currentDialogue.triggeredDialogue.currentDialogue] != "") {


					string[] splitter = currentDialogue.triggeredDialogue.newPosition [currentDialogue.triggeredDialogue.currentDialogue].Split (new string[] { "/separate/" }, StringSplitOptions.None);

					Vector2 movePos = transform.parent.InverseTransformPoint (transform.position);

					bool moveX = false;
					bool moveY = false;
					if (splitter.Length > 1) {
						float valueX = float.Parse (splitter [0], CultureInfo.InvariantCulture);
						float valueY = float.Parse (splitter [1], CultureInfo.InvariantCulture);

						if (splitter [0] == "0") {
							movePos.x = transform.localPosition.x;
						} else {
							movePos.x = movePos.x + valueX;
							moveX = true;
						}

						if (splitter [1] == "0") {
							movePos.y = transform.localPosition.y;
						} else {
							movePos.y = movePos.y + valueY;
							moveY = true;
						}
					} else {
						movePos = other.transform.localPosition;
					}

					if (moveX) {
						LeanTween.moveLocalX (gameObject, movePos.x, currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
					}

					if (moveY) {
						LeanTween.moveLocalY (gameObject, movePos.y, currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]);
					}

					firstIterationDialog = false;
				}
			}

			if (text != null && text.text == "") {
				bubble.SetActive (false);
			} else {
				if (text != null && text.text.Contains ("<otherWeapon>")) {
					string otherWeapon = "sword";
					if (globalScript.currentWeapon == "sword") {
						otherWeapon = "bow";
					}
					text.text = text.text.Replace ("<otherWeapon>", otherWeapon);
				}
			}

			if (eSkipObj != null) {
				if (currentDialogue.triggeredDialogue.skippable [currentDialogue.triggeredDialogue.currentDialogue]) {
					eSkipObj.SetActive (true);
				} else {
					eSkipObj.SetActive (false);
				}
			}

			if (currentDialogue.triggeredDialogue.timer >= currentDialogue.triggeredDialogue.timeInBetween [currentDialogue.triggeredDialogue.currentDialogue]) {
				if (eSkipObj != null) {
					eSkipObj.SetActive (false);
				}
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
					if (bubble != null && startAutomatic == false && bubble.activeSelf == true) {
						bubble.SetActive (false);
						if (subBubble != null) {
							subBubble.SetActive (false);
						}
					}

					if (currentDialogue.triggeredDialogue.flagsAfterDialogue.Length >= 1) {
						GameObject indicator = GameObject.Find ("indicatorObjective");
						objectiveSystemClass indicatorScript = null;
						if (indicator != null) {
							indicatorScript = indicator.GetComponent<objectiveSystemClass> ();
						}
						if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "mentor1Talked") {
							globalScript.currentQuest = 1;
							globalScript.currentGold += 100;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "hiro2Talked") {
							globalScript.currentQuest = 2;
							globalScript.currentGold += 100;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "steveTalkedWithMentor1") {
							globalScript.currentQuest = 3;
							globalScript.currentGold += 100;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "shop1Talked") {
							globalScript.currentQuest = 4;
							globalScript.currentGold += 100;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "doneBeforeForest1") {
							GetComponent<npcSystemClass> ().enabled = false;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "reachedForest1") {
							globalScript.currentQuest = 5;
							globalScript.currentGold += 100;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "gotEarthOrb1") {
							globalScript.currentQuest = 6;
							globalScript.currentGold += 200;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "steveTalkedWithMentor2") {
							globalScript.currentQuest = 7;
							globalScript.currentGold += 200;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "wentToBed2") {
							globalScript.currentQuest = 8;
							globalScript.currentGold += 200;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "etherBoyTalkedWithMentorInTown2") {
							globalScript.currentQuest = 9;
							globalScript.currentGold += 200;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "gotFireOrb1") {
							globalScript.currentQuest = 10;
							globalScript.currentGold += 200;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "steveTalkedWithMentor3") {
							globalScript.currentQuest = 11;
							globalScript.currentGold += 1000;
							readyForTriggered = true;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "etherBoyTalkedWithMentorInTown3") {
							globalScript.currentQuest = 12;
							globalScript.currentGold += 1000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "gotIceOrb1") {
							globalScript.currentQuest = 13;
							globalScript.currentGold += 1000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "openedTempleDoor") {
							globalScript.currentQuest = 14;
							globalScript.currentGold += 1000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
							GetComponent<npcSystemClass> ().enabled = false;
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "etherBoyTalkedWithMentorInTemple") {
							globalScript.currentQuest = 15;
							globalScript.currentGold += 1000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "steveTurnedIntoChaos") {
							globalScript.currentQuest = 16;
							globalScript.currentGold += 5000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "gotAirOrb1") {
							globalScript.currentQuest = 17;
							globalScript.currentGold += 2000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "elderFightsChaos") {
							globalScript.currentQuest = 18;
							globalScript.currentGold += 1000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "hiroFightsChaos") {
							globalScript.currentQuest = 19;
							globalScript.currentGold += 5000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						} else if (currentDialogue.triggeredDialogue.flagsAfterDialogue [0] == "gameCompleted") {
							globalScript.currentQuest = 20;
							globalScript.currentGold += 1000000;
							if (indicatorScript != null) {
								indicatorScript.showRewardScreen ();
							}
						}
					}
				}
			}
		} else if (startAutomatic && (activePopup == null || activePopup.activeSelf == false)) {
			automaticDialogueStep ();
		} else {
			if (bubble != null && bubble.activeSelf == true) {
				bubble.SetActive (false);
			}
		}

		if (readyForTriggered || activateTriggeredManually || startTriggered) {
			bool pressedTalk = inputBroker.GetButtonDown ("Fire2");

			if (pressedTalk || activateTriggeredManually) {
				if (activateTriggeredManually) {
					character = GameObject.Find ("etherBoy");
					activateTriggeredManually = false;
				}
				if (!startTriggered) {
					if (character != null) {
						if (character.GetComponent<characterClass> () != null) {
							if (currentDialogue != null && currentDialogue.triggeredDialogue != null) {
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
		if (bubble == null) {
			return;
		}

		if (bubble.activeSelf == false) {
			bubble.SetActive (true);
		}

		if (currentDialogue.automaticDialogue.dialogues [currentDialogue.automaticDialogue.currentDialogue] == "") {
			if (bubble.activeSelf == true) {
				bubble.SetActive (false);
			}
		}

		GameObject subBubble = null;
		if (bubble != null && bubble.activeSelf) {
			if (currentDialogue.automaticDialogue.bubbleDirection != null && currentDialogue.automaticDialogue.currentDialogue < currentDialogue.automaticDialogue.bubbleDirection.Length
				&& currentDialogue.automaticDialogue.bubbleDirection [currentDialogue.automaticDialogue.currentDialogue] == "right") {
				subBubble = bubble.transform.GetChild (1).gameObject;
				subBubble.SetActive (true);
				bubble.transform.GetChild (0).gameObject.SetActive (false);
			} else {
				subBubble = bubble.transform.GetChild (0).gameObject;
				subBubble.SetActive (true);
				bubble.transform.GetChild (1).gameObject.SetActive (false);
			}

			if (currentDialogue.triggeredDialogue.bubbleType != null && currentDialogue.triggeredDialogue.bubbleType.Length > currentDialogue.triggeredDialogue.currentDialogue) {
				if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "normal") {
					subBubble.GetComponent<SpriteRenderer> ().sprite = normalBubble;
				} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "shout") {
					subBubble.GetComponent<SpriteRenderer> ().sprite = shoutBubble;
				} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "whisper") {
					subBubble.GetComponent<SpriteRenderer> ().sprite = whisperBubble;
				}
			}
		} 

		if (subBubble != null) {
			text = subBubble.transform.GetChild (0).GetComponent<TextMeshPro> ();;
		}

		if (text != null) {
			text.text = currentDialogue.automaticDialogue.dialogues [currentDialogue.automaticDialogue.currentDialogue];
		}
		currentDialogue.automaticDialogue.timer += Time.deltaTime;

		GameObject other = null;
		Transform bubbleOther = null;
		if (currentDialogue.automaticDialogue.forOther.Length >= currentDialogue.automaticDialogue.currentDialogue && currentDialogue.automaticDialogue.forOther [currentDialogue.automaticDialogue.currentDialogue] != "") {
			text.text = "";
			other = GameObject.Find (currentDialogue.automaticDialogue.forOther [currentDialogue.automaticDialogue.currentDialogue]);

			if (other != null) {
				bubbleOther = other.transform.Find ("bubbleGroup");
				bubbleOther.gameObject.SetActive (true);
				GameObject subBubbleOther = null;
				if (currentDialogue.automaticDialogue.bubbleDirection != null && currentDialogue.automaticDialogue.currentDialogue < currentDialogue.automaticDialogue.bubbleDirection.Length
				    && currentDialogue.automaticDialogue.bubbleDirection [currentDialogue.automaticDialogue.currentDialogue] == "right") { 
					bubbleOther.GetChild (0).gameObject.SetActive (false);
					subBubbleOther = bubbleOther.GetChild (1).gameObject;
					subBubbleOther.SetActive (true);
				} else {
					bubbleOther.GetChild (1).gameObject.SetActive (false);
					subBubbleOther = bubbleOther.GetChild (0).gameObject;
					subBubbleOther.SetActive (true);
				}

				if (currentDialogue.triggeredDialogue.bubbleType != null && currentDialogue.triggeredDialogue.bubbleType.Length > currentDialogue.triggeredDialogue.currentDialogue) {
					if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "normal") {
						subBubbleOther.GetComponent<SpriteRenderer> ().sprite = normalBubble;
					} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "shout") {
						subBubbleOther.GetComponent<SpriteRenderer> ().sprite = shoutBubble;
					} else if (currentDialogue.triggeredDialogue.bubbleType [currentDialogue.triggeredDialogue.currentDialogue] == "whisper") {
						subBubbleOther.GetComponent<SpriteRenderer> ().sprite = whisperBubble;
					}
				}

				subBubbleOther.transform.GetChild (0).GetComponent<TextMeshPro> ().text = currentDialogue.automaticDialogue.dialogues [currentDialogue.automaticDialogue.currentDialogue];

				npcSystemClass otherNPC = other.GetComponent<npcSystemClass> ();
				if (otherNPC != null) {
					otherNPC.enabled = false;
				}
			}
		}

		if (text != null && text.text == "") {
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

	void loadScriptSFX () {
		if (currentDialogue != null) {
			if (currentDialogue.hasTriggeredDialogue) {
				if (currentDialogue.triggeredDialogue.playSounds != null) {
					if (currentDialogue.triggeredDialogue.playSounds.Length > 0) {
						scriptSFX = new AudioSource[currentDialogue.triggeredDialogue.playSounds.Length];

						for (int i = 0; i < currentDialogue.triggeredDialogue.playSounds.Length; i++) {
							if (currentDialogue.triggeredDialogue.playSounds [i] != "") {
								string audioName = currentDialogue.triggeredDialogue.playSounds [i];
								AudioSource audioSFX = gameObject.AddComponent<AudioSource> ();
								audioSFX.playOnAwake = false;
								scriptSFX [i] = audioSFX;
								AudioClip clip = Resources.Load<AudioClip> (audioName);
								audioSFX.clip = clip;
								print (audioName);
								print (clip);
							}
						}
					}
				}
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
			
		if (dialogueFiles.Length > 0 && value < dialogueFiles.Length && dialogueFiles [value] != "") {
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

			loadScriptSFX ();
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
				} else if (transform.name == "fire_sphere") {
					GetComponent<Collider2D> ().enabled = false;
					activateTriggeredManually = true;
				} else if (transform.name == "ice_sphere") {
					GetComponent<Collider2D> ().enabled = false;
					activateTriggeredManually = true;
				} else if (transform.name == "air_sphere") {
					GetComponent<Collider2D> ().enabled = false;
					activateTriggeredManually = true;
				} else if (transform.name == "lastElder") {
					GetComponent<Collider2D> ().enabled = false;
					activateTriggeredManually = true;
				} else if (transform.name == "templeDoor") {
					GetComponent<Collider2D> ().enabled = false;
					activateTriggeredManually = true;
				} else if (transform.name == "Steve") {
					if (globalScript.currentQuest == 15) {
						GetComponent<Collider2D> ().enabled = false;
						activateTriggeredManually = true;
					}
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
