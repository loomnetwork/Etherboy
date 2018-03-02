using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiroController : MonoBehaviour {

    public Sprite sitting;
    public Sprite standing;

    public SpriteRenderer spriteRenderer;
    
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!GameManager.instance.eventsManager.CheckEvent("hiro_standing"))
        {
            spriteRenderer.sprite = sitting;
        }
	}
	
	// Update is called once per frame
	void Update () {
	}
}
