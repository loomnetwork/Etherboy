using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponStandClass : MonoBehaviour {
	private SpriteRenderer thisRend;

	// Use this for initialization
	void Start () {
		if (globalScript.currentQuest <= 3) {
			transform.parent.gameObject.SetActive (false);
		}
		thisRend = GetComponent<SpriteRenderer> ();
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.name == "sword") {
			if (thisRend.sprite.name != globalScript.equippedSword) {
				thisRend.sprite = Resources.Load<Sprite> (globalScript.equippedSword);
			}
		} else if (gameObject.name == "bow") {
			if (thisRend.sprite.name != globalScript.equippedBow) {
				thisRend.sprite = Resources.Load<Sprite> (globalScript.equippedBow);
			}
		}

		if (globalScript.currentWeapon == transform.name) {
			if (thisRend.enabled) {
				thisRend.enabled = false;
				for (int i = 0; i < transform.childCount; i++) {
					transform.GetChild (i).gameObject.SetActive (false);
				}
			}
		} else {
			if (!thisRend.enabled) {
				thisRend.enabled = true;
				for (int i = 0; i < transform.childCount; i++) {
					transform.GetChild (i).gameObject.SetActive (true);
				}
			}
		}
	}
}
