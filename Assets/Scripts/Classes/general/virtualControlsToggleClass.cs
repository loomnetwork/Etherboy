using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class virtualControlsToggleClass : MonoBehaviour {
	private GameObject character;
	private Collider2D charCollider;

	public SpriteRenderer atkBtnRend;
	public pressedButtonClass atkBtnScript;

	public Sprite atkBtnNormal;
	public Sprite atkBtnDown;
	public Sprite talkBtnNormal;
	public Sprite talkBtnDown;
	public Sprite doorBtnNormal;
	public Sprite doorBtnDown;

	// Use this for initialization
	void Awake () {
		#if !UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR
			gameObject.SetActive(false);
		#else
			SceneManager.sceneLoaded += OnSceneLoaded;
		#endif
	}

	void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
		if (gameObject == null) {
			SceneManager.sceneLoaded -= OnSceneLoaded;
			return;
		}
		character = GameObject.Find ("etherBoy");
		if (character != null) {
			charCollider = character.GetComponent<Collider2D> ();
		}
		if (scene.name == "menuScene" || scene.name == "splashScene" || scene.name == "gameOverScene" || scene.name == "uploadedScene"
			|| scene.name == "townLevel1CutScene" || scene.name == "creditsScene" || scene.name == "privateRoomCutScene" || scene.name == "townLevel1CutScene2") {
			transform.parent.gameObject.SetActive(false);
		} else {
			transform.parent.gameObject.SetActive(true);
		}
	}

	void Update () {
		if (character != null) {
			RaycastHit2D[] hit = Physics2D.RaycastAll (charCollider.bounds.center, Vector2.zero, Mathf.Infinity);

			bool found = false;
			if (hit.Length > 0) {
				for (int i = 0; i < hit.Length; i++) {
					if (hit [i].transform.gameObject.layer == LayerMask.NameToLayer("NPCs")) {
						npcSystemClass npcScript = hit [i].transform.GetComponent<npcSystemClass> ();
						if (npcScript != null) {
							if (npcScript.currentDialogue != null && npcScript.currentDialogue.hasTriggeredDialogue) {
								found = true;
								if (atkBtnRend.sprite != talkBtnNormal || atkBtnRend.sprite != talkBtnDown) {
									atkBtnRend.sprite = talkBtnNormal;
									atkBtnScript.normalSprite = talkBtnNormal;
									atkBtnScript.pressedSprite = talkBtnDown;
									atkBtnScript.key = "Fire3";
								}
							}
						}
					} else if (hit [i].transform.name == "door") {
						found = true;
						if (atkBtnRend.sprite != doorBtnNormal || atkBtnRend.sprite != doorBtnDown) {
							atkBtnRend.sprite = doorBtnNormal;
							atkBtnScript.normalSprite = doorBtnNormal;
							atkBtnScript.pressedSprite = doorBtnDown;
							atkBtnScript.key = "Vertical";
						}
					}
				}
			}

			if (!found) {
				if (atkBtnRend.sprite != atkBtnNormal || atkBtnRend.sprite != atkBtnDown) {
					atkBtnRend.sprite = atkBtnNormal;
					atkBtnScript.normalSprite = atkBtnNormal;
					atkBtnScript.pressedSprite = atkBtnDown;
					atkBtnScript.key = "Fire3";
				}
			}
		}
	}
}
