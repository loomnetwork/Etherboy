using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundMovementClass : MonoBehaviour {
	public GameObject toFollow;
	public float offsetY;

	private Vector2 basePos;
	private Vector2 leftBottomScreen;
	private SpriteRenderer thisRend;

	// Use this for initialization
	void Start () {
		basePos = transform.position;
		leftBottomScreen = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));
		thisRend = GetComponent<SpriteRenderer> ();
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 currPos = toFollow.transform.position;
		currPos.y *= 0.8f;
		currPos.y += offsetY;
		if (currPos.y - thisRend.bounds.extents.y > leftBottomScreen.y) {
			currPos.y = leftBottomScreen.y + thisRend.bounds.extents.y;
		}
		currPos.x = 0;
		transform.position = currPos;
	}
}
