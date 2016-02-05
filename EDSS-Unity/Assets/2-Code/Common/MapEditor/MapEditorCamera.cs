//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// MapEditorCamera - Camera script for use with MapEditor, within Unity Editor
// Created: Febuary 4 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: Febuary 4 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

[ExecuteInEditMode]
public class MapEditorCamera : MonoBehaviour
{
    Transform _transform;
    Transform _cameraTrans;

    public Vector2 MouseSensitivity = new Vector2(1f, 1f);

    public Camera _theCamera;

    public float maxX = 90f;
    public float minX = -90f;
    protected float _currentXRotation = 0f;
    protected float _currentYRotation = 0f;

    public Vector3 cameraMoveModifier = Vector3.one;

    //protected EntitySpriteGameObject _curHighLightESGO;
    //protected EntitySpriteGameObject _prevHighLightESGO;

    //protected BoundsDrawing _lineBoundsDrawer;

    //public EntitySpriteGameObject CurrentHighlitedESGO { get { return _curHighLightESGO; } }

    void Start()
    {
        _transform = this.gameObject.transform;
        _theCamera = this.gameObject.GetComponentInChildren<Camera>();

        if (_theCamera == null)
        {
            Debug.LogError("Can't find camera for FPS camera");
        }

        _cameraTrans = _theCamera.transform;

        //_lineBoundsDrawer = PoolManager.Singleton.RequestBoundsDrawer();
        //_lineBoundsDrawer.Detach();
    }

    void Update()
    {
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 cameraMousePos = _theCamera.ScreenToViewportPoint(mousePos);

        //Debug.Log("Mousepos " + mousePos + " cameramousepos " + cameraMousePos);

        //if (cameraMousePos.x < 0f || cameraMousePos.x > 1f || cameraMousePos.y < 0f || cameraMousePos.y > 1f)
        //{
        //    return;
        //}

        if (Input.GetMouseButton(1))
        {
            if (Screen.showCursor)
            {
                Screen.showCursor = false;
            }

            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            //Debug.Log("X " + xAxis + " Y " + yAxis);

            _currentXRotation -= (yAxis * MouseSensitivity.y);
            _currentXRotation = Mathf.Clamp(_currentXRotation, minX, maxX);

            _currentYRotation += (xAxis * MouseSensitivity.x);

            //_transform.Rotate(0, xAxis * MouseSensitivity.x, 0);

            _cameraTrans.localRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0f);
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (!Screen.showCursor)
            {
                Screen.showCursor = true;
            }
        }

        bool shiftDown = Input.GetKey(KeyCode.LeftShift);
        Vector3 moveDelta = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDelta.z += (1f * cameraMoveModifier.z);
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDelta.z -= (1f * cameraMoveModifier.z);
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDelta.x -= (1f * cameraMoveModifier.x);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDelta.x += (1f * cameraMoveModifier.x);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            moveDelta.y += (1f * cameraMoveModifier.y);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDelta.y -= (1f * cameraMoveModifier.y);
        }

        if (shiftDown)
        {
            _cameraTrans.Translate(moveDelta, Space.World);
        }
        else
        {
            _cameraTrans.Translate(moveDelta, Space.Self);
        }

        //MouseCrossHair();
    }

    //void MouseCrossHair()
    //{
    //    Vector3 centerPoint = _cameraTrans.transform.position;
    //    Vector3 dir = _cameraTrans.transform.forward;

    //    int layer = 1 << ClientGameManager.ClientTriggerLayer;

    //    RaycastHit hitInfo;

    //    bool hit = Physics.Raycast(centerPoint, dir, out hitInfo, 10f, layer);

    //    if (hit)
    //    {
    //        //Debug.Log("Hit " + hitInfo.collider.gameObject.transform.root.name);

    //        _curHighLightESGO = hitInfo.collider.gameObject.transform.root.gameObject.GetComponent<EntitySpriteGameObject>();

    //        if (_curHighLightESGO == null)
    //        {
    //            return;
    //        }

    //        if (_curHighLightESGO != _prevHighLightESGO)
    //        {
    //            _curHighLightESGO.Highlight();
    //            _lineBoundsDrawer.DrawCube(_curHighLightESGO._transform.position, _curHighLightESGO.Cubecollider.BoxSize);
    //            _lineBoundsDrawer.Enable();

    //            if (_prevHighLightESGO != null)
    //            {
    //                _prevHighLightESGO.DeHighlight();
    //            }
    //        }

    //        _prevHighLightESGO = _curHighLightESGO;
    //    }
    //    else if (_prevHighLightESGO != null)
    //    {
    //        _lineBoundsDrawer.Disable();
    //        _prevHighLightESGO.DeHighlight();
    //        _prevHighLightESGO = null;
    //    }
    //}

    void OnPostRender()
    {
        //_lineBoundsDrawer.OnPostRender();
    }
}