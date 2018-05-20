using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chaosFollowingClass : MonoBehaviour {
	private float speedX;
	private GameObject character;
	private string state;
	private GameObject startPoint;
	private GameObject hitPoint;
	private float timer;
	// Use this for initialization
	void Start () {
		state = "waitingToStart";
		speedX = 0.035f;
		character = GameObject.Find ("etherBoy");
		timer = 0;

		GetComponent<npcSystemClass> ().enabled = false;

		hitPoint = GameObject.Find ("etherboyFlyingPoint");
		startPoint = GameObject.Find ("chaosStartPoint");
	}
	
	// Update is called once per frame
	void Update () {
		if (state == "follow") {
			Vector2 currPos = transform.position;
			currPos.x -= speedX;
			currPos.y = character.transform.position.y + 1.5f;
			transform.position = currPos;

			if (currPos.x < character.transform.position.x + 3.5f) {
				GetComponent<npcSystemClass> ().enabled = false;
				LeanTween.value(0, 1, 0.1f).setOnComplete(()=>{
					transform.GetChild (0).gameObject.SetActive (false);
				});

				if (globalScript.chaosHitEtherboyOnce) {
					globalScript.chaosHitEtherboyOnce = false;
					character.GetComponent<characterClass> ().enabled = false;
					character.GetComponent<BoxCollider2D> ().enabled = false;
					for (int i = character.transform.childCount - 1; i >= 0; i--) {
						GameObject c = character.transform.GetChild (i).gameObject;
						DestroyImmediate (c);
					}
					transform.GetChild (1).gameObject.SetActive (false);
					transform.GetChild (2).gameObject.SetActive (true);
					state = "caught";
					timer = 0;
				} else {
					state = "hit";
					globalScript.chaosHitEtherboyOnce = true;
					transform.GetChild (1).GetComponent<Animator> ().Play ("Chaos_Swings_Scythe", -1, 0f);
					Collider2D charCollider = character.transform.GetComponent<Collider2D> ();
					character.transform.GetChild (1).gameObject.SetActive (true);
					character.transform.GetChild (2).gameObject.SetActive (false);
					character.transform.GetChild (3).gameObject.SetActive (false);
					charCollider.enabled = false;
					character.GetComponent<Rigidbody2D> ().gravityScale = 0;
					character.GetComponent<characterClass> ().state = "flying";
					character.transform.GetChild (1).GetComponent<Animator> ().Play ("Idle", -1, 0);
					LeanTween.moveLocal (character, hitPoint.transform.localPosition, 1).setDelay (0.7f).setOnStart (() => {
						GetComponent<npcSystemClass> ().enabled = false;
						transform.GetChild (0).gameObject.SetActive (false);
						character.transform.GetChild (1).GetComponent<Animator> ().Play ("Hit", -1, 0);
						transform.GetChild (1).GetComponent<Animator> ().Play ("Chaos_Walk_Hover", -1, 0f);
					}).setOnComplete (() => {
						GetComponent<npcSystemClass> ().enabled = true;
						character.GetComponent<Rigidbody2D> ().gravityScale = 1;

						transform.GetChild (1).GetComponent<Animator> ().Play ("Chaos_Walk_Hover", -1, 0f);
						charCollider.enabled = true;

						character.GetComponent<Rigidbody2D> ().gravityScale = 1;
						character.GetComponent<Rigidbody2D> ().velocity = new Vector2(0, 0);
						character.GetComponent<characterClass> ().state = "normal";
						character.GetComponent<characterClass> ().resetState();
						character.transform.GetChild (1).GetComponent<Animator> ().Play ("Idle", -1, 0f);
						state = "follow";
					});
				}
			}
		} else if (state == "caught") {
			timer += Time.deltaTime;
			if (timer > 3) {
				transform.GetChild (2).GetComponent<Animator> ().Play ("Chaos_Throws_Etherboy", -1, 0f);
				state = "done";
				timer = 0;
			}
		} else if (state == "done") {
			timer += Time.deltaTime;

			if (timer > 2) {
				state = "finished";
				globalScript.changeScene ("gameOverScene");
			}
		} else if (state == "waitingToStart") {
			if (character.transform.position.x <= startPoint.transform.position.x) {
				GetComponent<npcSystemClass> ().enabled = true;
				transform.GetChild (1).gameObject.SetActive (true);
				state = "follow";
			}
		}
	}
}
