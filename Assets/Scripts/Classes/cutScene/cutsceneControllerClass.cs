using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cutsceneControllerClass : MonoBehaviour {
	public Animator globalControl;

	// Use this for initialization
	void Awake () {
		if (SceneManager.GetActiveScene ().name == "townLevel1CutScene") {
			globalControl.Play ("01_Sky", -1, 0f);
			LeanTween.value (0, 1, 10.5f).setOnComplete (() => {
				globalScript.changeScene ("privateRoomCutScene");
			});
		} else if (SceneManager.GetActiveScene ().name == "privateRoomCutScene") {
			globalControl.Play ("03_SleepingEtherboy", -1, 0f);
			LeanTween.value (0, 1, 10.5f).setOnComplete (() => {
				globalControl.Play ("04_CloseUp_Etherboy", -1, 0f);
				LeanTween.value (0, 1, 6.3f).setOnComplete (() => {
					globalControl.Play ("05_Dressing", -1, 0f);
					LeanTween.value (0, 1, 9.45f).setOnComplete (() => {
						globalScript.changeScene ("townLevel1CutScene2");
					});
				});
			});
		} else if (SceneManager.GetActiveScene ().name == "townLevel1CutScene2") {
			globalControl.Play ("06_Ready", -1, 0f);
			LeanTween.value (0, 1, 6f).setOnComplete (() => {
				SceneManager.LoadScene("townLevel1Scene");
			});
		}
	}
}
