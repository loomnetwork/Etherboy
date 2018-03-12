using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MoveDirections
{
    /*
     * OBJECT CLASS TO BE PASSED IN FUNCTION CALLS
     */
    public GameObject originObject;
    public GameObject targetObject;
    public GameObject callbackObject;
    public string callbackFunction;
    public string[] parameters;

    public MoveDirections(GameObject _originObect, GameObject _targetObject, GameObject _callbackObject = null, string _callbackFunction = null, string[] _parameters = null)
    {
        originObject = _originObect;
        targetObject = _targetObject;
        callbackObject = _callbackObject;
        callbackFunction = _callbackFunction;
        parameters = _parameters;
    }
}
