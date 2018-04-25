using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopTabButtonClass : MonoBehaviour, ITouchable {
	private GameObject parentBox;
	private SpriteRenderer tabButtonRend;
	private bool mustFocus = true;

	public bool MustFocus {
		get {
			return mustFocus;
		}
		set {
		}
	}

	private Vector3 baseScale;
	private float scale = 0.8f;
	// Use this for initialization
	public bool TouchBegan (Vector2 touchPosition) {
		GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
		GetComponent<AudioSource> ().Play ();
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		if (!isInBounds) {
			GetComponent<Renderer>().material.color = new Color(1, 1, 1);
			touchController.FocusObject = null;
		}
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		touchController.FocusObject = null;

		for (int i = 0; i < parentBox.transform.childCount; i++) {
			GameObject c = transform.GetChild (i).gameObject;
			if (c.name == gameObject.name) {
				c.SetActive (true);
			} else {
				c.SetActive (false);
			}
		}

		return false;
	}

	void Start () {
		parentBox = transform.parent.GetChild (6).gameObject;
		tabButtonRend = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		
	}
}
