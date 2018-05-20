using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpBarClass : MonoBehaviour {
	public characterClass characterScript;

	private int baseLife;
	private int currentLife;
	private float rechargeTime;

	private Color healthColor = new Color(61f/255f, 191f/255f, 35f/255f);
	private Color lowColor = new Color(232f/255f, 35f/255f, 35f/255f);

	private SpriteRenderer thisRend;

	// Use this for initialization
	void Start () {
		thisRend = GetComponent<SpriteRenderer> ();
		thisRend.color = healthColor;
		baseLife = 100;
		currentLife = baseLife;
		rechargeTime = 0;
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
		if (characterScript == null) {
			GameObject charScriptGameObj = GameObject.Find ("etherBoy");
			if (charScriptGameObj != null) {
				characterScript = charScriptGameObj.GetComponent<characterClass> ();
				characterScript.life = currentLife;
			}
		}

		if (characterScript != null) {
			Vector2 currScale = transform.localScale;
			currScale.x = (float)characterScript.life / 100f;
			transform.localScale = currScale;

			if (characterScript.life <= 10) {
				thisRend.color = lowColor;
			} else {
				thisRend.color = healthColor;
			}

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
}
