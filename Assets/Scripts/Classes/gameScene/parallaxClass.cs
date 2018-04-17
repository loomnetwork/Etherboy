using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxClass : MonoBehaviour {
	public float parallaxValue;
	public GameObject objectToFollow;

	// Update is called once per frame
	void Update () {
		Vector2 currPos = objectToFollow.transform.position;
		currPos.x *= parallaxValue;
		currPos.y = transform.position.y;
		transform.position = currPos;
	}
}
