//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// MultiAngleComponent - Monobehaviour component for multi-angle stuffs which changes its sprite
// depending on viewing angle
// Created: December 14 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 14 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class MultiAngleComponent : MonoBehaviour
{
    protected EntitySpriteGameObject _entitySpriteObject;
    protected GameData.EntityDataTemplate.MultiAngleStateTemplate _currentMultiAngleTemplate;
    protected float _updateTimer;
    protected float _updateTimerDelta = 1f / 15f;

    public EntitySpriteGameObject EntitySpriteObject { get { return _entitySpriteObject; } }

    public void Create(EntitySpriteGameObject entitySpriteGO, GameData.EntityDataTemplate.MultiAngleStateTemplate initialMultiAngleTemplate)
    {
        _entitySpriteObject = entitySpriteGO;
        UpdateMultiAngleTemplate(initialMultiAngleTemplate);
    }

    public void UpdateMultiAngleTemplate(GameData.EntityDataTemplate.MultiAngleStateTemplate newState)
    {
        _currentMultiAngleTemplate = newState;

        _entitySpriteObject.UpdateSprite(_currentMultiAngleTemplate.SpriteUID);
        _entitySpriteObject.UpdateMesh();
        _entitySpriteObject.UpdateUVs();
    }

    void Update()
    {
        if (_updateTimer > Time.time)
        {
            return;
        }
        _updateTimer = Time.time + _updateTimerDelta;

        ////float yRot = _entitySpriteObject._transform.rotation.eulerAngles.y;
        float ang = Vector3.Angle(_entitySpriteObject._transform.forward,
            GameManager.Singleton.playerControl._transform.position - _entitySpriteObject._transform.position);

        float angDir = Helpers.AngleDir(_entitySpriteObject._transform.forward,
            GameManager.Singleton.playerControl._transform.position - _entitySpriteObject._transform.position, Vector3.up);

        float angMod = ang;
        ang *= angDir;
        if (angDir < 0)
        {
            angMod = ang + 360f;
        }

        //Debug.Log("Ang " + ang + " andDir " + angDir + " angMod " + angMod);

        GameData.EntityDataTemplate.MultiAngleStateTemplate template = null;
        foreach (KeyValuePair<ushort, GameData.EntityDataTemplate.MultiAngleStateTemplate> t in _entitySpriteObject.EntityData.Template.MultiAngleStates)
        {
            float minC1 = t.Value.AngleMinC1;
            float maxC1 = t.Value.AngleMaxC1;

            float minC2 = t.Value.AngleMinC2;
            float maxC2 = t.Value.AngleMaxC2;

            //if (ang > minC1 && ang < maxC1 && angMod > minC2 && angMod < maxC2)
            if (ang < 0)
            {
                if (ang > minC2)
                {
                    //Debug.Log("T1 " + t.Key + " Ang " + ang + " andDir " + angDir + " angMod " + angMod);
                    //key = t.Key;
                    template = t.Value;
                    break;
                }
                else if (angMod > minC1 && angMod < maxC1)
                {
                    //Debug.Log("T4 " + t.Key + " Ang " + ang + " andDir " + angDir + " angMod " + angMod);
                    //key = t.Key;
                    template = t.Value;
                    break;
                }
            }
            else
            {
                if (ang > minC2 && ang < maxC2)
                {
                    //Debug.Log("T2 " + t.Key + " Ang " + ang + " andDir " + angDir + " angMod " + angMod);
                    //key = t.Key;
                    template = t.Value;
                    break;
                }
                else if (angMod > minC1 && angMod < maxC1)
                {
                    //Debug.Log("T3 " + t.Key + " Ang " + ang + " andDir " + angDir + " angMod " + angMod);
                    //key = t.Key;
                    template = t.Value;
                    break;
                }
            }
        }

        UpdateMultiAngleTemplate(template);
    }
}