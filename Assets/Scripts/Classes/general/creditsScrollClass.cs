using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creditsScrollClass : MonoBehaviour {
	private float scrollSpeed = 0.02f;
	
	// Update is called once per frame
	void Update () {
		Vector2 currPos = transform.position;
		currPos.y += scrollSpeed;
		transform.position = currPos;

		if (currPos.y >= 36.15f) {
			currPos.y = 36.15f;
			transform.position = currPos;
			LeanTween.value (0, 1, 2).setOnComplete (() => {
				globalScript.changeScene("menuScene");
			});
			GetComponent<creditsScrollClass> ().enabled = false;
		}
	}
}
