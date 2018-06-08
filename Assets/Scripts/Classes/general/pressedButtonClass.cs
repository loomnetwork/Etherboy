using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressedButtonClass : MonoBehaviour, ITouchableMultiTouch {
	public string key;
	public Sprite normalSprite;
	public Sprite pressedSprite;
	public bool useSprites;

	private SpriteRenderer thisRend;

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
		inputBroker.setKey (key, 1);

		if (useSprites) {
			thisRend.sprite = pressedSprite;
		}

		if (transform.name == "attackButton") {
			inputBroker.setKey ("Fire2", 1);
		}
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		//touchController.FocusObject = null;
		transform.localScale = baseScale;
		inputBroker.setKey (key, 0);

		if (useSprites) {
			thisRend.sprite = normalSprite;
		}

		if (transform.name == "attackButton") {
			inputBroker.setKey ("Fire2", 0);
		}
		return false;
	}

	void Start () {
		thisRend = GetComponent<SpriteRenderer> ();
		baseScale = transform.localScale;
	}
}