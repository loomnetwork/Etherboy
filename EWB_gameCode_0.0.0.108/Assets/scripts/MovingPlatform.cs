using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float speed = .5f;
    public float rangeY = 100f;
    public float rangeX = 0f;
    public float offset = 0f;
    private bool increasing = true;
    private Vector2 origin;
    

	private void Start () {
        origin = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
	}
	
    private void FixedUpdate()
    {
        if (rangeX != 0)
        {
            var deltaX = speed * Time.deltaTime * .01f;
            deltaX = increasing ? deltaX : -deltaX;
            
            if (increasing)
            {
                increasing = transform.position.x > origin.x + rangeX;
            }
            else
            {
                increasing = transform.position.x < origin.x - rangeX;
            }
            
            transform.Translate(deltaX, 0, 0);
        }
        else if (rangeY != 0)
        {
            var deltaY = speed * Time.deltaTime * 8.0f;
            deltaY = increasing ? deltaY : -deltaY;
            
            if (increasing)
            {
                if(transform.position.y > origin.y + rangeY)
                {
                    increasing = false;
                }
            }
            else
            {
                if(increasing = transform.position.y < origin.y - rangeY)
                {
                    increasing = true;
                }
            }
            
            transform.Translate(0, deltaY, 0);
        }
    }
}
