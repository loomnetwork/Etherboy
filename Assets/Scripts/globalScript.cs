using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SampleState {
	public int currentQuest;
	public string currentWeapon;
	public string equippedSword;
	public string equippedBow;
	public string equippedHelm;
	public int questStep;
	public int currentGold;
	public string lastScene;
}

public class globalScript : MonoBehaviour {
	public static bool useBackend = true;

	public string androidMoreGamesURL;
	public string iosMoreGamesURL;
	public string androidDemoButtonURL;
	public string iOSDemoButtonURL;

	public string unlockAllIapAndroid;
	public string unlockAllIapIOS;
       
    public bool removeAds;
	public bool removeIAP;
	public bool resetsSavedData;

	public static string moreGamesMarketURL = "";
	public static string demoButtonURL = "";

	public static string unlockAlliAP;

	public static bool hasAlreadyLoaded;


	public static string previousScene;
	public static string selectionHandle;

	public static int currentQuest;
	public static int questStep;
	public static string currentWeapon = "sword";
	public static string equippedSword = "sword1";
	public static string equippedBow = "bow1";
	public static string equippedHelm = "helm1";
	public static string gameState;

	public static int currentGold;

	public static int startingOrderEnemies;

	public static float shakeScreenTime;
	public static GameObject groupToShake;
	public static Vector2 basePositionGroupToShake;

	public static string lastPlayedScene;
	public static string sceneBeforeDeath;


	private static GameObject fader;


	public static void giveCoins () {
		int currentCoins = PlayerPrefs.GetInt ("currentCoins");
		currentCoins += 1000;
		PlayerPrefs.SetInt ("currentCoins", currentCoins);
	}

	public static void showFullScreenAd () {
		print ("SHOWING AD");
		bool noAds = false;
		if (PlayerPrefs.GetInt ("isAdsRemoved") == 1) {
			noAds = true;
		}
		if (!noAds) {
			if (Application.isMobilePlatform) {
			/*	if (ApplovinAds.HasInterstitial()) {
					ApplovinAds.ShowInterstital ();
				} else {
					Admob.ShowInterstitial();
				} */
			}
		}
	}

	public static void showVideoAd () {
		bool noAds = false;
		if (!noAds) {
			if (Application.isMobilePlatform) {
			//	ApplovinAds.ShowRewardedVideo (giveCoins);
			}
		}
	}

	public static void changeScene(string sceneName) {
		if (sceneName == "gameOverScene") {
			globalScript.sceneBeforeDeath = globalScript.previousScene;
		}

		if (SceneManager.GetActiveScene ().name != "gameOverScene") {
			globalScript.previousScene = SceneManager.GetActiveScene ().name;
		} else {
			globalScript.previousScene = globalScript.sceneBeforeDeath;
		}

		startingOrderEnemies = 0;

		if (fader != null) {
			fader.SetActive (true);
			LeanTween.alpha (fader, 1, 0.25f).setOnComplete (() => {
				SceneManager.LoadScene (sceneName);
				LeanTween.alpha(fader, 0, 0.25f).setOnComplete (() => {
					fader.SetActive(false);
				});
			});
		} else {
			SceneManager.LoadScene (sceneName);
		}
	}

	public static void loadGame (SampleState saveData) {
		if (!useBackend) {
			saveData = JsonUtility.FromJson<SampleState> (PlayerPrefs.GetString ("savedData"));
		}
		if (saveData != null) {
			globalScript.currentGold = saveData.currentGold;
			globalScript.currentQuest = saveData.currentQuest;
			globalScript.currentWeapon = saveData.currentWeapon;
			globalScript.questStep = saveData.questStep;
			globalScript.lastPlayedScene = saveData.lastScene;

			if (globalScript.lastPlayedScene != "") {
				globalScript.changeScene (globalScript.lastPlayedScene);
			}
		}
	}

	public static void saveGame () {
		SampleState saveData = new SampleState ();
		saveData.currentGold = globalScript.currentGold;
		saveData.currentQuest = globalScript.currentQuest;
		saveData.currentWeapon = globalScript.currentWeapon;
		saveData.questStep = globalScript.questStep;
		saveData.lastScene = SceneManager.GetActiveScene ().name;

		if (!useBackend) {
			string jsonData = JsonUtility.ToJson (saveData);
			if (jsonData != null) {
				PlayerPrefs.SetString ("savedData", jsonData);
			}
		} else {
			GameObject.Find ("backend").GetComponent<backendClass> ().SaveState (saveData);
		}
	}

