//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EDSSFirstPersonCamera - This is the first person camera used by players
// Created: December 7 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 7 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class EDSSFirstPersonCamera : MonoBehaviour
{
    Transform _transform;
    Transform _cameraTrans;

    public Vector2 MouseSensitivity = new Vector2(1f, 1f);

    public Camera _theCamera;

    public float maxX = 90f;
    public float minX = -90f;
    protected float _currentRotation = 0f;

    void Start()
    {
        _transform = this.gameObject.transform;
        _theCamera = this.gameObject.GetComponentInChildren<Camera>();

        if (_theCamera == null)
        {
            Debug.LogError("Can't find camera for FPS camera");
        }

        _cameraTrans = _theCamera.transform;
    }

    void Update()
    {
        float xAxis = Input.GetAxis("Mouse X");
        float yAxis = Input.GetAxis("Mouse Y");

        _currentRotation -= (yAxis * MouseSensitivity.y);
        _currentRotation = Mathf.Clamp(_currentRotation, minX, maxX);

        _transform.Rotate(0, xAxis * MouseSensitivity.x, 0);

        _cameraTrans.localRotation = Quaternion.Euler(_currentRotation, 0f, 0f);
    }
}