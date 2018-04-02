using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class globalScript : MonoBehaviour {

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
		globalScript.previousScene = SceneManager.GetActiveScene ().name;

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

	void Awake() {
		print ("DONE");
		if (hasAlreadyLoaded == true)
			return;

		hasAlreadyLoaded = true;
		Application.targetFrameRate = 60;

		currentQuest = 0;

		fader = GameObject.Find ("fader");

		print (fader);
		if (fader != null) {
			Object.DontDestroyOnLoad (fader);
			fader.SetActive (false);
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