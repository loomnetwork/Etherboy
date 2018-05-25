using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class inputBroker : MonoBehaviour {
	static Hashtable myHashtable = new Hashtable();

	public static void setKey (string key, int value) {
		if (myHashtable.Contains (key)) {
			myHashtable [key] = value;
		} else {
			myHashtable.Add (key, value);
		}
	}

	public static void setAxis (string key, float value) {
		if (myHashtable.Contains (key)) {
			myHashtable [key] = value;
		} else {
			myHashtable.Add (key, value);
		}
	}

	public static bool GetKeyDown (string key) {
		return Input.GetKeyDown (key) || (myHashtable.Contains (key) && int.Parse(myHashtable[key].ToString()) == 1);
	}

	public static float GetAxis (string axis) {
		float value = 0;

		if (myHashtable.Contains (axis)) {
			value = int.Parse(myHashtable [axis].ToString());
		} else {
			value = Input.GetAxis (axis);
		}
		return value;
	}

	public static bool GetButton (string key) {
		bool found = false;
		#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
			found = (myHashtable.Contains (key) && int.Parse(myHashtable[key].ToString()) == 1);
		#else
			found = Input.GetButton (key) || (myHashtable.Contains (key) && int.Parse(myHashtable[key].ToString()) == 1);
		#endif
		return found;
	}

	public static bool GetButtonDown (string key) {
		bool found = false;
		if (myHashtable.Contains (key)) {
			if (int.Parse (myHashtable [key].ToString ()) == 1) {
				found = true;
				myHashtable [key] = 0;
			}
		}
		return Input.GetButtonDown (key) || found;
	}
}
