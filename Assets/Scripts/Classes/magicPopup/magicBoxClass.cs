using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class magicBoxClass : MonoBehaviour, ITouchable {
	public Sprite iconActive;
	public Sprite iconInactive;
	public Sprite panelActive;
	public Sprite panelInactive;

	private TextMesh titleText;
	private TextMeshPro buttonText;
	private SpriteRenderer panelRend;
	private SpriteRenderer iconRend;

	private bool isActive;
	public bool isEquipped;

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
		GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
		GetComponent<AudioSource> ().Play ();
		return false;
	}

	// Update is called once per frame
	public bool TouchMoved (Vector2 touchPosition, bool isInBounds) {
		return false;
	}

	public bool TouchEnded (Vector2 touchPosition) {
		GetComponent<Renderer>().material.color = new Color(1, 1, 1);
		touchController.FocusObject = null;

		if (isActive) {
			for (int i = 0; i < transform.parent.childCount; i++) {
				GameObject c = transform.parent.GetChild (i).gameObject;
				magicBoxClass cScript = c.GetComponent<magicBoxClass> ();
				cScript.isEquipped = false;
				cScript.updateMetaData ();
			}
			isEquipped = true;

			if (transform.name == "box1") {
				globalScript.equippedMagic = "earth";
			} else if (transform.name == "box2") {
				globalScript.equippedMagic = "fire";
			} else if (transform.name == "box3") {
				globalScript.equippedMagic = "ice";
			} else if (transform.name == "box4") {
				globalScript.equippedMagic = "air";
			}

			updateMetaData ();
		}

		return false;
	}

	void Start () {
		titleText = transform.GetChild (0).GetChild (2).GetComponent<TextMesh> ();
		buttonText = transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<TextMeshPro> ();
		iconRend = transform.GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ();
		panelRend = transform.GetChild (0).GetComponent<SpriteRenderer> ();

		int valueToCheck = globalScript.currentQuest;
		if (!SceneManager.GetActiveScene ().name.Contains ("private")) {
			valueToCheck++;
		}
		if (transform.name == "box1") {
			if (valueToCheck > 6) {
				isActive = true;

				if (globalScript.equippedMagic == "earth") {
					isEquipped = true;
				}
			}
		} else if (transform.name == "box2") {
			if (valueToCheck > 10) {
				isActive = true;

				if (globalScript.equippedMagic == "fire") {
					isEquipped = true;
				}
			}
		} else if (transform.name == "box3") {
			if (valueToCheck > 14) {
				isActive = true;

				if (globalScript.equippedMagic == "ice") {
					isEquipped = true;
				}
			}
		} else if (transform.name == "box4") {
			if (valueToCheck > 17) {
				isActive = true;

				if (globalScript.equippedMagic == "air") {
					isEquipped = true;
				}
			}
		}

		updateMetaData ();
	}

	public void updateMetaData () {
		if (isEquipped) {
			buttonText.transform.parent.gameObject.SetActive (true);
			buttonText.text = "Equipped";
			panelRend.sprite = panelActive;
			iconRend.sprite = iconActive;
		} else if (isActive) {
			buttonText.transform.parent.gameObject.SetActive (true);
			buttonText.text = "Select";
			panelRend.sprite = panelInactive;
			iconRend.sprite = iconInactive;
		} else {
			buttonText.transform.parent.gameObject.SetActive (false);
			panelRend.sprite = panelInactive;
			iconRend.sprite = iconInactive;
			gameObject.SetActive (false);
		}
	}
}

