﻿//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// DoorComponent - Monobehaviour component for doors
// Created: December 17 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 17 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class DoorComponent : MonoBehaviour
{
    public enum DoorLockState
    {
        Unlocked, //Unlocked for everyone
        Locked, //Locked for everyone
        Restricted, //Restricted to certain security clearences
    }

    public enum DoorPoweredState
    {
        Unpowered,
        Powered,
        Hacked
    }

    protected EntitySpriteGameObject _entitySpriteObject;
    [SerializeField]
    protected GameData.EntityDataTemplate.DoorStateTemplate _currentDoorTemplate;
    [SerializeField]
    protected GameData.EntityDataTemplate.DoorStateTemplate.DoorConditionTemplate _currentCondition;
    [SerializeField]
    protected DoorLockState _lockState;
    [SerializeField]
    protected DoorPoweredState _poweredState;
    [SerializeField]
    protected bool _isWelded;
    [SerializeField]
    protected float _animTime;
    [SerializeField]
    protected ushort _currentConditionStateIndex;

    public EntitySpriteGameObject EntitySpriteObject { get { return _entitySpriteObject; } }
    public DoorLockState LockState { get { return _lockState; } }
    public DoorPoweredState PowerState { get { return _poweredState; } }
    public bool IsWelded { get { return _isWelded; } }

    public void Create(EntitySpriteGameObject entitySpriteGo, GameData.EntityDataTemplate.DoorStateTemplate template)
    {
        _entitySpriteObject = entitySpriteGo;
        UpdateDoorTemplate(template);
    }

    public void UpdateDoorTemplate(GameData.EntityDataTemplate.DoorStateTemplate newTemplate)
    {
        _currentDoorTemplate = newTemplate;

        if (_currentDoorTemplate.DoorConditions == null || _currentDoorTemplate.DoorConditions.Count < 1)
        {
            Debug.LogError(string.Format("No door conditions found for current door template: UID {0} Name {1}",
                _currentDoorTemplate.ReferencedEntityDataTemplate.UID, _currentDoorTemplate.ReferencedEntityDataTemplate.Name));
            return;
        }
    }

    public void ChangeLockState(DoorLockState newState)
    {
        _lockState = newState;
    }

    public void ChangePowerState(DoorPoweredState newState)
    {
        _poweredState = newState;
    }

    public void ChangeWelded(bool isWelded)
    {
        _isWelded = isWelded;
    }

    public void ChangeCondition(GameData.EntityDataTemplate.DoorStateTemplate.DoorConditionTemplate newCondition)
    {
        _currentCondition = newCondition;
        _animTime = Time.time + _currentCondition.ConditionDefaultSpeed;
        _currentConditionStateIndex = 0;
    }

    void Update()
    {
        if (_animTime > Time.time)
        {
            return;
        }

        if (_currentCondition == null)
            return;

        ushort stateUID = _currentCondition.ReferencedStates[_currentConditionStateIndex];

        GameData.EntityDataTemplate.StateTemplate state = _entitySpriteObject.EntityData.Template.EntityStates[stateUID];

        _entitySpriteObject.UpdateSprite(state.SpriteUID);
        _entitySpriteObject.UpdateMaterial();

        _entitySpriteObject.UpdateMesh();
        _entitySpriteObject.UpdateUVs();

        _animTime = Time.time + _currentCondition.ConditionDefaultSpeed;

        _currentConditionStateIndex++;

        if (_currentConditionStateIndex > _currentCondition.ReferencedStates.Length - 1)
        {
            _currentConditionStateIndex = 0;
        }
    }

    public void Activate()
    {
        //Door is welded, so do something welded related?
        if (_isWelded)
        {
            Debug.Log("Door is welded");
            return;
        }

        switch (_poweredState)
        {
            case DoorPoweredState.Powered:
                if (_lockState == DoorLockState.Unlocked)
                {
                    Debug.Log(string.Format("Door {0} UID {1} (template {2}) is powered and unlocked", 
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
                }
                else if (_lockState == DoorLockState.Locked)
                {
                    Debug.Log(string.Format("Door {0} UID {1} (template {2}) is powered and locked", 
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
                }
                else if (_lockState == DoorLockState.Restricted)
                {
                    Debug.Log(string.Format("Door {0} UID {1} (template {2}) is powered and restricted",
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
                }
                else
                {
                    Debug.LogWarning(string.Format("Door {0} UID {1} (template {2}) is powered and unhandled: {3}",
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID, _lockState));
                }

                break;

            case DoorPoweredState.Unpowered:
                if (_lockState == DoorLockState.Unlocked)
                {
                    Debug.Log(string.Format("Door {0} UID {1} (template {2}) is unpowered and unlocked",
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));

                    GameData.EntityDataTemplate.DoorStateTemplate.DoorConditionTemplate newCond = _currentDoorTemplate.DoorConditions[1];
                    ChangeCondition(newCond);
                }
                else if (_lockState == DoorLockState.Locked)
                {
                    Debug.Log(string.Format("Door {0} UID {1} (template {2}) is unpowered and locked",
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
                }
                else if (_lockState == DoorLockState.Restricted)
                {
                    Debug.Log(string.Format("Door {0} UID {1} (template {2}) is unpowered and restricted",
                        _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
                }
                break;

            case DoorPoweredState.Hacked:
                break;
        }
    }
}