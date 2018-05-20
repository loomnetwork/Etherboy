using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressedArrowButtonClass : MonoBehaviour, ITouchableMultiTouch {
	public string key;
	public float value;
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
		transform.localScale = new Vector2 (scale*baseScale.x, scale);
		inputBroker.setAxis (key, value);
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
		inputBroker.setAxis (key, 0);
		return false;
	}

	void Start () {
		baseScale = transform.localScale;
	}
}
