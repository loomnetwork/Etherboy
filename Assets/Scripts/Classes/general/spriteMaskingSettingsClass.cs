using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteMaskingSettingsClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
