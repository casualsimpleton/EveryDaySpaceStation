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
        EntitySelect,
        FaceEdit
    }

    public enum MouseActionType
    {
        AddBlock,
        RemoveBlock,
        PaintFace,
        ClearFace
    }

    public class MouseActionEvent
    {
        public MouseActionEvent(MouseActionType actionType, Vec3Int pos)
        {
            _mouseActionType = actionType;
            _position = pos;
        }

        public MouseActionEvent(MouseActionType actionType, Vec3Int pos, GameManifestV2.BlockDataTemplate.ShowFaceDirection face)
        {
            _mouseActionType = actionType;
            _position = pos;
            _face = face;
        }

        public MouseActionType _mouseActionType;
        public Vec3Int _position;
        public GameManifestV2.BlockDataTemplate.ShowFaceDirection _face;
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
    public GameManifestV2.BlockDataTemplate.ShowFaceDirection _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceZMinus;

    public GameObject _targetCube;
    public GameObject _targetPlane;

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

    public void AddMouseAction(MouseActionType actionType, Vec3Int pos, GameManifestV2.BlockDataTemplate.ShowFaceDirection face)
    {
        Debug.Log(string.Format("Mouse action: {0} Pos: {1} Face: {2}", actionType, pos, face));
        _pendingMouseActions.Enqueue(new MouseActionEvent(actionType, pos, face));
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

        if (_targetPlane == null)
        {
            _targetPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _targetPlane.collider.enabled = false;
        }

        _pendingMouseActions = new Queue<MouseActionEvent>();

        _targetCube.transform.localScale = (VoxelBlock.DefaultBlockSize * 1.02f);
        _targetPlane.transform.localScale = (VoxelBlock.DefaultBlockSize * 1.02f);

        _targetCube.SetActive(false);
        _targetPlane.SetActive(false);

        if (_theCamera == null)
        {
            Debug.LogError("Can't find camera for FPS camera");
        }

        _cameraTrans = _theCamera.transform;

        //_lineBoundsDrawer = PoolManager.Singleton.RequestBoundsDrawer();
        //_lineBoundsDrawer.Detach();

        _curTargetBlockPos = Vec3Int.Zero;
        _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceZMinus;
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
                    //Remove block
                    AddMouseAction(MouseActionType.RemoveBlock, _curTargetBlockPos);
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    //Add block
                    AddMouseAction(MouseActionType.AddBlock, _curTargetBlockPos);
                }
            }
        }

        if (_mouseSelectMode == MouseEditMode.FaceEdit)
        {
            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    //Clear face
                    AddMouseAction(MouseActionType.ClearFace, _curTargetBlockPos, _curTargetFace);
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    //Paint face
                    AddMouseAction(MouseActionType.PaintFace, _curTargetBlockPos, _curTargetFace);
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
        else if (_mouseSelectMode == MouseEditMode.FaceEdit)
        {
            //CheckForBlock();
            CalculateFace();
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

    void CheckForBlock(Vector3 targetPos)
    {
        targetPos.x = Mathf.Floor(targetPos.x / VoxelBlock.DefaultBlockSize.x);
        targetPos.y = Mathf.Floor(targetPos.y / VoxelBlock.DefaultBlockSize.y);
        targetPos.z = Mathf.Floor(targetPos.z / VoxelBlock.DefaultBlockSize.z);

        targetPos += Vector3.one * 0.5f;

        _curTargetBlockPos.x = Mathf.FloorToInt(targetPos.x);
        _curTargetBlockPos.y = Mathf.FloorToInt(targetPos.y);
        _curTargetBlockPos.z = Mathf.FloorToInt(targetPos.z);

        _targetCube.transform.position = targetPos;
    }

    void CalculateFace()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Ray camRay = _theCamera.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        bool didHit = Physics.Raycast(camRay, out hitInfo, 15f);

        Vector3 facePlanePos = Vector3.zero;
        Vector3 facePlaneRot = Vector3.zero;

        if (didHit)
        {
            //Debug.Log("Hit point " + hitInfo.point + " object " + hitInfo.collider.gameObject.name + " normal " + hitInfo.normal);
            //Debug.DrawLine(this._transform.position, hitInfo.point, Color.red, 0.1f);
            //Debug.DrawRay(hitInfo.point, hitInfo.normal * 0.2f, Color.yellow, 0.1f);

            CheckForBlock(hitInfo.point + -hitInfo.normal.Multiply(VoxelBlock.DefaultBlockSize * 0.05f));

            if (hitInfo.normal.y == 0f)
            {
                if (hitInfo.normal.x == 0f)
                {
                    //Z+ Front
                    if (hitInfo.normal.z > 0)
                    {
                        _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceZPlus;
                        facePlanePos.x = (int)hitInfo.point.x * VoxelBlock.DefaultBlockSize.x + (VoxelBlock.DefaultBlockSize.x * 0.5f);
                        facePlanePos.y = (int)hitInfo.point.y * VoxelBlock.DefaultBlockSize.y + (VoxelBlock.DefaultBlockSize.y * 0.5f);
                        facePlanePos.z = (int)hitInfo.point.z * VoxelBlock.DefaultBlockSize.z + 0.02f;

                        facePlaneRot.y = 180;
                    }
                    //Z - back
                    else
                    {
                        _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceZMinus;

                        facePlanePos.x = (int)hitInfo.point.x * VoxelBlock.DefaultBlockSize.x + (VoxelBlock.DefaultBlockSize.x * 0.5f);
                        facePlanePos.y = (int)hitInfo.point.y * VoxelBlock.DefaultBlockSize.y + (VoxelBlock.DefaultBlockSize.y * 0.5f);
                        facePlanePos.z = (int)hitInfo.point.z * VoxelBlock.DefaultBlockSize.z - 0.02f;

                        facePlaneRot.y = 0;
                    }
                }
                else
                {
                    //X+ Right
                    if (hitInfo.normal.x > 0)
                    {
                        _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceXPlus;

                        facePlanePos.x = (int)hitInfo.point.x * VoxelBlock.DefaultBlockSize.x + 0.02f;
                        facePlanePos.y = (int)hitInfo.point.y * VoxelBlock.DefaultBlockSize.y + (VoxelBlock.DefaultBlockSize.y * 0.5f);
                        facePlanePos.z = (int)hitInfo.point.z * VoxelBlock.DefaultBlockSize.z + (VoxelBlock.DefaultBlockSize.z * 0.5f);

                        facePlaneRot.y = 270f;
                    }
                    //X- Left
                    else
                    {
                        _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceXMinus;

                        facePlanePos.x = (int)hitInfo.point.x * VoxelBlock.DefaultBlockSize.x - 0.02f;
                        facePlanePos.y = (int)hitInfo.point.y * VoxelBlock.DefaultBlockSize.y + (VoxelBlock.DefaultBlockSize.y * 0.5f);
                        facePlanePos.z = (int)hitInfo.point.z * VoxelBlock.DefaultBlockSize.z + (VoxelBlock.DefaultBlockSize.z * 0.5f);

                        facePlaneRot.y = 90f;
                    }
                }
            }
            else
            {
                //Top face
                if (hitInfo.normal.y > 0)
                {
                    _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceYPlus;

                    facePlanePos.x = (int)hitInfo.point.x * VoxelBlock.DefaultBlockSize.x + (VoxelBlock.DefaultBlockSize.x * 0.5f);
                    facePlanePos.y = (int)hitInfo.point.y * VoxelBlock.DefaultBlockSize.y + 0.02f;
                    facePlanePos.z = (int)hitInfo.point.z * VoxelBlock.DefaultBlockSize.z + (VoxelBlock.DefaultBlockSize.z * 0.5f);

                    facePlaneRot.x = 90f;
                }
                //Bottom face
                else
                {
                    _curTargetFace = GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceYMinus;

                    facePlanePos.x = (int)hitInfo.point.x * VoxelBlock.DefaultBlockSize.x + (VoxelBlock.DefaultBlockSize.x * 0.5f);
                    facePlanePos.y = (int)hitInfo.point.y * VoxelBlock.DefaultBlockSize.y - 0.02f;
                    facePlanePos.z = (int)hitInfo.point.z * VoxelBlock.DefaultBlockSize.z + (VoxelBlock.DefaultBlockSize.z * 0.5f);

                    facePlaneRot.x = 270f;
                }
            }

            //Debug.Log("Pos " + facePlanePos + " rot " + facePlaneRot);

            _targetPlane.transform.position = facePlanePos;
            _targetPlane.transform.rotation = Quaternion.Euler(facePlaneRot);
        }
    }

    public void ResetSelectionBlock()
    {
        _targetCube.transform.localScale = Vector3.one;
        _targetPlane.transform.localScale = Vector3.one;
    }

    void OnPostRender()
    {
        //_lineBoundsDrawer.OnPostRender();
    }
}