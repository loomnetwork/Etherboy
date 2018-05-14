using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowShopClass : MonoBehaviour, ITouchable {
	public GameObject middleBox;
	public int direction;
	private bool isPressed;
	private GameObject activeGroup;
	private SpriteRenderer bounds;
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
		transform.localScale = new Vector2 (baseScale.x*scale, baseScale.y*scale);
		for (int i = 0; i < middleBox.transform.childCount; i++) {
			GameObject c = middleBox.transform.GetChild (i).gameObject;
			if (c.activeSelf) {
				activeGroup = c;
				break;
			}
		}
		bounds = activeGroup.transform.GetChild (activeGroup.transform.childCount - 1).GetComponent<SpriteRenderer> ();
		isPressed = true;
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		isPressed = false;
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		touchController.FocusObject = null;
		transform.localScale = baseScale;
		return false;
	}

	void Update () {
		if (isPressed) {
			GameObject activeGroup = null;

			for (int i = 0; i < middleBox.transform.childCount; i++) {
				GameObject c = middleBox.transform.GetChild (i).gameObject;
				if (c.activeSelf) {
					activeGroup = c;
					break;
				}
			}

			if (activeGroup != null) {
				Vector2 currPos = activeGroup.transform.localPosition;
				currPos.x += direction * 0.1f;

				activeGroup.transform.localPosition = currPos;

				if (direction < 0) {
					if (bounds.transform.position.x + bounds.bounds.extents.x < 4.5f) {
						currPos.x = 4.5f - bounds.bounds.extents.x - bounds.transform.localPosition.x;
						activeGroup.transform.localPosition = currPos;
					}
				} else {
					if (bounds.transform.position.x - bounds.bounds.extents.x > -4.5f) {
						currPos.x = -4.5f + bounds.bounds.extents.x - bounds.transform.localPosition.x;
						activeGroup.transform.localPosition = currPos;
					}
				}
			}
		}
	}

	void Start () {
		baseScale = transform.localScale;
	}
}