	public static void fadeToBlack (float seconds) {
		if (fader != null) {
			fader.SetActive (true);
			LeanTween.alpha (fader, 1, 0.25f).setOnComplete (() => {
				globalScript.saveGame();
				LeanTween.alpha (fader, 0, 0.25f).setOnComplete (() => {
					fader.SetActive (false);
				}).setDelay(seconds-0.5f);
			});
		}
	}

	void Update () {
		if (shakeScreenTime > 0) {
			print (shakeScreenTime);
			shakeScreenTime -= Time.deltaTime;
			groupToShake.transform.position = new Vector2 (Random.Range (basePositionGroupToShake.x-0.5f, basePositionGroupToShake.x+0.5f), Random.Range (basePositionGroupToShake.y-0.5f, basePositionGroupToShake.y+0.5f));
			if (shakeScreenTime <= 0) {
				groupToShake.transform.position = basePositionGroupToShake;
				shakeScreenTime = 0;
			}
		}

		if (!useBackend) {
			GameObject loginGroup = GameObject.Find ("loginGroup");
			if (loginGroup != null) {
				loginGroup = loginGroup.transform.GetChild (0).gameObject;
				if (loginGroup.activeSelf) {
					loginGroup.SetActive (false);
				}
			}
		} else {
			if (SceneManager.GetActiveScene ().name == "menuScene") {
				if (PlayerPrefs.GetString ("identityString") != "") {
					GameObject loginGroup = GameObject.Find ("loginGroup");
					if (loginGroup != null) {
						loginGroup = loginGroup.transform.GetChild (0).gameObject;
						if (loginGroup.activeSelf) {
							loginGroup.SetActive (false);
							GameObject.Find ("backend").GetComponent<backendClass> ().SignIn ();
							GameObject menuGroup = GameObject.Find ("menuGroup");
							if (menuGroup != null) {
								menuGroup = menuGroup.transform.GetChild (0).gameObject;
								if (!menuGroup.activeSelf) {
									menuGroup.SetActive (true);
								}
							}
						}
					}
				} else {
					GameObject menuGroup = GameObject.Find ("menuGroup");
					if (menuGroup != null) {
						menuGroup = menuGroup.transform.GetChild (0).gameObject;
						if (menuGroup.activeSelf) {
							menuGroup.SetActive (false);
							GameObject loginGroup = GameObject.Find ("loginGroup");
							if (loginGroup != null) {
								loginGroup = loginGroup.transform.GetChild (0).gameObject;
								if (!loginGroup.activeSelf) {
									loginGroup.SetActive (true);
								}
							}
						}
					}
				}
			}
		}
	}

	void Awake() {
		#if UNITY_EDITOR
			useBackend = false;
		#endif

		if (SceneManager.GetActiveScene ().name == "menuScene") {
			Update ();
		}

		if (hasAlreadyLoaded == true )
			return;

		shakeScreenTime = 0;
		currentGold = 0;
		gameState = "";
		hasAlreadyLoaded = true;
		Application.targetFrameRate = 60;

		PlayerPrefs.SetString ("sword1", "purchased");
		PlayerPrefs.SetString ("bow1", "purchased");

		currentQuest = 0;
		questStep = 0;

		fader = GameObject.Find ("fader");

		if (fader != null) {
			Object.DontDestroyOnLoad (fader);
			fader.SetActive (false);
		}

		GameObject userInterface = GameObject.Find ("UserInterface");

		if (userInterface != null) {
			Object.DontDestroyOnLoad (userInterface);
		}

		GameObject talkGroup = GameObject.Find ("talkGroup");

		if (talkGroup != null) {
			Object.DontDestroyOnLoad (talkGroup);
		}

		unlockAlliAP = unlockAllIapAndroid;
        
        if (removeAds == true) {
			PlayerPrefs.SetInt ("isAdsRemoved", 1);
		}

		if (removeIAP == true) {
			PlayerPrefs.SetInt ("isAllUnlocked", 1);
		//	PlayerPrefs.SetInt ("isAdsRemoved", 1);
		}

		if (resetsSavedData == true) {
			PlayerPrefs.DeleteAll ();
		}

		if (PlayerPrefs.GetInt ("firstTime") == 0) {
			PlayerPrefs.SetInt ("firstTime", 1);
			PlayerPrefs.SetInt ("defenseTeamZombie", 1);
		}

		#if UNITY_IOS
			moreGamesMarketURL = iosMoreGamesURL;
			demoButtonURL = iOSDemoButtonURL;
			unlockAlliAP = unlockAllIapIOS;
		#endif
		#if UNITY_ANDROID
			moreGamesMarketURL = androidMoreGamesURL;
			demoButtonURL = androidDemoButtonURL;
			unlockAlliAP = unlockAllIapAndroid;
		#endif
	}
}