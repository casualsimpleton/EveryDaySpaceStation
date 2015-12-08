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
    Transform _transform;

    [SerializeField]
    CharacterController _charController;

    [SerializeField]
    KeyCode _runKey = KeyCode.LeftShift;

    [SerializeField]
    float _walkSpeed = 2f;

    [SerializeField]
    float _runSpeed = 4f;

    TileLight _playerLight;

    void Start()
    {
        _transform = this.gameObject.transform;
        _charController = this.gameObject.GetComponentInChildren<CharacterController>();

        if (_charController == null)
        {
            Debug.LogError("Can't find charactercontroller for FPS controls");
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
    }

    public void SpawnTileLight()
    {
        _playerLight = new TileLight(new Color32(255, 255, 255, 255), 5);

        SceneLevelManager.Singleton.AddLight(_playerLight);
        _playerLight.IsMobile = true;
    }
}