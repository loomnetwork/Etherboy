using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class goldCounterClass : MonoBehaviour {
	private TextMeshPro thisText;
	private int currentGold;
	// Use this for initialization
	void Start () {
		currentGold = globalScript.currentGold;
		thisText = GetComponent<TextMeshPro> ();
		thisText.text = globalScript.currentGold.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		if (currentGold != globalScript.currentGold) {
			if (currentGold > globalScript.currentGold) {
				currentGold = globalScript.currentGold;
				thisText.text = globalScript.currentGold.ToString ();
			} else {
				currentGold++;
				thisText.text = currentGold.ToString ();
			}
		}	
	}
}
