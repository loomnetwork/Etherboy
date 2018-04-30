using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class markerClass : MonoBehaviour {

	public GameObject[] neededObjectives;

	private Vector2 leftTopSide;
	private Vector2 rightBottomSide;

	private Vector2 objectivePoint;
	private Vector2 unalteredPoint;

	private GameObject pointer;
	private GameObject arrow;
	// Use this for initialization
	void Start () {
		leftTopSide = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
		rightBottomSide = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

		pointer = transform.GetChild (0).gameObject;
		arrow = pointer.transform.GetChild (0).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
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
					Steve.SetActive (true);
					Steve.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 1, NPC.transform.localPosition.y - 0.2f);
					globalScript.gameState = "isTalking";
					LeanTween.value (0, 1, 0.5f).setOnComplete (() => {
						NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
					});
				}
				found = false;
			} else if (globalScript.currentQuest == 3) {
				if (neededObjectives [3].activeSelf) {
					neededObjectives [3].SetActive (false);
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
					Steve.SetActive (true);
					Steve.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 1, NPC.transform.localPosition.y - 0.2f);
					globalScript.gameState = "isTalking";
					LeanTween.value (0, 1, 0.5f).setOnComplete (() => {
						NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
					});
				}
				found = false;
			} else if (globalScript.currentQuest == 7) {
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
					globalScript.gameState = "isTalking";
					NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
				}
				found = false;
			} else {
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
					Steve.SetActive (true);
					Steve.transform.localPosition = new Vector2 (NPC.transform.localPosition.x - 1, NPC.transform.localPosition.y - 0.2f);
					globalScript.gameState = "isTalking";
					LeanTween.value (0, 1, 0.5f).setOnComplete (() => {
						NPC.GetComponent<npcSystemClass> ().activateTriggeredManually = true;
					});
				}
				found = false;
			} else if (globalScript.currentQuest == 11) {
				neededObjectives [3].SetActive (false);
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
			} else {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1f;

				unalteredPoint = door.transform.position;
			}
		} else if (SceneManager.GetActiveScene ().name == "forestLevel2Scene") {
			GameObject door = neededObjectives [1];
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
			} else {
				GameObject door = neededObjectives [1];
				objectivePoint = door.transform.position;
				objectivePoint.y += 3f;
				objectivePoint.x += 1f;

				unalteredPoint = door.transform.position;
			}
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
