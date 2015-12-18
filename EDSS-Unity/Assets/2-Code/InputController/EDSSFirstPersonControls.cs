//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EDSSFirstPersonControls - This is the first person character input for local client
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

public class EDSSFirstPersonControls : MonoBehaviour
{
    public Transform _transform;

    [SerializeField]
    CharacterController _charController;

    [SerializeField]
    KeyCode _runKey = KeyCode.LeftShift;

    [SerializeField]
    float _walkSpeed = 2f;

    [SerializeField]
    float _runSpeed = 4f;

    [SerializeField]
    Vec2Int _currentTile = new Vec2Int(0,0);

    EDSSFirstPersonCamera _edssCamera;

    TileLightGameObject _playerLight;
    
    void Awake()
    {
        _transform = this.gameObject.transform;
    }

    void Start()
    {        
        _charController = this.gameObject.GetComponentInChildren<CharacterController>();
        
        if (_charController == null)
        {
            Debug.LogError("Can't find charactercontroller for FPS controls");
        }

        _edssCamera = this.gameObject.GetComponent<EDSSFirstPersonCamera>();

        if (_edssCamera == null)
        {
            Debug.LogError("Can't find edssfirstpersoncamera");
        }
    }

    void Update()
    {
        float moveSpeed = _walkSpeed;
        if (Input.GetKey(_runKey))
        {
            moveSpeed = _runSpeed;
        }

        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Vector3 moveDir = (((_transform.forward * z) * moveSpeed) + ((_transform.right * x) * moveSpeed)) * Time.deltaTime;

        //if (moveDir != Vector3.zero)
        //{
        //    Debug.Log(string.Format("X {0} \tY {1} \tZ {2} --- \t{3}, \t{4}", moveDir.x, moveDir.y, moveDir.z, x, z));
        //}
        
        _charController.Move(moveDir);

        if (Input.GetKeyDown(KeyCode.L))
        {
            _playerLight.enabled = !_playerLight.enabled;
        }

        UpdateTilePosition();

        if (Input.GetKey(KeyCode.E))
        {
            if (_edssCamera.CurrentHighlitedESGO != null)
            {
                GameManager.Singleton.Client_AttemptInput(_edssCamera.CurrentHighlitedESGO);
            }
        }
    }

    public void SpawnTileLight()
    {
        //_playerLight = new TileLight(new Color32(255, 255, 255, 255), 5);

        //SceneLevelManager.Singleton.AddLight(_playerLight);
        //_playerLight.IsMobile = true;

        GameObject go = new GameObject("PlayerLight");
        go.transform.parent = _transform;
        go.transform.localPosition = Vector3.zero;

        _playerLight = go.AddComponent<TileLightGameObject>();
    }

    public void UpdateTilePosition()
    {
        Vector3 worldPosition = _transform.position;
        _currentTile.x = (int)worldPosition.x;
        _currentTile.y = (int)worldPosition.z;
    }
}