using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinDropClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody2D> ().velocity = new Vector2(Random.Range (-6f, 6f), Random.Range (6f, 9f));
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.collider.name == "etherBoy") {
			Physics2D.IgnoreCollision (collision.collider, GetComponent<Collider2D>());
			globalScript.currentGold++;
			Destroy (gameObject);
		}
	}
}
