using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class goldCounterClass : MonoBehaviour {
	private TextMeshPro thisText;

	// Use this for initialization
	void Start () {
		thisText = GetComponent<TextMeshPro> ();
	}
	
	// Update is called once per frame
	void Update () {
		thisText.text = globalScript.currentGold.ToString ();
	}
}
