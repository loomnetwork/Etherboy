using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopTabButtonClass : MonoBehaviour, ITouchable {
	public Sprite activeButton;
	public Sprite activeIcon;
	public Sprite inactiveButton;
	public Sprite inactiveIcon;

	private GameObject parentBox;
	private SpriteRenderer tabButtonRend;
	private SpriteRenderer iconRend;
	private bool mustFocus = true;
	private GameObject ownTab;
	private bool status;

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
			GameObject c = parentBox.transform.GetChild (i).gameObject;
			if (c.name == gameObject.name) {
				c.SetActive (true);
			} else {
				c.SetActive (false);
			}
		}

		return false;
	}

	void Start () {
		status = false;
		parentBox = transform.parent.GetChild (6).gameObject;
		tabButtonRend = GetComponent<SpriteRenderer> ();
		iconRend = transform.GetChild (0).GetComponent<SpriteRenderer> ();

		for (int i = 0; i < parentBox.transform.childCount; i++) {
			GameObject c = parentBox.transform.GetChild (i).gameObject;
			if (c.name == gameObject.name) {
				ownTab = c;
			}
		}
	}

	void Update () {
		if (ownTab.activeSelf) {
			if (status == false) {
				status = true;
				tabButtonRend.sprite = activeButton;
				iconRend.sprite = activeIcon;
			}
		} else {
			if (status) {
				status = false;
				tabButtonRend.sprite = inactiveButton;
				iconRend.sprite = inactiveIcon;
			}
		}
	}
}
