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
    public enum MouseEditMode
    {
        None,
        BlockEdit,
        EntitySelect
    }

    public enum MouseActionType
    {
        AddBlock,
        RemoveBlock
    }

    public class MouseActionEvent
    {
        public MouseActionEvent(MouseActionType actionType, Vec3Int pos)
        {
            _mouseActionType = actionType;
            _position = pos;
        }

        public MouseActionType _mouseActionType;
        public Vec3Int _position;
    }

    Transform _transform;
    Transform _cameraTrans;

    public Vector2 MouseSensitivity = new Vector2(1f, 1f);

    public Camera _theCamera;

    public float maxX = 90f;
    public float minX = -90f;
    protected float _currentXRotation = 0f;
    protected float _currentYRotation = 0f;

    public Vector3 cameraMoveModifier = Vector3.one;

    public Vec3Int _curTargetBlockPos = Vec3Int.Zero;

    public GameObject _targetCube;

    //protected EntitySpriteGameObject _curHighLightESGO;
    //protected EntitySpriteGameObject _prevHighLightESGO;

    //protected BoundsDrawing _lineBoundsDrawer;

    //public EntitySpriteGameObject CurrentHighlitedESGO { get { return _curHighLightESGO; } }

    protected MouseEditMode _mouseSelectMode;

    public MouseEditMode MouseSelectionMode
    {
        get { return _mouseSelectMode; }
        set { _mouseSelectMode = value; }
    }

    Queue<MouseActionEvent> _pendingMouseActions;

    public MouseActionEvent GetMouseAction()
    {
        if (_pendingMouseActions == null)
            return null;

        if (_pendingMouseActions.Count < 1)
            return null;

        return _pendingMouseActions.Dequeue();
    }

    public void AddMouseAction(MouseActionType actionType, Vec3Int pos)
    {
        Debug.Log(string.Format("Mouse action: {0} Pos: {1}", actionType, pos));
        _pendingMouseActions.Enqueue(new MouseActionEvent(actionType, pos));
    }

    void Start()
    {
        _transform = this.gameObject.transform;
        _theCamera = this.gameObject.GetComponentInChildren<Camera>();

        if (_targetCube == null)
        {
            _targetCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _targetCube.collider.enabled = false;
        }

        _pendingMouseActions = new Queue<MouseActionEvent>();

        _targetCube.transform.localScale = (VoxelBlock.DefaultBlockSize * 1.02f);

        _targetCube.SetActive(false);

        if (_theCamera == null)
        {
            Debug.LogError("Can't find camera for FPS camera");
        }

        _cameraTrans = _theCamera.transform;

        //_lineBoundsDrawer = PoolManager.Singleton.RequestBoundsDrawer();
        //_lineBoundsDrawer.Detach();

        _curTargetBlockPos = Vec3Int.Zero;
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

        //When holding down right click and pressing left click, add a block remove event
        if (_mouseSelectMode == MouseEditMode.BlockEdit)
        {
            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    AddMouseAction(MouseActionType.RemoveBlock, _curTargetBlockPos);
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    AddMouseAction(MouseActionType.AddBlock, _curTargetBlockPos);
                }
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

        if (_mouseSelectMode == MouseEditMode.BlockEdit)
        {
            CheckForBlock();
        }
    }

    void CheckForBlock()
    {
        Vector3 centerPoint = _theCamera.transform.position;
        Vector3 dir = _cameraTrans.transform.forward;

        Vector3 targetPos = centerPoint + (dir * 2.5f);

        targetPos.x = Mathf.Floor(targetPos.x / VoxelBlock.DefaultBlockSize.x);
        targetPos.y = Mathf.Floor(targetPos.y / VoxelBlock.DefaultBlockSize.y);
        targetPos.z = Mathf.Floor(targetPos.z / VoxelBlock.DefaultBlockSize.z);

        targetPos += Vector3.one * 0.5f;

        _curTargetBlockPos.x = Mathf.FloorToInt(targetPos.x);
        _curTargetBlockPos.y = Mathf.FloorToInt(targetPos.y);
        _curTargetBlockPos.z = Mathf.FloorToInt(targetPos.z);

        _targetCube.transform.position = targetPos;
    }

    public void ResetSelectionBlock()
    {
        _targetCube.transform.localScale = Vector3.one;
    }

    void OnPostRender()
    {
        //_lineBoundsDrawer.OnPostRender();
    }
}