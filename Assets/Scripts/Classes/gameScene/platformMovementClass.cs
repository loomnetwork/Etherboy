using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformMovementClass : MonoBehaviour {
	public float topLimit;
	public float bottomLimit;
	public float leftLimit;
	public float rightLimit;

	public float initialSpeedY;
	public float initialSpeedX;

	private float topLimitReal;
	private float bottomLimitReal;
	private float rightLimitReal;
	private float leftLimitReal;

	private float currentSpeedY;
	private float currentSpeedX;

	// Use this for initialization
	void Start () {
		currentSpeedY = initialSpeedY;
		currentSpeedX = initialSpeedX;

		topLimitReal = transform.localPosition.y + topLimit;
		bottomLimitReal = transform.localPosition.y + bottomLimit;

		rightLimitReal = transform.localPosition.x + rightLimit;
		leftLimitReal = transform.localPosition.x + leftLimit;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 currPos = transform.localPosition;
		currPos.y += currentSpeedY;
		currPos.x += currentSpeedX;

		if (currentSpeedY > 0) {
			if (currPos.y > topLimitReal) {
				currPos.y = topLimitReal;
				currentSpeedY = -Mathf.Abs (initialSpeedY);
			}
		} else if (currentSpeedY < 0) {
			if (currPos.y < bottomLimitReal) {
				currPos.y = bottomLimitReal;
				currentSpeedY = Mathf.Abs (initialSpeedY);
			}
		}

		if (currentSpeedX > 0) {
			if (currPos.x > rightLimitReal) {
				currPos.x = rightLimitReal;
				currentSpeedX = -Mathf.Abs (initialSpeedX);
			}
		} else if (currentSpeedX < 0) {
			if (currPos.x < leftLimitReal) {
				currPos.x = leftLimitReal;
				currentSpeedX = Mathf.Abs (initialSpeedX);
			}
		}

		transform.localPosition = currPos;
	}
}
