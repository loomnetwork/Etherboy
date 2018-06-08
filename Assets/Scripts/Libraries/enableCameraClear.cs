using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableCameraClear : MonoBehaviour {
	private void OnEnable () {
		#if UNITY_IOS || UNITY_ANDROID
		// For some reason, on some platforms camera outputs nothing if clear flags are not set
			GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
		#endif
	}
}
