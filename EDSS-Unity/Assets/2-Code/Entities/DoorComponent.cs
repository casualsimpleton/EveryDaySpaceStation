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
    protected EntitySpriteGameObject _entitySpriteObject;
    protected GameData.EntityDataTemplate.DoorStateTemplate _currentDoorTemplate;

    public EntitySpriteGameObject EntitySpriteObject { get { return _entitySpriteObject; } }

    public void Create(EntitySpriteGameObject entitySpriteGo, GameData.EntityDataTemplate.DoorStateTemplate template)
    {
        _entitySpriteObject = entitySpriteGo;
        UpdateDoorTemplate(template);
    }

    public void UpdateDoorTemplate(GameData.EntityDataTemplate.DoorStateTemplate newTemplate)
    {
        _currentDoorTemplate = newTemplate;
    }
}