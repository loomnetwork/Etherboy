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
	private float scale = 0.9f;
	// Use this for initialization
	public bool TouchBegan (Vector2 touchPosition) {
		GetComponent<AudioSource> ().Play ();
		GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
		transform.localScale = new Vector2 (scale, scale);

		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		if (!isInBounds) {
			GetComponent<Renderer>().material.color = new Color(1, 1, 1);
			transform.localScale = baseScale;
			touchController.FocusObject = null;
		}
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		touchController.FocusObject = null;
		transform.localScale = baseScale;
		if (transform.name == "newGameButton") {
			globalScript.currentGold = 0;
			globalScript.currentQuest = 0;
			globalScript.currentWeapon = "sword";
			globalScript.questStep = 0;
			globalScript.lastPlayedScene = "";
			globalScript.equippedBow = "bow1";
			globalScript.equippedHelm = "";
			globalScript.equippedSword = "sword1";
			globalScript.equippedMagic = "earth";
			globalScript.previousScene = "";
			globalScript.bow1Purchased = true;
			globalScript.bow2Purchased = false;
			globalScript.bow3Purchased = false;
			globalScript.bow4Purchased = false;
			globalScript.bow5Purchased = false;
			globalScript.helm1Purchased = false;
			globalScript.helm2Purchased = false;
			globalScript.helm3Purchased = false;
			globalScript.helm4Purchased = false;
			globalScript.helm5Purchased = false;
			globalScript.sword1Purchased = true;
			globalScript.sword2Purchased = false;
			globalScript.sword3Purchased = false;
			globalScript.sword4Purchased = false;
			globalScript.sword5Purchased = false;
			globalScript.chaosHitEtherboyOnce = false;
		}
		if (backScene == "usePreviousScene") {
			globalScript.changeScene (globalScript.previousScene);
		} else {
			globalScript.changeScene (backScene);
		}
		return false;
	}

	void Start () {
		baseScale = transform.localScale;
	}
}