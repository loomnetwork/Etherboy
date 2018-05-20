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
				setAsPurchased ();
				updateMetaData ();

				TouchEnded (touchPosition);
			}
		} else {
			if (transform.parent.name == "tabSword") {
				globalScript.equippedSword = objectName;
				globalScript.currentWeapon = "sword";
			} else if (transform.parent.name == "tabBow") {
				globalScript.equippedBow = objectName;
				globalScript.currentWeapon = "bow";
			} else if (transform.parent.name == "tabHelm") {
				globalScript.equippedHelm = objectName;
			}

			for (int i = 0; i < transform.parent.childCount-1; i++) {
				GameObject c = transform.parent.GetChild (i).gameObject;
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
		bool bought = false;

		if (objectName == "bow1") {
			if (globalScript.bow1Purchased) {
				bought = true;
			}
		} else if (objectName == "bow2") {
			if (globalScript.bow2Purchased) {
				bought = true;
			}
		} else if (objectName == "bow3") {
			if (globalScript.bow3Purchased) {
				bought = true;
			}
		} else if (objectName == "bow4") {
			if (globalScript.bow4Purchased) {
				bought = true;
			}
		} else if (objectName == "bow5") {
			if (globalScript.bow5Purchased) {
				bought = true;
			}
		} else if (objectName == "sword1") {
			if (globalScript.sword1Purchased) {
				bought = true;
			}
		} else if (objectName == "sword2") {
			if (globalScript.sword2Purchased) {
				bought = true;
			}
		} else if (objectName == "sword3") {
			if (globalScript.sword3Purchased) {
				bought = true;
			}
		} else if (objectName == "sword4") {
			if (globalScript.sword4Purchased) {
				bought = true;
			}
		} else if (objectName == "sword5") {
			if (globalScript.sword5Purchased) {
				bought = true;
			}
		} else if (objectName == "helm1") {
			if (globalScript.helm1Purchased) {
				bought = true;
			}
		} else if (objectName == "helm2") {
			if (globalScript.helm2Purchased) {
				bought = true;
			}
		} else if (objectName == "helm3") {
			if (globalScript.helm3Purchased) {
				bought = true;
			}
		} else if (objectName == "helm4") {
			if (globalScript.helm4Purchased) {
				bought = true;
			}
		} else if (objectName == "helm5") {
			if (globalScript.helm5Purchased) {
				bought = true;
			}
		}

		if (bought) {
			ownedText.gameObject.SetActive (true);
			costGroup.SetActive (false);
			isPurchasable = false;

			if (transform.parent.name == "tabSword") {
				if (globalScript.equippedSword == objectName) {
					ownedText.text = "EQUIPPED";
					selectText.transform.parent.gameObject.SetActive (false);
				} else {
					selectText.transform.parent.gameObject.SetActive (true);
					ownedText.text = "OWNED";
					selectText.text = "Select";
				}
			} else if (transform.parent.name == "tabBow") {
				if (globalScript.equippedBow == objectName) {
					ownedText.text = "EQUIPPED";
					selectText.transform.parent.gameObject.SetActive (false);
				} else {
					selectText.transform.parent.gameObject.SetActive (true);
					ownedText.text = "OWNED";
					selectText.text = "Select";
				}
			} else if (transform.parent.name == "tabHelm") {
				if (globalScript.equippedHelm == objectName) {
					ownedText.text = "EQUIPPED";
					selectText.transform.parent.gameObject.SetActive (false);
				} else {
					selectText.transform.parent.gameObject.SetActive (true);
					ownedText.text = "OWNED";
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

	void setAsPurchased () {
		if (objectName == "bow1") {
			globalScript.bow1Purchased = true;
		} else if (objectName == "bow2") {
			globalScript.bow2Purchased = true;
		} else if (objectName == "bow3") {
			globalScript.bow3Purchased = true;
		} else if (objectName == "bow4") {
			globalScript.bow4Purchased = true;
		} else if (objectName == "bow5") {
			globalScript.bow5Purchased = true;
		} else if (objectName == "sword1") {
			globalScript.sword1Purchased = true;
		} else if (objectName == "sword2") {
			globalScript.sword2Purchased = true;
		} else if (objectName == "sword3") {
			globalScript.sword3Purchased = true;
		} else if (objectName == "sword4") {
			globalScript.sword4Purchased = true;
		} else if (objectName == "sword5") {
			globalScript.sword5Purchased = true;
		} else if (objectName == "helm1") {
			globalScript.helm1Purchased = true;
		} else if (objectName == "helm2") {
			globalScript.helm2Purchased = true;
		} else if (objectName == "helm3") {
			globalScript.helm3Purchased = true;
		} else if (objectName == "helm4") {
			globalScript.helm4Purchased = true;
		} else if (objectName == "helm5") {
			globalScript.helm5Purchased = true;
		}

		globalScript.saveGame ();
	}
}
