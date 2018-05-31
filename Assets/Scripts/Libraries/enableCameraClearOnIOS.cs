using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableCameraClearOnIOS : MonoBehaviour {
	private void OnEnable () {
#if UNITY_IOS
	    // For some reason, on iOS camera outputs nothing if clear flags are not set
	    GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
#endif
	}
}
