using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class fanClass : MonoBehaviour {
	private List<Rigidbody2D> objects;
	// Use this for initialization
	void Start () {
		objects = new List<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < objects.Count; i++) {
			objects [i].velocity = new Vector2 (objects [i].velocity.x, 6f);
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (!collider.isTrigger && collider.GetComponent<Rigidbody2D> ()) {
			objects.Add (collider.GetComponent<Rigidbody2D> ());
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (!collider.isTrigger && collider.GetComponent<Rigidbody2D> ()) {
			objects.Remove (collider.GetComponent<Rigidbody2D> ());
		}
	}
}
