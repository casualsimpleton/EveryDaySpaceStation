//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ContainerComponent - Monobehaviour component for containers
// Created: January 16 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 16 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class ContainerComponent : MonoBehaviour
{
    public enum ContainerOpenState
    {
        Closed, //Lid is closed
        Opened, //Container is open
    }

    protected EntitySpriteGameObject _entitySpriteObject;
    [SerializeField]
    protected GameData.EntityDataTemplate.ContainerStateTemplate _currentContainerTemplate;
    [SerializeField]
    protected ContainerOpenState _containerOpenState;
    [SerializeField]
    protected float _timeWhenLastConditionChange;
    [SerializeField]
    protected bool _isActivated;
    [SerializeField]
    protected bool _hasLid;

    public EntitySpriteGameObject EntitySpriteObject { get { return _entitySpriteObject; } }
    public ContainerOpenState OpenState { get { return _containerOpenState; } }
    public float DurationLastStateChange { get { return (Time.time - _timeWhenLastConditionChange); } }
    public bool IsActivated { get { return _isActivated; } }
    public void ConsumeActivation() { _isActivated = false; }

    public void Create(EntitySpriteGameObject entitySpriteGo, GameData.EntityDataTemplate.ContainerStateTemplate template)
    {
        _entitySpriteObject = entitySpriteGo;
        UpdateContainerTemplate(template);

        _hasLid = template.HasLid;

        if (_hasLid)
        {

        }
    }

    public void UpdateContainerTemplate(GameData.EntityDataTemplate.ContainerStateTemplate newTemplate)
    {
        _currentContainerTemplate = newTemplate;
    }

    public void Activate()
    {
        _isActivated = true;

        //_currentCondition.CheckConditionTransitions(this);
    }
}