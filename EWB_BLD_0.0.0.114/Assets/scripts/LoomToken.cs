using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoomToken : MonoBehaviour {
    
    /*
     * DESTROYS LOOM TOKEN BASED ON EVENT CHECK.  ONLY LIVES ON THE LOOM TOKEN BY THE FOREST SHRINE
     */
	void Start ()
    {
		if(GameManager.instance.eventsManager.CheckEvent("got_loom_token"))
        {
            Destroy(gameObject);
        }
	}
}
