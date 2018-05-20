using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class continueButtonClass : MonoBehaviour, ITouchable {

	public string nextScene;
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
		GetComponent<AudioSource> ().Play ();
		GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
		transform.localScale = new Vector2 (scale, scale);

		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		if (!isInBounds) {
			GetComponent<Renderer>().material.color = new Color(1, 1, 1);
			transform.localScale = baseScale;
			touchController.FocusObject = null;
		}
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		touchController.FocusObject = null;
		transform.localScale = baseScale;
		if (!globalScript.useBackend) {
			globalScript.chaosHitEtherboyOnce = false;
			globalScript.loadGame (new SampleState ());
		} else {
			globalScript.chaosHitEtherboyOnce = false;
			GameObject.Find ("backend").GetComponent<backendClass> ().QuerySaveData ();
		}
		return false;
	}

	void Start () {
		baseScale = transform.localScale;

		if (!globalScript.useBackend) {
			if (PlayerPrefs.GetString ("savedData") == "") {
				GetComponent<BoxCollider2D> ().enabled = false;
				GetComponent<Renderer> ().material.color = new Color (0.7f, 0.7f, 0.7f);
			}
		}
	}
}