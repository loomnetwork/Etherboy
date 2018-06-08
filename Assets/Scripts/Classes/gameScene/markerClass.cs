using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Anim_Sys;

public class markerClass : MonoBehaviour {

	public GameObject[] neededObjectives;

	private Vector2 leftTopSide;
	private Vector2 rightBottomSide;

	private Vector2 objectivePoint;
	private Vector2 unalteredPoint;

	private objectiveSystemClass indicatorScript;

	private GameObject pointer;
	private GameObject arrow;
	// Use this for initialization
	void Start () {
		leftTopSide = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
		rightBottomSide = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

		pointer = transform.GetChild (0).gameObject;
		arrow = pointer.transform.GetChild (0).gameObject;

		GameObject indicator = GameObject.Find ("indicatorObjective");
		if (indicator != null) {
			indicatorScript = indicator.GetComponent<objectiveSystemClass> ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (indicatorScript != null && indicatorScript.activeQuest != globalScript.currentQuest) {
			if (indicatorScript.status == "idle") {
				indicatorScript.showNewObjective ();
			}
		}

		bool found = true;
		if (SceneManager.GetActiveScene ().name == "townLevel1Scene") {
			if (globalScript.currentQuest == 0) {
				GameObject NPC = neededObjectives [0];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 1) {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 2) {
				GameObject NPC = neededObjectives [0];
				if (globalScript.gameState != "isTalking") {
					GameObject Steve = neededObjectives [3];
					if (Steve.activeSelf != true) {
						Steve.SetActive (true);
						Steve.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 1, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain1 = neededObjectives [5];
						Villain1.SetActive (true);
						Villain1.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 1, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain2 = neededObjectives [6];
						Villain2.SetActive (true);
						Villain2.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 2, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain3 = neededObjectives [8];
						Villain3.SetActive (true);
						Villain3.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 3, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain4 = neededObjectives [9];
						Villain4.SetActive (true);
						Villain4.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 2, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain5 = neededObjectives [10];
						Villain5.SetActive (true);
						Villain5.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 3, NPC.transform.localPosition.y - 0.2f);
					}

					GameObject Character = neededObjectives [7];
					if (Character.transform.localPosition.x > -16) {
						globalScript.gameState = "isTalking";
						LeanTween.value (0, 1, 0).setOnComplete (() => {
							NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
						});
					}
				}
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 3) {
				if (neededObjectives [3].activeSelf) {
					neededObjectives [3].SetActive (false);
					neededObjectives [5].SetActive (false);
					neededObjectives [6].SetActive (false);
					neededObjectives [8].SetActive (false);
					neededObjectives [9].SetActive (false);
					neededObjectives [10].SetActive (false);
				}
				GameObject NPC = neededObjectives [4];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 1.95f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 4 || globalScript.currentQuest == 5 || globalScript.currentQuest == 6) {
				GameObject door = neededObjectives [2];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1;

				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "innLevel1Scene") {
			if (globalScript.currentQuest == 0) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 1) {
				GameObject NPC = neededObjectives [1];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2.15f;
				objectivePoint.x += 0f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 2) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 3) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 4 || globalScript.currentQuest == 5 || globalScript.currentQuest == 6) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "privateRoom1Scene") {
			if (globalScript.currentQuest == 0) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 1) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 2) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 3) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 4 || globalScript.currentQuest == 5 || globalScript.currentQuest == 6) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "forestLevel1Scene") {
			if (globalScript.currentQuest != 5) {
				neededObjectives [1].SetActive (false);
				neededObjectives [1].transform.parent.gameObject.SetActive (false);
			}
			if (globalScript.currentQuest == 0) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
				unalteredPoint.x -= 10;
			} else if (globalScript.currentQuest == 1) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
				unalteredPoint.x -= 10;
			} else if (globalScript.currentQuest == 2) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
				unalteredPoint.x -= 10;
			} else if (globalScript.currentQuest == 3) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
				unalteredPoint.x -= 10;
			} else if (globalScript.currentQuest == 4) {
				GameObject NPC = neededObjectives [0];
				if (globalScript.gameState != "isTalking") {
					globalScript.gameState = "isTalking";
					NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
				}
				found = false;
			} else if (globalScript.currentQuest == 5) {
				GameObject sphere = neededObjectives [1];
				sphere.transform.parent.gameObject.SetActive (true);
				sphere.SetActive (true);
				objectivePoint = sphere.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0;

				unalteredPoint = sphere.transform.position;
			} else if (globalScript.currentQuest == 6) {
				GameObject path = neededObjectives [2];
				path.SetActive (true);
				objectivePoint = path.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 0;

				unalteredPoint = path.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "townLevel2Scene") {
			if (globalScript.currentQuest < 6) {
				GameObject door = neededObjectives [2];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 6) {
				GameObject NPC = neededObjectives [0];
				if (globalScript.gameState != "isTalking") {
					GameObject Steve = neededObjectives [3];
					if (Steve.activeSelf != true) {
						Steve.SetActive (true);
						Steve.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 1, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain1 = neededObjectives [6];
						Villain1.SetActive (true);
						Villain1.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 1, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain2 = neededObjectives [7];
						Villain2.SetActive (true);
						Villain2.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 2, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain3 = neededObjectives [8];
						Villain3.SetActive (true);
						Villain3.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 3, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain4 = neededObjectives [9];
						Villain4.SetActive (true);
						Villain4.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 2, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain5 = neededObjectives [10];
						Villain5.SetActive (true);
						Villain5.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 3, NPC.transform.localPosition.y - 0.2f);
					}

					GameObject Character = neededObjectives [5];
					if (Character.transform.localPosition.x > -19.5f) {
						globalScript.gameState = "isTalking";
						LeanTween.value (0, 1, 0).setOnComplete (() => {
							NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
						});
					}
				}
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 7) {
				if (neededObjectives [3].activeSelf) {
					neededObjectives [3].SetActive (false);
					neededObjectives [6].SetActive (false);
					neededObjectives [7].SetActive (false);
					neededObjectives [8].SetActive (false);
					neededObjectives [9].SetActive (false);
					neededObjectives [10].SetActive (false);
				}
				GameObject door = neededObjectives [4];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;
				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 8) {
				GameObject NPC = neededObjectives [0];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 9) {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1f;
				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "innLevel2Scene") {
			if (globalScript.currentQuest == 7) {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;
				unalteredPoint = door.transform.position;
			} else {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;
				unalteredPoint = door.transform.position;
			}
		} else if (SceneManager.GetActiveScene ().name == "privateRoom2Scene") {
			if (globalScript.currentQuest == 7) {
				GameObject NPC = neededObjectives [0];
				if (globalScript.gameState != "isTalking") {
					GameObject character = neededObjectives [2];
					if (character.transform.localPosition.x > 3.56f) {
						globalScript.gameState = "isTalking";
						NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
					}
				}
				GameObject bed = neededObjectives [3];
				bed.SetActive (false);
				objectivePoint = bed.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0;
				unalteredPoint = bed.transform.position;
			} else {
				GameObject bed = neededObjectives [3];
				bed.SetActive (true);
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;
				unalteredPoint = door.transform.position;
			}
		} else if (SceneManager.GetActiveScene ().name == "forestLevel2Scene") {
			if (globalScript.currentQuest != 9) {
				neededObjectives [0].SetActive (false);
			}

			if (globalScript.currentQuest == 9) {
				GameObject sphere = neededObjectives [0];
				sphere.transform.parent.gameObject.SetActive (true);
				sphere.SetActive (true);
				objectivePoint = sphere.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0;

				unalteredPoint = sphere.transform.position;
			} else if (globalScript.currentQuest == 10) {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1;

				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "townLevel3Scene") {
			if (globalScript.currentQuest < 10) {
				GameObject door = neededObjectives [2];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
			} 
			if (globalScript.currentQuest == 10) {
				GameObject NPC = neededObjectives [0];
				if (globalScript.gameState != "isTalking") {
					GameObject Steve = neededObjectives [3];
					if (Steve.activeSelf != true) {
						Steve.SetActive (true);
						Steve.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 1, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain1 = neededObjectives [4];
						Villain1.SetActive (true);
						Villain1.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 1, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain2 = neededObjectives [5];
						Villain2.SetActive (true);
						Villain2.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 2, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain3 = neededObjectives [6];
						Villain3.SetActive (true);
						Villain3.transform.localPosition = new Vector2 (NPC.transform.localPosition.x + 3, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain4 = neededObjectives [7];
						Villain4.SetActive (true);
						Villain4.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 2, NPC.transform.localPosition.y - 0.2f);
						GameObject Villain5 = neededObjectives [8];
						Villain5.SetActive (true);
						Villain5.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 3, NPC.transform.localPosition.y - 0.2f);
					}

					GameObject Character = neededObjectives [9];
					if (Character.transform.localPosition.x > -18) {
						globalScript.gameState = "isTalking";
						LeanTween.value (0, 1, 0f).setOnComplete (() => {
							NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
						});
					}
				}
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 11) {
				if (neededObjectives [3].activeSelf) {
					neededObjectives [3].SetActive (false);
					neededObjectives [4].SetActive (false);
					neededObjectives [5].SetActive (false);
					neededObjectives [6].SetActive (false);
					neededObjectives [7].SetActive (false);
					neededObjectives [8].SetActive (false);
				}
				GameObject NPC = neededObjectives [0];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;
			} else if (globalScript.currentQuest == 12) {
				if (neededObjectives [3].activeSelf) {
					neededObjectives [3].SetActive (false);
				}
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1;

				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "forestLevel3Scene") {
			if (globalScript.currentQuest != 12) {
				neededObjectives [0].SetActive (false);
			} else if (globalScript.currentQuest != 14) {
				//neededObjectives [1].SetActive (false);
			}

			if (globalScript.currentQuest == 12) {
				GameObject sphere = neededObjectives [0];
				sphere.transform.parent.gameObject.SetActive (true);
				sphere.SetActive (true);
				objectivePoint = sphere.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0;

				unalteredPoint = sphere.transform.position;
			} else if (globalScript.currentQuest == 13) {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;

				unalteredPoint = door.transform.position;
			} else if (globalScript.currentQuest == 14) {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;

				unalteredPoint = door.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "innerTempleScene") {
			if (globalScript.currentQuest == 14) {
				GameObject NPC = neededObjectives [0];
				if (globalScript.gameState != "isTalking") {
					globalScript.gameState = "isTalking";
					NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
				}
				found = false;
			} else if (globalScript.currentQuest == 15) {
				GameObject loomToken = neededObjectives [1];
				objectivePoint = loomToken.transform.position;
				objectivePoint.y += 2f;

				unalteredPoint = loomToken.transform.position;
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "darkForestLevel3Scene") {
			if (globalScript.currentQuest != 16) {
				neededObjectives [1].SetActive (false);
			}

			if (globalScript.currentQuest == 16) {
				GameObject sphere = neededObjectives [1];
				sphere.transform.parent.gameObject.SetActive (true);
				sphere.SetActive (true);
				objectivePoint = sphere.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0;

				unalteredPoint = sphere.transform.position;
			} else {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1f;

				unalteredPoint = door.transform.position;
			}
		} else if (SceneManager.GetActiveScene ().name == "darkTownLevel3Scene") {
			if (globalScript.currentQuest == 17) {
				GameObject NPC = neededObjectives [1];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;

				if (globalScript.gameState != "isTalking") {
					GameObject Character = neededObjectives [4];
					if (Character.transform.position.x < NPC.transform.position.x + 5) {
						globalScript.gameState = "isTalking";
						LeanTween.value (0, 1, 0).setOnComplete (() => {
							NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
						});
					}
				}
			} else {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1f;

				unalteredPoint = door.transform.position;

				if (globalScript.currentQuest == 18) {
					if (!neededObjectives [2].activeSelf) {
						neededObjectives [2].SetActive (true);
					}

					if (neededObjectives [1].name == "Elder") {
						if (!neededObjectives [3].transform.GetChild (1).gameObject.activeSelf) {
							neededObjectives [1].name = "ElderDone";
							Animator elderAnim = neededObjectives [1].transform.GetChild (1).GetComponent<Animator> ();
							if (!elderAnim.GetCurrentAnimatorStateInfo (0).IsName ("Haduken2")) {
								elderAnim.Play ("Haduken2", -1, 0f);
							}
							neededObjectives [1].transform.GetChild (1).GetComponent<Anim_GlobalControl> ().flip = true;
							neededObjectives [3].transform.GetChild (1).gameObject.SetActive (true);
							Vector2 currPos = neededObjectives [3].transform.localPosition;
							currPos.x -= 14;
							neededObjectives [3].transform.localPosition = currPos;
						}
					}

					if (!neededObjectives [0].activeSelf) {
						neededObjectives [0].SetActive (true);
					}
				} else {
					if (neededObjectives [2].activeSelf) {
						neededObjectives [2].SetActive (false);
					}
				}
			}
		} else if (SceneManager.GetActiveScene ().name == "forestLevel2Scene") {
			GameObject door = neededObjectives [1];
			objectivePoint = door.transform.position;
			objectivePoint.y += 3f;
			objectivePoint.x += 1f;

			unalteredPoint = door.transform.position;
		} else if (SceneManager.GetActiveScene ().name == "darkForestLevel2Scene") {
			GameObject door = neededObjectives [1];
			objectivePoint = door.transform.position;
			objectivePoint.y += 3f;
			objectivePoint.x += 1f;

			unalteredPoint = door.transform.position;
		} else if (SceneManager.GetActiveScene ().name == "darkForestLevel1Scene") {
			GameObject door = neededObjectives [0];
			objectivePoint = door.transform.position;
			objectivePoint.y += 3f;
			objectivePoint.x += 1f;

			unalteredPoint = door.transform.position;
		} else if (SceneManager.GetActiveScene ().name == "darkTownLevel2Scene") {
			if (globalScript.currentQuest == 18) {
				GameObject NPC = neededObjectives [0];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.15f;

				unalteredPoint = NPC.transform.position;

				if (globalScript.gameState != "isTalking") {
					GameObject Character = neededObjectives [4];
					if (Character.transform.position.x < NPC.transform.position.x + 5) {
						globalScript.gameState = "isTalking";
						LeanTween.value (0, 1, 0).setOnComplete (() => {
							NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
						});
					}
				}
			} else {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1f;

				unalteredPoint = door.transform.position;

				if (globalScript.currentQuest == 19) {
					if (!neededObjectives [2].activeSelf) {
						neededObjectives [2].SetActive (true);
					}
					if (neededObjectives [0].activeSelf) {
						neededObjectives [0].SetActive (false);
					}
					if (!neededObjectives [3].activeSelf) {
						neededObjectives [3].SetActive (true);
					}

					if (!neededObjectives [1].activeSelf) {
						neededObjectives [1].SetActive (true);
					}
				} else {
					if (neededObjectives [2].activeSelf) {
						neededObjectives [2].SetActive (false);
					}
				}
			}
		} else if (SceneManager.GetActiveScene ().name == "darkTownLevel1Scene") {
			GameObject NPC = neededObjectives [0];
			objectivePoint = NPC.transform.position;
			objectivePoint.y += 2f;
			objectivePoint.x += 0.15f;

			unalteredPoint = NPC.transform.position;
		} else {
			found = false;
		}

		if (globalScript.gameState == "isTalking") {
			found = false;
		}

		if (found) {
			pointer.SetActive (true);
			Vector2 currPos = objectivePoint;
			arrow.SetActive (false);

			if (objectivePoint.x > rightBottomSide.x - 1f) {
				currPos.x = rightBottomSide.x - 1f;
				arrow.SetActive (true);
			}

			if (objectivePoint.x < leftTopSide.x + 1f) {
				currPos.x = leftTopSide.x + 1f;
				arrow.SetActive (true);
			}

			if (objectivePoint.y > leftTopSide.y - 1f) {
				currPos.y = leftTopSide.y - 1f;
				arrow.SetActive (true);
			}

			if (objectivePoint.y < rightBottomSide.y + 1f) {
				currPos.y = rightBottomSide.y + 1f;
				arrow.SetActive (true);
			}

			if (unalteredPoint.x > rightBottomSide.x - 1f) {
				arrow.SetActive (true);
			} else if (unalteredPoint.x < leftTopSide.x + 1f) {
				arrow.SetActive (true);
			} else if (unalteredPoint.y > leftTopSide.y - 1f) {
				arrow.SetActive (true);
			} else if (unalteredPoint.y < rightBottomSide.y + 1f) {
				arrow.SetActive (true);
			}

			if (arrow.activeSelf) {
				Vector2 pointerPos = pointer.transform.position;
				Vector2 targetDir = unalteredPoint - pointerPos;
				float angle = Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90;
				arrow.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			}

			pointer.transform.position = currPos;
		} else {
			pointer.SetActive (false);
		}
	}
}
