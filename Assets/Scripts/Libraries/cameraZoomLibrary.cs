using UnityEngine;
using System.Collections;

public class cameraZoomLibrary : MonoBehaviour {
	private Camera thisCamera;

	void Start () {
		thisCamera = Camera.main;
	}

	public void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
	{
		float multiplier = (1.0f / thisCamera.orthographicSize * amount);

		transform.position += (zoomTowards - transform.position) * multiplier; 

		thisCamera.orthographicSize -= amount;

	//	thisCamera.orthographicSize = Mathf.Clamp(thisCamera.orthographicSize, minZoom, maxZoom);
	}
}
