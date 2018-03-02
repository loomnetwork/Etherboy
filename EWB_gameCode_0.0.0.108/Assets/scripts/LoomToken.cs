using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoomToken : MonoBehaviour {
    
	void Start ()
    {
		if(GameManager.instance.eventsManager.CheckEvent("got_loom_token"))
        {
            Destroy(gameObject);
        }
	}
}
