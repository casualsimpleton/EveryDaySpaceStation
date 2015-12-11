//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// TileLightGameObject - A gameobject based monobehaviour for carrying around a tilelight
// Created: December 9 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 9 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class TileLightGameObject : MonoBehaviour
{
    #region Vars
    protected TileLight _light;
    #endregion

    public void Create(TileLight newLight)
    {
        _light = newLight;
        _light.IsMobile = true;
        _light._transform = this.transform;
    }

    void Start()
    {
        TileLight newLight = new TileLight(new Color32(255, 255, 255, 255), 5);
        Create(newLight);

        SceneLevelManager.Singleton.AddLight(_light);
    }

    public void OnEnable()
    {
        if (_light != null)
        {
            SceneLevelManager.Singleton.AddLight(_light);
        }
    }

    public void OnDisable()
    {
        if (_light != null)
        {
            SceneLevelManager.Singleton.RemoveLight(_light);
        }
    }

    void OnDestroy()
    {
        if (_light != null)
        {
            SceneLevelManager.Singleton.RemoveLight(_light);
        }

        _light = null;
    }

    void Update()
    {
    }
}