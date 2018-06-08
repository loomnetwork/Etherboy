using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class analogControllerClass : MonoBehaviour, ITouchableMultiTouch {
	public GameObject middleAnalog;
	public GameObject innerAnalog;

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
		//innerAnalog.GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
		//transform.localScale = new Vector2 (scale*baseScale.x, scale);

		moveAnalog (touchPosition);
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		moveAnalog (touchPosition);
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		//innerAnalog.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		//transform.localScale = baseScale;

		middleAnalog.transform.localPosition = new Vector2 (0, 0);
		innerAnalog.transform.localPosition = new Vector2 (0, 0);

		inputBroker.setAxis ("Horizontal", 0);
		inputBroker.setAxis ("Vertical", 0);
		return false;
	}

	void moveAnalog (Vector2 touchPosition) {
		Vector2 currPos = Camera.main.ScreenToWorldPoint (touchPosition);

		float angle = Mathf.Atan2 (currPos.x - transform.position.x, currPos.y - transform.position.y);
		float distance = Mathf.Sqrt ((currPos.x - transform.position.x) * (currPos.x - transform.position.x) + (currPos.y - transform.position.y) * (currPos.y - transform.position.y));
		currPos = transform.position;

		currPos.x += Mathf.Sin (angle)*Mathf.Min(distance, 0.5f);
		currPos.y += Mathf.Cos (angle)*Mathf.Min(distance, 0.5f);

		middleAnalog.transform.position = currPos;

		currPos = new Vector2 (0, 0);

		currPos.x += Mathf.Sin (angle)*Mathf.Min(distance, 0.3f);
		currPos.y += Mathf.Cos (angle)*Mathf.Min(distance, 0.3f);

		innerAnalog.transform.localPosition = currPos;

		float angleDeg = angle * Mathf.Rad2Deg + 180;

		if (angleDeg >= 315 || angleDeg < 45) {
			inputBroker.setAxis ("Horizontal", 0);
			inputBroker.setAxis ("Vertical", -1);
		} else if (angleDeg >= 45 && angleDeg < 135) {
			inputBroker.setAxis ("Horizontal", -1);
			inputBroker.setAxis ("Vertical", 0);
		} else if (angleDeg >= 135 && angleDeg < 225) {
			inputBroker.setAxis ("Horizontal", 0);
			inputBroker.setAxis ("Vertical", 1);
		} else if (angleDeg >= 225 && angleDeg < 315) {
			inputBroker.setAxis ("Horizontal", 1);
			inputBroker.setAxis ("Vertical", 0);
		}
	}

	void Start () {
		baseScale = transform.localScale;
	}
}