using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpBarClass : MonoBehaviour {
	public characterClass characterScript;

	private int baseLife;
	private int currentLife;
	private float rechargeTime;

	// Use this for initialization
	void Start () {
		baseLife = 100;
		currentLife = baseLife;
		rechargeTime = 0;
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
		if (characterScript == null) {
			characterScript = GameObject.Find ("etherBoy").GetComponent<characterClass> ();
			characterScript.life = currentLife;
		}
		Vector2 currScale = transform.localScale;
		currScale.x = (float)characterScript.life / 100f;
		transform.localScale = currScale;

		if (characterScript.life != baseLife) {
			rechargeTime += Time.deltaTime;
			if (rechargeTime > 5) {
				characterScript.life += 1;
			}
		} else {
			rechargeTime = 0;
		}

		currentLife = characterScript.life;
	}
}
