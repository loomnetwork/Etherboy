using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0f);

    public bool useSlerp = true;
    public bool useX = true;
    public bool useY = true;

    public Vector3 minPoint = new Vector3(10.73f, 8, -3f);
    public Vector3 maxPoint = new Vector3(161.4f, 19.3f, -3f);

    [Range(0.1f, 1.0f)]
    public float bufferSpeed = 0.125f;

    public bool toggleOff = false;

    public bool shaking = false;
    public float shakeTimer;
    public float shakeAmount;


    public void ToggleOff(bool stopFollow = true)
    {
        toggleOff = stopFollow;
    }
    
    /*
     * ATTACHED TO CAMERA OBJECT, FOLLOWS PLAYERS x/y WITH INITIAL OFFSET BASED ON INSPECTOR (SCENE) SETTINGS
     */
    void FixedUpdate()
    {
        if (!toggleOff)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition = new Vector3(desiredPosition.x, desiredPosition.y, -3f);

            Vector3 bufferedPosition = Vector3.Slerp(transform.position, desiredPosition, bufferSpeed);
            bufferedPosition = new Vector3(bufferedPosition.x, bufferedPosition.y, -3f);

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
                        transform.position = new Vector3(bufferedPosition.x, transform.position.y, offset.z);
                    }
                    else if (useY)
                    {
                        transform.position = new Vector3(transform.position.x, bufferedPosition.y, offset.z);
                    }
                }
            }
            else
            {
                if (useX && useY)
                {
                    transform.position = desiredPosition;
                }
                else if (useX)
                {
                    transform.position = new Vector3(desiredPosition.x, transform.position.y, offset.z);
                }
                else if (useY)
                {
                    transform.position = new Vector3(transform.position.x, desiredPosition.y, offset.z);
                }
            }
        }
    }



    void Update()
    {
        if (shaking)
        {
            if (shakeTimer >= 0)
            {
                Vector2 shakePosition = Random.insideUnitCircle * shakeAmount;

                shakeTimer -= Time.deltaTime;

                transform.position = new Vector3(transform.position.x + shakePosition.x, transform.position.y + shakePosition.y, -3f);
            }
            else
            {
                Vector3 desiredPosition = target.position + offset;
                desiredPosition = new Vector3(desiredPosition.x, desiredPosition.y, -3f);
                

                transform.position = new Vector3(desiredPosition.x, transform.position.y, offset.z);
                shaking = false;

                GameManager.instance.player.SendMessage("EndShake", SendMessageOptions.RequireReceiver);
            }
        }
    }

    /*
     * USED IN THE TAVERN SCENE TO CAUSE THE EARTHQUAKE
     */
    public void ShakeCamera(float shakePower, float shakeDuration)
    {
        shakeTimer = shakeDuration;
        shakeAmount = shakePower;

        shaking = true;
    }
}
