using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splashScreenAnyKeyClass : MonoBehaviour, ITouchable {
	public string backScene;
	private bool mustFocus = true;

	public bool MustFocus {
		get {
			return mustFocus;
		}
		set {
		}
	}

	private Vector3 baseScale;
	private float scale = 0.9f;
	// Use this for initialization
	public bool TouchBegan (Vector2 touchPosition) {
		
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {

		touchController.FocusObject = null;
		moveToMenu ();
		return false;
	}

	void moveToMenu () {
		globalScript.changeScene ("menuScene");
		GetComponent<splashScreenAnyKeyClass> ().enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			moveToMenu ();
		}
	}
}
