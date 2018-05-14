using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chaosFollowingClass : MonoBehaviour {
	private float speedX;
	private GameObject character;
	private string state;
	private float timer;
	// Use this for initialization
	void Start () {
		state = "follow";
		speedX = 0.035f;
		character = GameObject.Find ("etherBoy");
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == "follow") {
			Vector2 currPos = transform.position;
			currPos.x -= speedX;
			currPos.y = character.transform.position.y + 1.5f;
			transform.position = currPos;

			if (currPos.x < character.transform.position.x + 3.5f) {
				character.GetComponent<characterClass> ().enabled = false;
				character.GetComponent<BoxCollider2D> ().enabled = false;
				for (int i = character.transform.childCount-1; i >= 0; i--) {
					GameObject c = character.transform.GetChild (i).gameObject;
					DestroyImmediate (c);
				}
				transform.GetChild (1).gameObject.SetActive (false);
				transform.GetChild (2).gameObject.SetActive (true);
				state = "caught";
				timer = 0;
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
		}
	}
}
