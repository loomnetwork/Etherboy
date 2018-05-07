using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class purchaseBoxClass : MonoBehaviour, ITouchable {
	private string objectName;
	private TextMeshPro selectText;
	private GameObject costGroup;
	private int cost;
	private TextMeshPro ownedText;
	private bool mustFocus = true;
	private bool isPurchasable;

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

		if (isPurchasable) {
			if (globalScript.currentGold >= cost) {
				globalScript.currentGold -= cost;
				PlayerPrefs.SetString (objectName, "purchased");
				updateMetaData ();
			}
		} else {
			if (transform.parent.name == "tabSword") {
				globalScript.equippedSword = objectName;
			} else if (transform.parent.name == "tabBow") {
				globalScript.equippedBow = objectName;
			} else if (transform.parent.name == "tabHelm") {
				globalScript.equippedHelm = objectName;
			}

			for (int i = 0; i < transform.parent.childCount; i++) {
				GameObject c = transform.GetChild (i).gameObject;
				c.GetComponent<purchaseBoxClass> ().updateMetaData ();
			}
		}
	
		return false;
	}

	void Start () {
		selectText = transform.GetChild (2).GetChild (0).GetComponent<TextMeshPro> ();
		costGroup = transform.GetChild (4).gameObject;
		cost = int.Parse(costGroup.transform.GetChild(1).name);
		ownedText = transform.GetChild (5).GetComponent<TextMeshPro> ();
		objectName = transform.GetChild (0).name;
		updateMetaData ();
	}

	public void updateMetaData () {
		if (PlayerPrefs.GetString (objectName) != "") {
			ownedText.gameObject.SetActive (true);
			costGroup.SetActive (false);
			isPurchasable = false;

			if (transform.parent.name == "tabSword") {
				if (globalScript.equippedSword == objectName) {
					ownedText.text = "EQUIPPED";
					selectText.transform.parent.gameObject.SetActive (false);
				} else {
					selectText.transform.parent.gameObject.SetActive (true);
					ownedText.text = "owned";
					selectText.text = "Select";
				}
			} else if (transform.parent.name == "tabBow") {
				if (globalScript.equippedBow == objectName) {
					ownedText.text = "EQUIPPED";
					selectText.transform.parent.gameObject.SetActive (false);
				} else {
					selectText.transform.parent.gameObject.SetActive (true);
					ownedText.text = "owned";
					selectText.text = "Select";
				}
			} else if (transform.parent.name == "tabHelm") {
				if (globalScript.equippedHelm == objectName) {
					ownedText.text = "EQUIPPED";
					selectText.transform.parent.gameObject.SetActive (false);
				} else {
					selectText.transform.parent.gameObject.SetActive (true);
					ownedText.text = "owned";
					selectText.text = "Select";
				}
			}
		} else {
			selectText.transform.parent.gameObject.SetActive (true);
			costGroup.SetActive (true);
			ownedText.gameObject.SetActive (false);
			selectText.text = "Purchase";
			isPurchasable = true;
		}
	}
}
