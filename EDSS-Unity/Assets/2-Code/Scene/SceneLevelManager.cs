//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneLevelManager - Unity Monobehaviour that sits on a gameobject and helps to control the rendering of the world within the game scene
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class SceneLevelManager : MonoBehaviour
{
    #region Singleton
    protected static SceneLevelManager m_singleton = null;
    public static SceneLevelManager Singleton
    {
        get
        {
            return m_singleton;
        }
    }

    void Awake()
    {
        m_singleton = this;
    }
    #endregion


}