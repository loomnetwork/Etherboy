using UnityEngine;
using System.Collections;

public class alignScript : MonoBehaviour {
	public float alignX = 0;
	public float alignY = 0;
	public float alignAddWidth = 0;
	public float alignAddHeight = 0;
	public float offsetX = 0;
	public float offsetY = 0;
	public bool skipX;
	public bool skipY;

	// Use this for initialization
	void Awake () {
		float posX = Screen.width*alignX;
		float posY = Screen.height*alignY;
		float distance = transform.position.z-Camera.main.transform.position.z;
		Renderer rend = this.GetComponent<Renderer>();

		Vector3 screenPosition = new Vector3(posX, posY, distance);
		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

		if (skipX == false) {
			worldPosition.x = worldPosition.x + (rend.bounds.size.x * alignAddWidth) + offsetX;
		} else {
			worldPosition.x = transform.position.x;
		}
		if (skipY == false) {
			worldPosition.y = worldPosition.y + (rend.bounds.size.y * alignAddHeight) + offsetY;
		} else {
			worldPosition.y = transform.position.y;
		}
		worldPosition.z = 0;

		transform.localPosition = worldPosition;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
