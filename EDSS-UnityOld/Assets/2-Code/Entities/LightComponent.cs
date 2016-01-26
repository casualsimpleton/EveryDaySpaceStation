//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// LightComponent - Monobehaviour component for lights
// Created: December 13 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 13 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class LightComponent : MonoBehaviour
{
    protected EntitySpriteGameObject _entitySpriteObject;
    [SerializeField]
    protected bool _isMobile;
    protected GameData.EntityDataTemplate.LightStateTemplate _currentLightTemplate;
    [SerializeField]
    Color32 _lightColor = new Color32(255, 255, 255, 255);
    
    public EntitySpriteGameObject EntitySpriteObject { get { return _entitySpriteObject; } }    
    public bool IsMobile { get { return _isMobile; } }
    public Color32 LightColor { get { return _lightColor; } }
    public int LightRadius { get { return _currentLightTemplate.LightRadius; } }

    public void Create(EntitySpriteGameObject entitySpriteGO, GameData.EntityDataTemplate.LightStateTemplate initialLightTemplate)
    {
        _entitySpriteObject = entitySpriteGO;
        UpdateLightTemplate(initialLightTemplate);

        _isMobile = false;
        //If it has fixed states, then it's immobile
        if(_entitySpriteObject.EntityData.Template.FixedStates == null)
        {
            _isMobile = true;
        }
    }

    public void UpdateLightTemplate(GameData.EntityDataTemplate.LightStateTemplate newState)
    {
        _currentLightTemplate = newState;

        if (_currentLightTemplate.LightRadius < 1)
        {
            SceneLevelManager.Singleton.RemoveLight(this);
        }
        else
        {
            SceneLevelManager.Singleton.AddLight(this);
        }
    }

    public void UpdatePosition()
    {
    }
}