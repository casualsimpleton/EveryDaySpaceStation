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

    [SerializeField]
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
    [SerializeField]
    protected EntitySpriteGraphics _lidGraphics;

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
            _lidGraphics = PoolManager.Singleton.RequestEntitySpriteGraphics();
            _lidGraphics.Detach();

            _lidGraphics.AttachToEntitySpriteGO(_entitySpriteObject);
            _lidGraphics.Create(_entitySpriteObject);

            _entitySpriteObject.AddAdditionalSprite(_lidGraphics);           

            _lidGraphics.UpdateSprite(_currentContainerTemplate.LidFrontSpriteUID);
            _lidGraphics.UpdateMesh(_entitySpriteObject.EntityData.CurrentEntityState.StateTemplate, false);
            _lidGraphics.UpdateUVs();

            _lidGraphics.UpdateScale(_currentContainerTemplate.LidGraphicSize);
            _lidGraphics.UpdateOffset(_currentContainerTemplate.LidPositionOffset);
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

    public void UpdateMultiAngle(ushort containerStateUID, float angle, float angleDir)
    {
        //The lid is dependent on the container. We assume the config file is correct
        //Based on the containerStateUID, we'll determine what version of the lid (if any) to display
        
        bool displayFront = false;
        bool displayBack = false;

        for (int i = 0; i < _currentContainerTemplate.LidContainerFrontStates.Length; i++)
        {
            if (_currentContainerTemplate.LidContainerFrontStates[i] == containerStateUID)
            {
                displayFront = true;
                break;
            }
        }

        if (displayFront)
        {
            //Display the front version of the lid
            _lidGraphics.Enable();
            _lidGraphics.UpdateSprite(_currentContainerTemplate.LidFrontSpriteUID);
            _lidGraphics.UpdateMesh(_entitySpriteObject.EntityData.CurrentEntityState.StateTemplate, false);
            _lidGraphics.UpdateUVs();

            _lidGraphics.UpdateScale(_currentContainerTemplate.LidGraphicSize);
            _lidGraphics.UpdateOffset(_currentContainerTemplate.LidPositionOffset);
            return;
        }

        for (int i = 0; i < _currentContainerTemplate.LidContainerBackStates.Length; i++)
        {
            if (_currentContainerTemplate.LidContainerBackStates[i] == containerStateUID)
            {
                displayBack = true;
                break;
            }
        }

        if (displayBack)
        {
            //Display the front version of the lid
            _lidGraphics.Enable();
            _lidGraphics.UpdateSprite(_currentContainerTemplate.LidBackSpriteUID);
            _lidGraphics.UpdateMesh(_entitySpriteObject.EntityData.CurrentEntityState.StateTemplate, false);
            _lidGraphics.UpdateUVs();

            _lidGraphics.UpdateScale(_currentContainerTemplate.LidGraphicSize);
            _lidGraphics.UpdateOffset(_currentContainerTemplate.LidPositionOffset);
            return;
        }

        //No state present, so disable it
        _lidGraphics.Disable();
    }
}