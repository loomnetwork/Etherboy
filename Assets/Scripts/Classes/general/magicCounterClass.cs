using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicCounterClass : MonoBehaviour {
	public Sprite earthActive;
	public Sprite earthInactive;
	public Sprite fireActive;
	public Sprite fireInactive;
	public Sprite iceActive;
	public Sprite iceInactive;
	public Sprite airActive;
	public Sprite airInactive;

	private SpriteRenderer thisRend;
	private string lastMagic;
#if UNITY_ANDROID || UNITY_IOS
	private Collider2D thisCollider;
#endif

	// Use this for initialization
	void Start () {
		lastMagic = globalScript.equippedMagic;
		thisRend = GetComponent<SpriteRenderer> ();

		#if UNITY_ANDROID || UNITY_IOS
			if (transform.name == "magicTimer") {
				gameObject.SetActive(false);
			} else {
				thisCollider = GetComponent<Collider2D>();
			}
		#endif
	}

	// Update is called once per frame
	void Update () {
		if (globalScript.currentQuest > 6) {
#if UNITY_ANDROID || UNITY_IOS
		    if (!thisCollider.enabled) {
		        thisCollider.enabled = true;
		    }
#endif
			if (globalScript.magicTimer <= 0) {
				if (globalScript.equippedMagic == "earth") {
					thisRend.sprite = earthActive;
				} else if (globalScript.equippedMagic == "fire") {
					thisRend.sprite = fireActive;
				} else if (globalScript.equippedMagic == "ice") {
					thisRend.sprite = iceActive;
				} else if (globalScript.equippedMagic == "air") {
					thisRend.sprite = airActive;
				}
			} else {
				globalScript.magicTimer -= Time.deltaTime;
				if (globalScript.equippedMagic == "earth") {
					thisRend.sprite = earthInactive;
				} else if (globalScript.equippedMagic == "fire") {
					thisRend.sprite = fireInactive;
				} else if (globalScript.equippedMagic == "ice") {
					thisRend.sprite = iceInactive;
				} else if (globalScript.equippedMagic == "air") {
					thisRend.sprite = airInactive;
				}
			}
		} else {
			thisRend.sprite = null;
#if UNITY_ANDROID || UNITY_IOS
		    if (thisCollider != null) {
		        if (thisCollider.enabled) {
		            thisCollider.enabled = false;
		        }
		    }
#endif
		}
	}
}
