using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class virtualControlsToggleClass : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		#if !UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR
			gameObject.SetActive(false);
		#else
			SceneManager.sceneLoaded += OnSceneLoaded;
		#endif
	}

	void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
		if (scene.name == "menuScene" || scene.name == "splashScene" || scene.name == "gameOverScene" || scene.name == "uploadedScene") {
			gameObject.SetActive(false);
		} else {
			gameObject.SetActive(true);
		}
	}
}
