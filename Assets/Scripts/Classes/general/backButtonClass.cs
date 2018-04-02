using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class backButtonClass : MonoBehaviour, ITouchable {

	public string backScene;
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
		GetComponent<AudioSource> ().Play ();
		GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
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
		if (backScene == "usePreviousScene") {
			globalScript.changeScene (globalScript.previousScene);
		} else {
			globalScript.changeScene (backScene);
		}
		return false;
	}
}