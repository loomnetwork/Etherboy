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
				GameObject door = neededObjectives [2];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.7f;

				unalteredPoint = door.transform.position;
				unalteredPoint.x += 10;
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
			} else {
				found = false;
			}
		} else if (SceneManager.GetActiveScene ().name == "forestLevel1Scene") {
			if (globalScript.currentQuest == 0) {
				GameObject door = neededObjectives [0];
				objectivePoint = door.transform.position;
				objectivePoint.y += 2f;
				objectivePoint.x += 0.75f;

				unalteredPoint = door.transform.position;
				unalteredPoint.x -= 10;
			} else if (globalScript.currentQuest == 1) {
				GameObject NPC = neededObjectives [0];
				objectivePoint = NPC.transform.position;
				objectivePoint.y += 2.15f;
				objectivePoint.x += 0f;

				unalteredPoint = NPC.transform.position;
				unalteredPoint.x -= 10;
			} else {
				found = false;
			}
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
				currPos.y = leftTopSide.y + 1f;
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
