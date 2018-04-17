using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class talkGroupClass : MonoBehaviour {

	private GameObject bottomBar;
	private GameObject topBar;
	private SpriteRenderer bottomBarRend;
	private SpriteRenderer topBarRend;

	private Vector2 leftTopBorder;
	private Vector2 rightBottomBorder;

	private bool isActive;

	// Use this for initialization
	void Start () {
		isActive = false;

		bottomBar = transform.GetChild (0).gameObject;
		topBar = transform.GetChild (1).gameObject;

		bottomBarRend = bottomBar.GetComponent<SpriteRenderer> ();
		topBarRend = topBar.GetComponent<SpriteRenderer> ();

		leftTopBorder = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));
		rightBottomBorder = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));

		bottomBar.transform.position = new Vector2 (0, rightBottomBorder.y - bottomBarRend.bounds.extents.y);
		topBar.transform.position = new Vector2 (0, leftTopBorder.y + topBarRend.bounds.extents.y);
	}
	
	// Update is called once per frame
	void Update () {
		if (globalScript.gameState == "isTalking") {
			if (!isActive) {
				isActive = true;
				LeanTween.cancel (bottomBar);
				LeanTween.cancel (topBar);

				LeanTween.moveY (bottomBar, rightBottomBorder.y + bottomBarRend.bounds.extents.y, 0.25f);
				LeanTween.moveY (topBar, leftTopBorder.y - topBarRend.bounds.extents.y, 0.25f);
			}
		} else {
			if (isActive) {
				isActive = false;
				LeanTween.cancel (bottomBar);
				LeanTween.cancel (topBar);

				LeanTween.moveY (bottomBar, rightBottomBorder.y - bottomBarRend.bounds.extents.y, 0.25f);
				LeanTween.moveY (topBar, leftTopBorder.y + topBarRend.bounds.extents.y, 0.25f);
			}
		}
	}
}
