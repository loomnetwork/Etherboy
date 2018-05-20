using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class objectiveSystemClass : MonoBehaviour {
	public GameObject goldIndicator;
	public int activeQuest;
	public string status;

	private string[] objectiveNames = {
		"Talk to the town elder",
		"Talk to Hiro in the town INN",
		"Talk to the town elder again",
		"Talk to the Shop keeper",
		"Reach the Forbidden Forest",
		"Reach the Merkle Tree",
		"Bring ORB to the ELDER in Littletown",
		"Get to your bed at the Inn",
		"Speak with the Elder",
		"Find the Fire Merkle Tree",
		"Get the Orb to the Elder in Stonetown",
		"Speak with the Elder in Stonetown",
		"Reach the Merkle Tree",
		"Reach the temple of NODE",
		"Bring the Water orb to the Elder",
		"Take the LOOM TOKEN",
		"Get to the Merkle Tree",
		"Bring the Orb to the Elder in Stonetown",
		"Reach BOB at Bricktown",
		"Save the People"
	};

	private TextMeshPro thisTextMesh;
	private GameObject objMarker;
	private GameObject redFlag;

	private Vector2 basePositionMarker;
	private Vector2 basePositionText;
	private Vector2 basePositionFlag;

	private Vector2 baseScaleMarker;
	private Vector2 baseScaleText;
	private Vector2 baseScaleFlag;

	// Use this for initialization
	void Start () {
		status = "idle";
		activeQuest = -1;
		thisTextMesh = transform.GetChild (1).GetComponent<TextMeshPro> ();
		objMarker = transform.GetChild (1).GetChild(0).gameObject;
		redFlag = transform.GetChild (0).gameObject;

		basePositionMarker = objMarker.transform.localPosition;
		basePositionText = thisTextMesh.transform.localPosition;
		basePositionFlag = redFlag.transform.localPosition;

		baseScaleMarker = objMarker.transform.localScale;
		baseScaleText = thisTextMesh.transform.localScale;
		baseScaleFlag = redFlag.transform.localScale;
	}

	public void showNewObjective () {
		thisTextMesh.gameObject.SetActive (true);
		objMarker.SetActive (true);
		redFlag.SetActive (true);

		thisTextMesh.text = objectiveNames [globalScript.currentQuest];
		thisTextMesh.transform.localScale = baseScaleText;
		thisTextMesh.transform.localPosition = basePositionText;

		objMarker.transform.localScale = baseScaleMarker;
		objMarker.transform.localPosition = basePositionMarker;

		redFlag.transform.localScale = baseScaleFlag;
		redFlag.transform.localPosition = basePositionFlag;

		thisTextMesh.alpha = 0;
		objMarker.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0f);
		redFlag.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0f);

		LeanTween.alpha (thisTextMesh.gameObject, 1, 0.25f).setOnUpdate ((value) => {
			thisTextMesh.alpha = value;
		});
		LeanTween.alpha (objMarker, 1, 0.25f);
		LeanTween.alpha (redFlag, 1, 0.25f);

		LeanTween.value (0, 1, 3).setOnComplete (moveObjectiveToRight);

		activeQuest = globalScript.currentQuest;
	}

	public void moveObjectiveToRight () {
		LeanTween.alpha(redFlag, 0, 0.25f);

		LeanTween.scale (thisTextMesh.gameObject, new Vector2 (0.7f, 0.7f), 1);
		Vector2 currPos = goldIndicator.transform.position;
		currPos.y -= 1;
		currPos.x += goldIndicator.GetComponent<SpriteRenderer> ().bounds.extents.x;
		currPos.x -= thisTextMesh.bounds.extents.x;
		LeanTween.move (thisTextMesh.gameObject, currPos, 1);

		//currPos.x -= (thisTextMesh.bounds.extents.x * 0.7f) + 0.5f;
		LeanTween.scale (objMarker, new Vector2 (0.6f, 0.6f), 1);
		//LeanTween.move (objMarker, currPos, 1);
	}

	public void showRewardScreen () {
		globalScript.gameState = "paused";
		status = "reward";

		GameObject rewardScreen = transform.parent.GetChild (1).gameObject;
		rewardScreen.SetActive (true);

		for (int i = 1; i < rewardScreen.transform.childCount; i++) {
			rewardScreen.transform.GetChild (i).GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
			LeanTween.alpha (rewardScreen.transform.GetChild (i).gameObject, 1, 0.25f);
		}

		LeanTween.value (0, 1, 3).setOnComplete (() => {
			for (int i = 1; i < rewardScreen.transform.childCount; i++) {
				LeanTween.alpha (rewardScreen.transform.GetChild (i).gameObject, 0, 0.25f);
			}

			LeanTween.value(0, 1, 0.25f).setOnComplete(()=>{
				rewardScreen.SetActive(false);
				status = "idle";
				globalScript.gameState = "";
			});
		});

	}
}
