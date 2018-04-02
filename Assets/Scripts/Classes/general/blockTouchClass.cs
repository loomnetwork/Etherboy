using UnityEngine;
using System.Collections;

public class blockTouchClass : MonoBehaviour, ITouchable {

	private bool mustFocus = true;

	public bool MustFocus {
		get {
			return mustFocus;
		}
		set {
		}
	}

	// Use this for initialization
	public bool TouchBegan (Vector2 touchPosition) {
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		return false;
	}
}
