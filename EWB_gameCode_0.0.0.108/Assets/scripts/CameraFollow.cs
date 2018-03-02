using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -1);
//    public bool useOffset = true;
    public bool useSlerp = true;
    public bool useX = true;
    public bool useY = true;

    public Vector3 minPoint = new Vector3(10.73f, 8, -1f);
    public Vector3 maxPoint = new Vector3(161.4f, 19.3f, -1f);

    [Range(0.1f, 1.0f)]
    public float bufferSpeed = 0.125f;

    public bool toggleOff = false;
    

    public void ToggleOff(bool stopFollow = true)
    {
        toggleOff = stopFollow;
    }
    
    void FixedUpdate()
    {
        if (!toggleOff)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 bufferedPosition = Vector3.Slerp(transform.position, desiredPosition, bufferSpeed);

            // IN ACCEPTABLE RANGE
            if (useSlerp)
            {
                if (bufferedPosition.x >= minPoint.x && bufferedPosition.y >= minPoint.y && bufferedPosition.x <= maxPoint.x && bufferedPosition.y <= maxPoint.y)
                {
                    if (useX && useY)
                    {
                        transform.position = bufferedPosition;
                    }
                    else if (useX)
                    {
                        transform.position = new Vector3(bufferedPosition.x, transform.position.y, transform.position.z);
                    }
                    else if (useY)
                    {
                        transform.position = new Vector3(transform.position.x, bufferedPosition.y, transform.position.z);
                    }
                }
            }
            else
            {
                if (useX && useY)
                {
                    transform.position = desiredPosition; ;
                }
                else if (useX)
                {
                    transform.position = new Vector3(desiredPosition.x, transform.position.y, transform.position.z);
                }
                else if (useY)
                {
                    transform.position = new Vector3(transform.position.x, desiredPosition.y, transform.position.z);
                }
            }
        }
    }
}
