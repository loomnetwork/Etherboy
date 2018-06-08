using UnityEngine;
using System.Collections;

public class fitTheScreen : MonoBehaviour {
	public bool fitH;
	public bool fitW;

	public bool keepH;
	public bool keepRatio;
	public bool fitOnlyIfLower;

	public bool skipUpdate;

	// Use this for initialization
	void Start () {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		if (sr==null) return;

		float width=sr.sprite.bounds.size.x;
		float height=sr.sprite.bounds.size.y;


		float worldScreenHeight=Camera.main.orthographicSize*2f;
		float worldScreenWidth=worldScreenHeight/Screen.height*Screen.width;

		if (fitW == true) {
			Vector3 xWidth = transform.localScale;
			xWidth.x = worldScreenWidth / width;
			transform.localScale = xWidth;
		}
		if (fitH == true) {
			Vector3 yHeight = transform.localScale;
			yHeight.y = worldScreenHeight / height;

			if (keepRatio) {
				if (yHeight.x > yHeight.y) {
					yHeight.y = yHeight.x;
				} else {
					yHeight.x = yHeight.y;
				}
			} else if (keepH) {
				yHeight.x = yHeight.y;
			}

			transform.localScale = yHeight;
		}
		if (fitOnlyIfLower) {
			if (sr.bounds.size.x < worldScreenWidth) {
				Vector3 xWidth = transform.localScale;
				xWidth.x = worldScreenWidth / width;
				xWidth.y = xWidth.x;
				transform.localScale = xWidth;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!skipUpdate) {
			Start ();
		}
	}
}
