//////////////////////////////////////////////////////////////////////////////////////////
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
    [SerializeField]
    protected float _curDurationLength;
    [SerializeField]
    protected bool _isActivated;

    public EntitySpriteGameObject EntitySpriteObject { get { return _entitySpriteObject; } }
    public DoorLockState LockState { get { return _lockState; } }
    public DoorPoweredState PowerState { get { return _poweredState; } }
    public bool IsWelded { get { return _isWelded; } }
    public bool IsDurationExceeded { get { return (_curDurationLength < Time.time ? true : false); } }
    public bool IsActivated { get { return _isActivated; } }
    public void ConsumeActivation() { _isActivated = false; } 

    public void Create(EntitySpriteGameObject entitySpriteGo, GameData.EntityDataTemplate.DoorStateTemplate template)
    {
        _entitySpriteObject = entitySpriteGo;
        UpdateDoorTemplate(template);

        //Set initial condition
        ChangeCondition(_currentDoorTemplate.DoorConditions[_currentDoorTemplate.InitialConditionUID]);
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
        Debug.Log(string.Format("{0} has new condition: {1}", _entitySpriteObject.name, newCondition.ToString()));
        _currentCondition = newCondition;
        _animTime = Time.time + _currentCondition.ConditionDefaultSpeed;
        _currentConditionStateIndex = 0;
        if (_currentCondition.ConditionDuration > 0f)
        {
            _curDurationLength = Time.time + _currentCondition.ConditionDuration;
        }
        else
        {
            _curDurationLength = 0f;
        }
    }

    void Update()
    {
        if (_animTime > Time.time)
        {
            return;
        }

        if (_currentCondition == null)
            return;

        ////Check for bounds, as we may be waiting on time even though we haven't exhausted all the frames
        //if (_currentConditionStateIndex > _currentCondition.ReferencedStates.Length - 1)
        //    return;

        if (_currentConditionStateIndex < _currentCondition.ReferencedStates.Length)
        {

            ushort stateUID = 0;

#if DEBUGCLIENT
            try
            {
                stateUID = _currentCondition.ReferencedStates[_currentConditionStateIndex];
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Exception " + ex.Message);
            }
#else
        stateUID = _currentCondition.ReferencedStates[_currentConditionStateIndex];
#endif

            GameData.EntityDataTemplate.StateTemplate state = _entitySpriteObject.EntityData.Template.EntityStates[stateUID];

            _entitySpriteObject.UpdateSprite(state.SpriteUID);
            _entitySpriteObject.UpdateMaterial();

            _entitySpriteObject.UpdateMesh();
            _entitySpriteObject.UpdateUVs();

            _entitySpriteObject.SetColliderState(_currentCondition.ConditionHasColliders[_currentConditionStateIndex]);

            _animTime = Time.time + _currentCondition.ConditionDefaultSpeed;

            _currentConditionStateIndex++;
        }

        //Either completed number of states (frames)
        //Or duration
        if (_currentConditionStateIndex > _currentCondition.ReferencedStates.Length - 1 ||
            (_currentCondition.ConditionDuration > 0f && Time.time > _curDurationLength))
        {

            if (_currentCondition.ConditionTransitions == null)
            {
                Debug.Log(string.Format("Condition: {0} has no transitions. Looping...", _currentCondition.ToString()));
                _currentConditionStateIndex = 0;
                return;
            }

            //Check to see if transitions are satisified           
            bool isTransitionSatisified = _currentCondition.CheckConditionTransitions(this);

            //Transition was met, we're moving on to another one via TransitionSatisifed()
            if (isTransitionSatisified)
            {
                return;
            }
        }
    }

    public void Activate()
    {
        _isActivated = true;
        //Door is welded, so do something welded related?
        //if (_isWelded)
        //{
        //    Debug.Log("Door is welded");
        //    return;
        //}

        //switch (_poweredState)
        //{
        //    case DoorPoweredState.Powered:
        //        if (_lockState == DoorLockState.Unlocked)
        //        {
        //            Debug.Log(string.Format("Door {0} UID {1} (template {2}) is powered and unlocked", 
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
        //        }
        //        else if (_lockState == DoorLockState.Locked)
        //        {
        //            Debug.Log(string.Format("Door {0} UID {1} (template {2}) is powered and locked", 
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
        //        }
        //        else if (_lockState == DoorLockState.Restricted)
        //        {
        //            Debug.Log(string.Format("Door {0} UID {1} (template {2}) is powered and restricted",
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
        //        }
        //        else
        //        {
        //            Debug.LogWarning(string.Format("Door {0} UID {1} (template {2}) is powered and unhandled: {3}",
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID, _lockState));
        //        }

        //        break;

        //    case DoorPoweredState.Unpowered:
        //        if (_lockState == DoorLockState.Unlocked)
        //        {
        //            Debug.Log(string.Format("Door {0} UID {1} (template {2}) is unpowered and unlocked",
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));

        //            GameData.EntityDataTemplate.DoorStateTemplate.DoorConditionTemplate newCond = _currentDoorTemplate.DoorConditions[1];
        //            ChangeCondition(newCond);
        //        }
        //        else if (_lockState == DoorLockState.Locked)
        //        {
        //            Debug.Log(string.Format("Door {0} UID {1} (template {2}) is unpowered and locked",
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
        //        }
        //        else if (_lockState == DoorLockState.Restricted)
        //        {
        //            Debug.Log(string.Format("Door {0} UID {1} (template {2}) is unpowered and restricted",
        //                _entitySpriteObject.EntityData.EntityName, _entitySpriteObject.EntityData.EntityUID, _entitySpriteObject.EntityData.TemplateUID));
        //        }
        //        break;

        //    case DoorPoweredState.Hacked:
        //        break;
        //}
    }

    public void TransitionSatisfied(GameData.EntityDataTemplate.DoorStateTemplate.DoorConditionTemplate.DoorTransitionTemplate transitionSatisified)
    {
        ChangeCondition(_currentDoorTemplate.DoorConditions[transitionSatisified.TransitionTargetConditionUID]);

        //Check to see if the transition required an action, and if so, mark it consumed
        for (int i = 0; i < transitionSatisified.TransitionRequirements.Count; i++)
        {
            if (transitionSatisified.TransitionRequirements[i].First == EntityTransitionVariables.NextAction)
            {
                ConsumeActivation();
                break;
            }
        }
    }
}