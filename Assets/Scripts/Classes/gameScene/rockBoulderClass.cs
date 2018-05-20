using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockBoulderClass : MonoBehaviour {

	private AudioSource audioSFX;
	private GameObject[] stones;
	float timer = 0;

	void Start () {
		audioSFX = gameObject.AddComponent<AudioSource> ();
		audioSFX.clip = Resources.Load<AudioClip> ("SFX/Etherboy/EBW_AUD_Etherboy_Haduken_Earth_Impact_F1_EXP");
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if (timer > 3.5f) {
			for (int i = 0; i < stones.Length; i++) {
				if (stones [i] != null) {
					LeanTween.alpha (stones [i], 0, 0.25f).destroyOnComplete = true;
				}
			}
			stones = null;
			LeanTween.alpha (gameObject, 0, 0.25f).destroyOnComplete = true;
			GetComponent<rockBoulderClass> ().enabled = false;
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		GetComponent<Collider2D> ().enabled = false;
		GetComponent<SpriteRenderer> ().enabled = false;
		Rigidbody2D thisBody = GetComponent<Rigidbody2D> ();
		stones = new GameObject[6];
		audioSFX.Play ();
		for (int i = transform.childCount-1; i >= 0; i--) {
			GameObject c = transform.GetChild (i).gameObject;
			stones [i] = c;
			c.transform.parent = transform.parent;
			c.SetActive (true);
			c.GetComponent<Collider2D> ().enabled = true;
			c.GetComponent<Rigidbody2D> ().velocity = new Vector2 (Random.Range (thisBody.velocity.x*0.4f, thisBody.velocity.x), Random.Range (5f, 20f));
		}
	}
}
